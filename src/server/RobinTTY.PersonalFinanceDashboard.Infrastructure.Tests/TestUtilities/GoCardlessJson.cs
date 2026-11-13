using System.Globalization;
using System.Text.Json;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

/// <summary>
/// Builds GoCardless API responses and the paths they are served at.
/// </summary>
internal static class GoCardlessJson
{
    public const string RequisitionsPath = "requisitions/";
    public const string InstitutionsPath = "institutions/";

    public static string RequisitionPath(Guid requisitionId) => $"requisitions/{requisitionId}/";

    public static string BalancesPath(Guid accountId) => $"accounts/{accountId}/balances/";
    public static string DetailsPath(Guid accountId) => $"accounts/{accountId}/details/";
    public static string AccountPath(Guid accountId) => $"accounts/{accountId}/";
    public static string TransactionsPath(Guid accountId) => $"accounts/{accountId}/transactions/";

    /// <param name="status">The GoCardless status code, e.g. "LN" (linked) or "EX" (expired).</param>
    public static object Requisition(Guid id, string status, DateTime created, params Guid[] accounts) =>
        new Dictionary<string, object?>
        {
            ["id"] = id,
            ["created"] = created,
            ["redirect"] = "https://localhost/",
            ["status"] = status,
            ["institution_id"] = "SANDBOXFINANCE_SFIN0000",
            ["agreement"] = Guid.NewGuid(),
            ["reference"] = id.ToString(),
            ["accounts"] = accounts,
            ["user_language"] = "EN",
            ["link"] = "https://localhost/link",
            ["ssn"] = null,
            ["account_selection"] = false,
            ["redirect_immediate"] = false
        };

    public static string Requisitions(params object[] requisitions) =>
        JsonSerializer.Serialize(new
            { count = requisitions.Length, next = (string?)null, previous = (string?)null, results = requisitions });

    public static string RequisitionJson(Guid id, string status, DateTime created, params Guid[] accounts) =>
        JsonSerializer.Serialize(Requisition(id, status, created, accounts));

    public static string Deleted(string summary) =>
        JsonSerializer.Serialize(new { summary, detail = summary });

    public static string Institutions(params (string Id, string Name)[] institutions) =>
        JsonSerializer.Serialize(institutions.Select(institution => new Dictionary<string, object>
        {
            ["id"] = institution.Id,
            ["name"] = institution.Name,
            ["bic"] = "TESTDEFF",
            ["transaction_total_days"] = "730",
            ["countries"] = new[] { "DE" },
            ["logo"] = "https://localhost/logo.png",
            ["max_access_valid_for_days"] = "180"
        }));

    public static string Account(Guid accountId, string institutionId) =>
        JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["id"] = accountId,
            ["created"] = "2026-11-01T10:00:00Z",
            ["last_accessed"] = "2026-11-01T10:00:00Z",
            ["iban"] = "DE00123456789",
            ["institution_id"] = institutionId,
            ["status"] = "READY",
            ["owner_name"] = "Owner"
        });

    public static string Details(Guid accountId, string currency, string product) =>
        JsonSerializer.Serialize(new
        {
            account = new
            {
                resourceId = accountId.ToString(),
                iban = "DE00123456789",
                currency,
                ownerName = "Owner",
                product,
                cashAccountType = "CACC"
            }
        });

    public static string Balances(decimal amount) =>
        JsonSerializer.Serialize(new
        {
            balances = new[]
            {
                new
                {
                    balanceAmount = new { amount = Format(amount), currency = "EUR" },
                    balanceType = "interimBooked",
                    referenceDate = "2026-11-13"
                }
            }
        });

    public static string Transactions(params (Guid Id, decimal Amount)[] transactions) =>
        JsonSerializer.Serialize(new
        {
            transactions = new
            {
                booked = transactions.Select(transaction => new
                {
                    transactionId = $"bank-{transaction.Id}",
                    internalTransactionId = transaction.Id.ToString("N"),
                    bookingDate = "2026-11-10",
                    valueDate = "2026-11-10",
                    transactionAmount = new { amount = Format(transaction.Amount), currency = "EUR" }
                }),
                pending = Array.Empty<object>()
            }
        });

    private static string Format(decimal amount) => amount.ToString(CultureInfo.InvariantCulture);
}