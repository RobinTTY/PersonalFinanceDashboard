using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class
    ThirdPartyDataRetrievalMetadataTypeConfiguration : IEntityTypeConfiguration<ThirdPartyDataRetrievalMetadata>
{
    public void Configure(EntityTypeBuilder<ThirdPartyDataRetrievalMetadata> builder)
    {
        builder.HasData(
            new ThirdPartyDataRetrievalMetadata
            {
                Id = new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"),
                DataType = ThirdPartyDataType.BankingInstitutions,
                DataSource = ThirdPartyDataSource.GoCardless,
                LastRetrievalTime = DateTime.MinValue,
                RetrievalInterval = TimeSpan.FromDays(7)
            },
            new ThirdPartyDataRetrievalMetadata
            {
                Id = new Guid("0b53ec13-5a8c-4910-bfff-1ba06c3f3859"),
                DataType = ThirdPartyDataType.AuthenticationRequests,
                DataSource = ThirdPartyDataSource.GoCardless,
                LastRetrievalTime = DateTime.MinValue,
                RetrievalInterval = TimeSpan.FromDays(7)
            }
        );
    }
}