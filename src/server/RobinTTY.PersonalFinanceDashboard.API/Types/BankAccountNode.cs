using GreenDonut;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types;

/// <summary>
/// Fields of <see cref="BankAccount"/> that aren't stored on the entity.
/// </summary>
[ObjectType<BankAccount>]
public static partial class BankAccountNode
{
    static partial void Configure(IObjectTypeDescriptor<BankAccount> descriptor)
    {
        // The sync status is looked up by the account id, so it's loaded even if the query doesn't select it
        descriptor.Field(account => account.Id).IsProjected();
    }

    /// <summary>
    /// How up to date the account is with the data source it originates from.
    /// </summary>
    public static async Task<SyncStatus> GetSyncStatus([Parent] BankAccount account,
        ISyncStatusByAccountIdDataLoader dataLoader, CancellationToken cancellationToken)
    {
        return await dataLoader.LoadRequiredAsync(account.Id!.Value, cancellationToken);
    }

    [DataLoader]
    internal static async Task<IReadOnlyDictionary<Guid, SyncStatus>> GetSyncStatusByAccountId(
        IReadOnlyList<Guid> accountIds, BankAccountSyncStatusService syncStatusService,
        CancellationToken cancellationToken)
    {
        return await syncStatusService.GetSyncStatuses(accountIds, cancellationToken);
    }
}