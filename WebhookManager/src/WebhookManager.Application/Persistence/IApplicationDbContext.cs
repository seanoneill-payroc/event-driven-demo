using Microsoft.EntityFrameworkCore;
using WebhookManager.Domain.Entities;

namespace WebhookManager.Application.Persistence;

public interface IApplicationDbContext
{
    public DbSet<WebhookSubscriptionEntity> WebhookSubscriptions { get; }
}
