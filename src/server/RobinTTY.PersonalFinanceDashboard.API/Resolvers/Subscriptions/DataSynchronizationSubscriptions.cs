using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Api.Models;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Subscriptions;

/// <summary>
/// Subscriptions to the synchronization of third-party data.
/// </summary>
[SubscriptionType]
public static class DataSynchronizationSubscriptions
{
    internal const string DataSynchronizedTopic = nameof(DataSynchronized);

    /// <summary>
    /// Is sent whenever a synchronization retrieved data from a data provider, successfully or not, so the data and
    /// its sync status can be refetched.
    /// </summary>
    [Subscribe]
    [Topic(DataSynchronizedTopic)]
    public static DataSynchronization DataSynchronized([EventMessage] DataSynchronization synchronization) =>
        synchronization;
}