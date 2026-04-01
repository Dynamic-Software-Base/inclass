using Infrastructure.Database;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel;

namespace Infrastructure.Outbox;

public class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is ApplicationDbContext dbContext)
        {
            var aggregates = dbContext.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IHasDomainEvents)
                .Select(e => (IHasDomainEvents)e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var outboxMessages = aggregates
                .SelectMany(a => a.DomainEvents)
                .Select(OutboxMessage.Create)
                .ToList();

            dbContext.OutboxMessages.AddRange(outboxMessages);

            foreach (IHasDomainEvents aggregate in aggregates)
            {
                aggregate.ClearDomainEvents();
            }
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
