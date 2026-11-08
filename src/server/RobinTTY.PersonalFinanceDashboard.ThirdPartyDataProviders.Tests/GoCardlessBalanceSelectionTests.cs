using RobinTTY.NordigenApiClient.Models.Responses;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Tests;

public class GoCardlessBalanceSelectionTests
{
    [Fact]
    public void InterimBooked_IsPreferredOverOtherTypes()
    {
        var balance = SelectBalance(
            (BalanceType.InterimAvailable, 1500m),
            (BalanceType.ClosingBooked, 900m),
            (BalanceType.InterimBooked, 1000m));

        Assert.Equal(1000m, balance);
    }

    [Fact]
    public void BookedBalance_IsPreferredOverAvailableBalance()
    {
        // The available balance can include a credit line
        var balance = SelectBalance(
            (BalanceType.InterimAvailable, 1500m),
            (BalanceType.ClosingBooked, 900m));

        Assert.Equal(900m, balance);
    }

    [Fact]
    public void AvailableBalance_IsUsedWhenNothingElseIsReported()
    {
        var balance = SelectBalance((BalanceType.InterimAvailable, 1500m));

        Assert.Equal(1500m, balance);
    }

    [Fact]
    public void UnknownBalanceTypes_AreIgnored()
    {
        var balance = SelectBalance((BalanceType.Information, 42m), (BalanceType.NonInvoiced, 7m));

        Assert.Null(balance);
    }

    private static decimal? SelectBalance(params (BalanceType Type, decimal Amount)[] balances)
    {
        var mapped = balances
            .Select(b => new Balance(new AmountCurrencyPair(b.Amount, "EUR"), b.Type, DateTime.UtcNow, null, null, ""))
            .ToList();

        return GoCardlessTypeExtensions.CreateBankAccount(Guid.NewGuid(), null, null, mapped).Balance;
    }
}