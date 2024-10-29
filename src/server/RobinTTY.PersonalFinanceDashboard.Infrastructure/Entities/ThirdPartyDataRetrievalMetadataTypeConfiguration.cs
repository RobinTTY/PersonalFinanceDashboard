using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.Configurations;

public class
    ThirdPartyDataRetrievalMetadataTypeConfiguration : IEntityTypeConfiguration<ThirdPartyDataRetrievalMetadata>
{
    public void Configure(EntityTypeBuilder<ThirdPartyDataRetrievalMetadata> builder)
    {
        builder.HasData(new ThirdPartyDataRetrievalMetadata(new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"),
            ThirdPartyDataType.BankingInstitutions, ThirdPartyDataSource.GoCardless, DateTime.MinValue,
            TimeSpan.FromDays(7)));
    }
}
