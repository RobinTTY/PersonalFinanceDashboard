namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <summary>
/// Lets only one synchronization with a third party run at a time. Concurrent requests would otherwise retrieve
/// the same data and both try to insert it.
/// </summary>
/// <remarks>The lock isn't reentrant, so a sync handler must not call another one while holding it.</remarks>
public sealed class DataSynchronizationLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Waits until no other synchronization is running.
    /// </summary>
    /// <returns>A handle that releases the lock when disposed.</returns>
    public async Task<IDisposable> Acquire()
    {
        await _semaphore.WaitAsync();
        return new Releaser(_semaphore);
    }

    private sealed class Releaser(SemaphoreSlim semaphore) : IDisposable
    {
        private int _released;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _released, 1) == 0)
                semaphore.Release();
        }
    }
}