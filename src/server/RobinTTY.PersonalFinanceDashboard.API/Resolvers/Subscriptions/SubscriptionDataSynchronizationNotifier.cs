using HotChocolate.Subscriptions;
using RobinTTY.PersonalFinanceDashboard.Api.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Subscriptions;

/// <summary>
/// Notifies the subscribers of <see cref="DataSynchronizationSubscriptions.DataSynchronized"/>.
/// </summary>
public sealed class SubscriptionDataSynchronizationNotifier(ITopicEventSender sender, TimeProvider timeProvider)
    : IDataSynchronizationNotifier
{
    /// <inheritdoc />
    public async Task NotifyDataSynchronized() =>
        await sender.SendAsync(DataSynchronizationSubscriptions.DataSynchronizedTopic,
            new DataSynchronization { CompletedAt = timeProvider.GetUtcNow().UtcDateTime });
}