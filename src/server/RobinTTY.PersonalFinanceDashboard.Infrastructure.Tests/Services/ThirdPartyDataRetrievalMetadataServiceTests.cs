using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.Services;

public class ThirdPartyDataRetrievalMetadataServiceTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 4)]
    [InlineData(5, 16)]
    [InlineData(6, 24)]
    [InlineData(1000, 24)]
    public void RetryDelay_DoublesWithEveryFailureUpToTheRetrievalInterval(int consecutiveFailures,
        int expectedHours)
    {
        var delay = ThirdPartyDataRetrievalMetadataService.GetRetryDelay(consecutiveFailures, TimeSpan.FromDays(1));

        Assert.Equal(TimeSpan.FromHours(expectedHours), delay);
    }
}