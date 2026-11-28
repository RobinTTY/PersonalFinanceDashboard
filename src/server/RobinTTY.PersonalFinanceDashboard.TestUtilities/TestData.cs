using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.TestUtilities;

/// <summary>
/// Creates entities to seed the database with.
/// </summary>
public static class TestData
{
    public static BankingInstitution Institution(string externalId) =>
        new(DataSource.GoCardless, externalId, "TESTDEFF", "Test Bank", new Uri("https://localhost/logo.png"), ["DE"]);

    public static BankingInstitution ManualInstitution() =>
        new(DataSource.Manual, null, "TESTDEFF", "My Bank", new Uri("https://localhost/logo.png"), ["DE"]);

    public static BankAccount Account(Guid externalId, string? currency = "EUR", decimal? balance = null) =>
        new(DataSource.GoCardless, externalId.ToExternalId(), currency: currency, balance: balance,
            iban: $"DE{externalId:N}", providerName: "Account");

    public static BankAccount ManualAccount() => new(DataSource.Manual, null, name: "Cash", currency: "EUR");

    public static AuthenticationRequest Request(Guid externalId, DateTime createdAt, params BankAccount[] accounts) =>
        new(DataSource.GoCardless, externalId.ToExternalId(), AuthenticationStatus.Active,
            new Uri("https://localhost/link"), createdAt, accounts.ToList());

    public static Transaction Transaction(Guid externalId, BankAccount account, decimal amount,
        string? category = null, string? notes = null, DateTime? valueDate = null,
        TransactionStatus status = TransactionStatus.Booked) =>
        new(DataSource.GoCardless, externalId.ToExternalId(), $"bank-{externalId}",
            valueDate ?? new DateTime(2026, 11, 10), null, null, amount, "EUR", category, notes, account, status);
}
