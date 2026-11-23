using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using NordigenTransaction = RobinTTY.NordigenApiClient.Models.Responses.Transaction;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Tests;

public class GoCardlessTransactionMappingTests
{
    private static readonly Guid AccountId = Guid.Parse("7e944232-bda9-40bc-b784-660c7ab5fe78");
    private static readonly Guid OtherAccountId = Guid.Parse("3b8f0a6e-2c4d-4e1f-9a7b-5d6c8e9f0a1b");

    // GoCardless sends amounts as strings
    private static readonly JsonSerializerOptions SerializerOptions =
        new() { NumberHandling = JsonNumberHandling.AllowReadingFromString };

    [Fact]
    public void GuidInternalTransactionId_IsUsedAsIs()
    {
        // Keeps ids of previously synced transactions unchanged
        var transaction = CreateTransaction(internalTransactionId: "a2fd8a8e9d4b4d6c8a1d3b7e8f9a0b1c");

        var id = Map(AccountId, transaction).Single();

        Assert.Equal(Guid.Parse("a2fd8a8e-9d4b-4d6c-8a1d-3b7e8f9a0b1c"), id);
    }

    [Fact]
    public void NonGuidInternalTransactionId_IsUsedInsteadOfBankId()
    {
        var first = CreateTransaction(internalTransactionId: "tx-1", transactionId: "same-bank-id");
        var second = CreateTransaction(internalTransactionId: "tx-2", transactionId: "same-bank-id");

        var ids = Map(AccountId, first, second);

        Assert.NotEqual(ids[0], ids[1]);
    }

    [Fact]
    public void MissingInternalTransactionId_UsesBankTransactionId()
    {
        // Same bank id but different content, so the id must come from the bank id, not the content
        var first = CreateTransaction(transactionId: "2026071800001", amount: "-10.00");
        var second = CreateTransaction(transactionId: "2026071800001", amount: "-12.50");

        var ids = Map(AccountId, first, second);

        Assert.Equal(ids[0], ids[1]);
    }

    [Fact]
    public void MissingTransactionIds_UsesEntryReference()
    {
        var first = CreateTransaction(entryReference: "REF-42", amount: "-10.00");
        var second = CreateTransaction(entryReference: "REF-42", amount: "-12.50");

        var ids = Map(AccountId, first, second);

        Assert.Equal(ids[0], ids[1]);
    }

    [Fact]
    public void SameBankTransactionId_InDifferentAccounts_GetsDifferentIds()
    {
        var transaction = CreateTransaction(transactionId: "2026071800001");

        var id = Map(AccountId, transaction).Single();
        var otherId = Map(OtherAccountId, transaction).Single();

        Assert.NotEqual(id, otherId);
    }

    [Fact]
    public void MissingAllIds_DifferentTransactionsGetDifferentIds()
    {
        var ids = Map(AccountId, CreateTransaction(amount: "-10.00"), CreateTransaction(amount: "-12.50"));

        Assert.Equal(2, ids.Distinct().Count());
    }

    [Fact]
    public void MissingAllIds_IdenticalTransactionsGetDistinctIds()
    {
        // e.g. buying the same coffee twice on the same day
        var ids = Map(AccountId, CreateTransaction(), CreateTransaction(), CreateTransaction());

        Assert.Equal(3, ids.Distinct().Count());
    }

    [Fact]
    public void MissingAllIds_IdsAreStableAcrossSyncs()
    {
        var firstSync = Map(AccountId, CreateTransaction(amount: "-10.00"), CreateTransaction(), CreateTransaction());
        var secondSync = Map(AccountId, CreateTransaction(amount: "-10.00"), CreateTransaction(), CreateTransaction());

        Assert.Equal(firstSync, secondSync);
        Assert.DoesNotContain(Guid.Empty, firstSync);
    }

    [Fact]
    public void DebtorIsPayer_CreditorIsPayee()
    {
        var transaction = CreateTransaction(internalTransactionId: "tx-1");

        var mapped = MapBooked(transaction).Single();

        Assert.Equal("Jane Doe", mapped.Payer);
        Assert.Equal("Corner Shop", mapped.Payee);
        Assert.Equal(AccountId.ToString(), mapped.BankAccount?.ExternalId);
    }

    [Fact]
    public void MappedTransactions_AreFromGoCardlessWithLowercaseIds()
    {
        var transaction = CreateTransaction(internalTransactionId: "A9A6B2F4-1C2D-4E5F-8A9B-0C1D2E3F4A5B");

        var mapped = MapBooked(transaction).Single();

        Assert.Equal(DataSource.GoCardless, mapped.Source);
        Assert.Equal("a9a6b2f4-1c2d-4e5f-8a9b-0c1d2e3f4a5b", mapped.ExternalId);
        Assert.Equal(DataSource.GoCardless, mapped.BankAccount?.Source);
    }

    [Fact]
    public void BookingDateAndDescription_AreMapped()
    {
        var mapped = MapBooked(CreateTransaction()).Single();

        Assert.Equal(new DateTime(2026, 7, 18), mapped.BookingDate);
        Assert.Equal("Card payment", mapped.Description);
    }

    [Fact]
    public void DescriptionLines_AreUsedWithoutUnstructuredDescription()
    {
        var transaction = CreateTransaction(configure: json =>
        {
            json.Remove("remittanceInformationUnstructured");
            json["remittanceInformationUnstructuredArray"] = new JsonArray("Corner Shop", "Card 1234");
        });

        Assert.Equal("Corner Shop Card 1234", MapBooked(transaction).Single().Description);
    }

    [Fact]
    public void PendingTransactions_AreMappedAfterTheBookedOnes()
    {
        var booked = CreateTransaction(transactionId: "booked");
        var pending = CreateTransaction(transactionId: "pending", configure: json => json.Remove("bookingDate"));

        var mapped = new AccountTransactions([booked], [pending]).ToTransactions(AccountId);

        Assert.Equal([("booked", TransactionStatus.Booked), ("pending", TransactionStatus.Pending)],
            mapped.Select(transaction => (transaction.BankTransactionId, transaction.Status)));
        Assert.Null(mapped[1].BookingDate);
    }

    [Fact]
    public void MissingAllIds_PendingTransactionsDoNotChangeTheIdsOfBookedOnes()
    {
        var bookedOnly = Map(AccountId, CreateTransaction(), CreateTransaction());
        var withPending = new AccountTransactions([CreateTransaction(), CreateTransaction()], [CreateTransaction()])
            .ToTransactions(AccountId).Select(transaction => Guid.Parse(transaction.ExternalId!)).ToList();

        Assert.Equal(bookedOnly, withPending.Take(2));
        Assert.Equal(3, withPending.Distinct().Count());
    }

    private static List<Transaction> MapBooked(params NordigenTransaction[] transactions) =>
        new AccountTransactions(transactions.ToList(), []).ToTransactions(AccountId);

    private static List<Guid> Map(Guid accountId, params NordigenTransaction[] transactions) =>
        new AccountTransactions(transactions.ToList(), []).ToTransactions(accountId)
            .Select(transaction => Guid.Parse(transaction.ExternalId!)).ToList();

    /// <summary>
    /// Creates a transaction the way the API client would, by deserializing a GoCardless response.
    /// </summary>
    private static NordigenTransaction CreateTransaction(string? internalTransactionId = null,
        string? transactionId = null, string? entryReference = null, string amount = "-4.20",
        Action<JsonObject>? configure = null)
    {
        var json = new JsonObject
        {
            ["bookingDate"] = "2026-07-18",
            ["valueDate"] = "2026-07-18",
            ["transactionAmount"] = new JsonObject { ["amount"] = amount, ["currency"] = "EUR" },
            ["debtorName"] = "Jane Doe",
            ["creditorName"] = "Corner Shop",
            ["remittanceInformationUnstructured"] = "Card payment"
        };

        if (internalTransactionId != null)
            json["internalTransactionId"] = internalTransactionId;
        if (transactionId != null)
            json["transactionId"] = transactionId;
        if (entryReference != null)
            json["entryReference"] = entryReference;
        configure?.Invoke(json);

        return JsonSerializer.Deserialize<NordigenTransaction>(json.ToJsonString(), SerializerOptions)!;
    }
}
