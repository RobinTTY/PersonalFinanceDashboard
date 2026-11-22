using System.Diagnostics.CodeAnalysis;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Core.DataProviders;

/// <summary>
/// The registered <see cref="IBankDataProvider"/>s, keyed by the data source they provide.
/// </summary>
public sealed class BankDataProviderRegistry
{
    private readonly Dictionary<DataSource, IBankDataProvider> _providers = [];

    /// <summary>
    /// The registered providers.
    /// </summary>
    public IReadOnlyCollection<IBankDataProvider> All => _providers.Values;

    /// <summary>
    /// Creates a new instance of <see cref="BankDataProviderRegistry"/>.
    /// </summary>
    /// <param name="providers">The providers to register, at most one per data source.</param>
    /// <exception cref="ArgumentException">Thrown if a provider is registered for manual data or two providers
    /// are registered for the same data source.</exception>
    public BankDataProviderRegistry(IEnumerable<IBankDataProvider> providers)
    {
        foreach (var provider in providers)
        {
            if (provider.Source == DataSource.Manual)
                throw new ArgumentException("Manual data has no provider.", nameof(providers));
            if (!_providers.TryAdd(provider.Source, provider))
                throw new ArgumentException($"More than one provider is registered for {provider.Source}.",
                    nameof(providers));
        }
    }

    /// <summary>
    /// Gets the provider of the given data source.
    /// </summary>
    /// <param name="source">The data source to get the provider for.</param>
    /// <param name="provider">The provider, or <see langword="null"/> if the source has none (e.g. manual data).</param>
    /// <returns>Whether a provider is registered for the data source.</returns>
    public bool TryGet(DataSource source, [NotNullWhen(true)] out IBankDataProvider? provider) =>
        _providers.TryGetValue(source, out provider);
}
