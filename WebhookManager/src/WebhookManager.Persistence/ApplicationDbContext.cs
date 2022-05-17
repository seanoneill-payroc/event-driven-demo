using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebhookManager.Application.Persistence;
using WebhookManager.Domain.Entities;

namespace WebhookManager.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<WebhookSubscriptionEntity> WebhookSubscriptions => Set<WebhookSubscriptionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
