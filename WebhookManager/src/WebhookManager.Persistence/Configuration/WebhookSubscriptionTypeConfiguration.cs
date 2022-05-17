using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookManager.Domain.Entities;

namespace WebhookManager.Persistence.Configuration;

public class WebhookSubscriptionTypeConfiguration : IEntityTypeConfiguration<WebhookSubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<WebhookSubscriptionEntity> builder)
    {
        throw new NotImplementedException();
    }
}

public class WebhookDeliveryAttemptTypeConfiguration : IEntityTypeConfiguration<WebhookDeliveryAttemptEntity>
{
    public void Configure(EntityTypeBuilder<WebhookDeliveryAttemptEntity> builder)
    {
        throw new NotImplementedException();
    }
}
