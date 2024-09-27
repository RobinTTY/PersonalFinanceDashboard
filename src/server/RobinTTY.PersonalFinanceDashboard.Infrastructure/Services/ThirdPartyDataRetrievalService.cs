using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

public class ThirdPartyDataRetrievalService
{
    private readonly ThirdPartyDataRetrievalMetadataRepository _thirdPartyDataRetrievalMetadataRepository;
    private readonly BankingInstitutionRepository _bankingInstitutionRepository;
    private readonly GoCardlessDataProvider _goCardlessDataProvider;

    public ThirdPartyDataRetrievalService(
        ThirdPartyDataRetrievalMetadataRepository thirdPartyDataRetrievalMetadataRepository,
        BankingInstitutionRepository bankingInstitutionRepository,
        GoCardlessDataProvider goCardlessDataProvider)
    {
        _thirdPartyDataRetrievalMetadataRepository = thirdPartyDataRetrievalMetadataRepository;
        _bankingInstitutionRepository = bankingInstitutionRepository;
        _goCardlessDataProvider = goCardlessDataProvider;
    }

    /// <summary>
    /// TODO: Can this be somehow generalized for every third party data type?
    /// Otherwise we need a third party data retrieval service for every third party data type
    ///    - There would need to be different strategies of data retrieval
    ///      - Clear table and reseed
    ///      - Upsert entries
    ///    - Method would need information how to get data from third party data source
    ///      - Maybe this can be handed over as a Func
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(string? countryCode)
    {
        var retrievalMetadata = await _thirdPartyDataRetrievalMetadataRepository
            .GetThirdPartyDataRetrievalMetadata(ThirdPartyDataType.BankingInstitutions);
        var nextRetrievalTime = retrievalMetadata.LastRetrievalTime + retrievalMetadata.RetrievalInterval;

        if (nextRetrievalTime < DateTime.Now)
        {
            var response = await _goCardlessDataProvider.GetBankingInstitutions();
            if (response.IsSuccessful)
            {
                await _bankingInstitutionRepository.DeleteBankingInstitutions();
                await _bankingInstitutionRepository.AddBankingInstitutions(response.Result);

                // Update metadata
                retrievalMetadata.LastRetrievalTime = DateTime.Now;
                await _thirdPartyDataRetrievalMetadataRepository
                    .UpdateThirdPartyDataRetrievalMetadata(retrievalMetadata);
                
                return response.Result;
            }
            else
            {
                // TODO: What to do in case of failure should depend on if we already have data
                throw new NotImplementedException();
            }
        }
        else
        {
            return await _bankingInstitutionRepository.GetBankingInstitutions(countryCode);
        }
    }
}
