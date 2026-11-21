using System.Threading.Channels;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <summary>
/// Requests a run of the <see cref="DataSynchronizationBackgroundService"/> before its next scheduled one.
/// </summary>
public sealed class DataSynchronizationTrigger
{
    // Requests made while a run is pending are covered by that run
    private readonly Channel<bool> _requests = Channel.CreateBounded<bool>(
        new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite });

    /// <summary>
    /// Requests a synchronization of all data. Data that isn't due for retrieval is still skipped.
    /// </summary>
    public void RequestSynchronization() => _requests.Writer.TryWrite(true);

    /// <summary>
    /// Waits until a synchronization is requested, and consumes the request.
    /// </summary>
    public async Task WaitForRequest(CancellationToken cancellationToken) =>
        await _requests.Reader.ReadAsync(cancellationToken);
}