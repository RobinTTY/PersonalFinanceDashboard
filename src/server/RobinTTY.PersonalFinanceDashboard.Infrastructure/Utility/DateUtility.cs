namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Utility;

public enum TimeUnit
{
    Seconds,
    Minutes,
    Hours,
    Days
}

public static class DateUtility
{
    /// <summary>
    /// Determines whether the given date is older than the specified amount of time.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="amount">The amount of time to compare against.</param>
    /// <param name="unit">The unit of time for <paramref name="amount"/>.</param>
    /// <returns><c>true</c> if the date is older than the specified time; otherwise, <c>false</c>.</returns>
    public static bool IsOlderThan(DateTime date, double amount, TimeUnit unit)
    {
        var elapsed = (DateTime.UtcNow - date.ToUniversalTime()).TotalSeconds;
        var threshold = unit switch
        {
            TimeUnit.Seconds => amount,
            TimeUnit.Minutes => amount * 60,
            TimeUnit.Hours   => amount * 3600,
            TimeUnit.Days    => amount * 86400,
            _                => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
        
        return elapsed > threshold;
    }
}