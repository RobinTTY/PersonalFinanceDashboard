using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.Configurations;

public class
    ThirdPartyDataRetrievalMetadataTypeConfiguration : IEntityTypeConfiguration<ThirdPartyDataRetrievalMetadataEntity>
{
    public void Configure(EntityTypeBuilder<ThirdPartyDataRetrievalMetadataEntity> builder)
    {
        builder.HasData(new ThirdPartyDataRetrievalMetadataEntity(Guid.NewGuid(),
            ThirdPartyDataType.BankingInstitutions, ThirdPartyDataSource.GoCardless, DateTime.MinValue,
            TimeSpan.FromDays(7)));
    }
}
