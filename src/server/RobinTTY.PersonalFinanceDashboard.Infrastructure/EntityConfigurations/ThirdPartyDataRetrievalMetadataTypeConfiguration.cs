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
            new ThirdPartyDataRetrievalMetadata(new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"),
                ThirdPartyDataType.BankingInstitutions, ThirdPartyDataSource.GoCardless, DateTime.MinValue,
                TimeSpan.FromDays(7)),
            new ThirdPartyDataRetrievalMetadata(new Guid("0b53ec13-5a8c-4910-bfff-1ba06c3f3859"),
                ThirdPartyDataType.AuthenticationRequests, ThirdPartyDataSource.GoCardless, DateTime.MinValue,
                TimeSpan.FromDays(7)),
            new ThirdPartyDataRetrievalMetadata(new Guid("6a7aa33d-1a81-46fe-80b2-d449ba851861"),
                ThirdPartyDataType.BankAccounts, ThirdPartyDataSource.GoCardless, DateTime.MinValue,
                TimeSpan.FromDays(7)),
            new ThirdPartyDataRetrievalMetadata(new Guid("80fbbf1a-c484-4369-aef9-6f24c3b02fa8"),
                ThirdPartyDataType.Transactions, ThirdPartyDataSource.GoCardless, DateTime.MinValue,
                TimeSpan.FromHours(12))
        );
    }
}