using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public sealed class DataSynchronizationBackgroundServiceTests : IAsyncDisposable
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    private readonly FakeTimeProvider _time = new();
    private readonly DataSynchronizationTrigger _trigger = new();
    private readonly RecordingSynchronizer _synchronizer = new();
    private readonly ServiceProvider _services;
    private readonly DataSynchronizationBackgroundService _service;

    public DataSynchronizationBackgroundServiceTests()
    {
        _services = new ServiceCollection().AddScoped<IDataSynchronizer>(_ => _synchronizer).BuildServiceProvider();
        _service = new DataSynchronizationBackgroundService(_services.GetRequiredService<IServiceScopeFactory>(),
            _trigger, _time, NullLogger<DataSynchronizationBackgroundService>.Instance);
    }

    public async ValueTask DisposeAsync()
    {
        await _service.StopAsync(CancellationToken.None);
        _service.Dispose();
        await _services.DisposeAsync();
    }

    [Fact]
    public async Task SynchronizesOnStartup()
    {
        await _service.StartAsync(CancellationToken.None);

        await _synchronizer.WaitForRun();
    }

    [Fact]
    public async Task SynchronizesAgainAfterTheInterval()
    {
        await _service.StartAsync(CancellationToken.None);
        await _synchronizer.WaitForRun();

        // Advancing before the service waits again would skip the interval, so it's advanced until a run happens
        var nextRun = _synchronizer.WaitForRun();
        var deadline = DateTime.UtcNow + Timeout;
        while (!nextRun.IsCompleted && DateTime.UtcNow < deadline)
        {
            _time.Advance(DataSynchronizationBackgroundService.Interval);
            try
            {
                await nextRun.WaitAsync(TimeSpan.FromMilliseconds(10));
            }
            catch (TimeoutException)
            {
                // Not run yet, so the time is advanced again
            }
        }

        await nextRun;
    }

    [Fact]
    public async Task SynchronizesRightAwayWhenRequested()
    {
        await _service.StartAsync(CancellationToken.None);
        await _synchronizer.WaitForRun();

        _trigger.RequestSynchronization();

        await _synchronizer.WaitForRun();
    }

    [Fact]
    public async Task FailedSynchronization_DoesNotStopLaterOnes()
    {
        _synchronizer.ThrowOnNextRun = true;
        await _service.StartAsync(CancellationToken.None);
        await _synchronizer.WaitForRun();

        _trigger.RequestSynchronization();

        await _synchronizer.WaitForRun();
    }

    [Fact]
    public async Task Stopping_CancelsTheRunningSynchronization()
    {
        _synchronizer.RunUntilCancelled = true;
        await _service.StartAsync(CancellationToken.None);
        await _synchronizer.WaitForRun();

        await _service.StopAsync(CancellationToken.None).WaitAsync(Timeout);

        Assert.True(_synchronizer.WasCancelled);
    }

    private sealed class RecordingSynchronizer : IDataSynchronizer
    {
        private readonly Channel<bool> _runs = Channel.CreateUnbounded<bool>();

        public bool ThrowOnNextRun { get; set; }
        public bool RunUntilCancelled { get; set; }
        public bool WasCancelled { get; private set; }

        public async Task WaitForRun() => await _runs.Reader.ReadAsync().AsTask().WaitAsync(Timeout);

        public async Task<bool> SynchronizeAll(bool forceThirdPartySync = false,
            CancellationToken cancellationToken = default)
        {
            _runs.Writer.TryWrite(true);
            if (RunUntilCancelled)
            {
                cancellationToken.Register(() => WasCancelled = true);
                await Task.Delay(System.Threading.Timeout.InfiniteTimeSpan, cancellationToken);
            }

            if (!ThrowOnNextRun)
                return true;

            ThrowOnNextRun = false;
            throw new InvalidOperationException("Synchronization failed");
        }

        public Task<bool> Synchronize(ThirdPartyDataType dataType, bool forceThirdPartySync = false,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<bool> RefreshAuthenticationRequest(Guid authenticationRequestId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
