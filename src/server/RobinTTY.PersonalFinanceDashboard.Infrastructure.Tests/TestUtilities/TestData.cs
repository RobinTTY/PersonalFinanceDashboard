using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

/// <summary>
/// Creates entities to seed the database with.
/// </summary>
internal static class TestData
{
    public static BankingInstitution Institution(string thirdPartyId) =>
        new(thirdPartyId, "TESTDEFF", "Test Bank", new Uri("https://localhost/logo.png"), ["DE"]);

    public static BankAccount Account(Guid thirdPartyId, string? currency = "EUR", decimal? balance = null) =>
        new(thirdPartyId, name: "Account", currency: currency, balance: balance);

    public static AuthenticationRequest Request(Guid thirdPartyId, DateTime createdAt, params BankAccount[] accounts) =>
        new(thirdPartyId, AuthenticationStatus.Active, new Uri("https://localhost/link"), createdAt, accounts.ToList());

    public static Transaction Transaction(Guid thirdPartyId, BankAccount account, decimal amount,
        string category = "", string notes = "") =>
        new(thirdPartyId, $"bank-{thirdPartyId}", new DateTime(2026, 11, 10), null, null, amount, "EUR", category,
            notes,
            [], account);
}