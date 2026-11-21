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
            await Task.WhenAny(nextRun, Task.Delay(10));
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

    private sealed class RecordingSynchronizer : IDataSynchronizer
    {
        private readonly Channel<bool> _runs = Channel.CreateUnbounded<bool>();

        public bool ThrowOnNextRun { get; set; }

        public async Task WaitForRun() => await _runs.Reader.ReadAsync().AsTask().WaitAsync(Timeout);

        public Task<bool> SynchronizeAll(bool forceThirdPartySync = false)
        {
            _runs.Writer.TryWrite(true);
            if (!ThrowOnNextRun)
                return Task.FromResult(true);

            ThrowOnNextRun = false;
            throw new InvalidOperationException("Synchronization failed");
        }

        public Task<bool> Synchronize(ThirdPartyDataType dataType, bool forceThirdPartySync = false) =>
            throw new NotSupportedException();

        public Task<bool> RefreshAuthenticationRequest(Guid authenticationRequestId) =>
            throw new NotSupportedException();
    }
}