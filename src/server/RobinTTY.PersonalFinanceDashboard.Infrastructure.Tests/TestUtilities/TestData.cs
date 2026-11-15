using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

/// <summary>
/// Creates entities to seed the database with.
/// </summary>
internal static class TestData
{
    public static BankingInstitution Institution(string externalId) =>
        new(DataSource.GoCardless, externalId, "TESTDEFF", "Test Bank", new Uri("https://localhost/logo.png"), ["DE"]);

    public static BankingInstitution ManualInstitution() =>
        new(DataSource.Manual, null, "TESTDEFF", "My Bank", new Uri("https://localhost/logo.png"), ["DE"]);

    public static BankAccount Account(Guid externalId, string? currency = "EUR", decimal? balance = null) =>
        new(DataSource.GoCardless, externalId.ToExternalId(), name: "Account", currency: currency, balance: balance);

    public static BankAccount ManualAccount() => new(DataSource.Manual, null, name: "Cash", currency: "EUR");

    public static AuthenticationRequest Request(Guid externalId, DateTime createdAt, params BankAccount[] accounts) =>
        new(DataSource.GoCardless, externalId.ToExternalId(), AuthenticationStatus.Active,
            new Uri("https://localhost/link"), createdAt, accounts.ToList());

    public static Transaction Transaction(Guid externalId, BankAccount account, decimal amount,
        string category = "", string notes = "") =>
        new(DataSource.GoCardless, externalId.ToExternalId(), $"bank-{externalId}", new DateTime(2026, 11, 10), null,
            null, amount, "EUR", category, notes, [], account);
}