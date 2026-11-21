using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <summary>
/// Synchronizes all third-party data on startup, every <see cref="Interval"/> and whenever the
/// <see cref="DataSynchronizationTrigger"/> requests it.
/// </summary>
public sealed class DataSynchronizationBackgroundService(
    IServiceScopeFactory scopeFactory,
    DataSynchronizationTrigger trigger,
    TimeProvider timeProvider,
    ILogger<DataSynchronizationBackgroundService> logger) : BackgroundService
{
    /// <summary>
    /// How often the data is checked. How often it's retrieved is set per data type by its retrieval metadata.
    /// </summary>
    public static readonly TimeSpan Interval = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SynchronizeAll(stoppingToken);
            await WaitForNextRun(stoppingToken);
        }
    }

    private async Task SynchronizeAll(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var synchronizer = scope.ServiceProvider.GetRequiredService<IDataSynchronizer>();
            if (!await synchronizer.SynchronizeAll(cancellationToken: stoppingToken))
                logger.LogWarning("Not all third-party data could be synchronized.");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Stopped synchronizing third-party data because the app is shutting down.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Synchronizing third-party data failed.");
        }
    }

    private async Task WaitForNextRun(CancellationToken stoppingToken)
    {
        using var waitCancellation = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var interval = Task.Delay(Interval, timeProvider, waitCancellation.Token);
        var request = trigger.WaitForRequest(waitCancellation.Token);

        await Task.WhenAny(interval, request);
        await waitCancellation.CancelAsync();
    }
}