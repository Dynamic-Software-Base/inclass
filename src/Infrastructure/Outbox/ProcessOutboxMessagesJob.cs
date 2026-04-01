using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedKernel;

namespace Infrastructure.Outbox;

public sealed class ProcessOutboxMessagesJob
{
    private const int BatchSize = 100;

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Objects
    };
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;
    public ProcessOutboxMessagesJob(
        ApplicationDbContext dbContext,
        IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        List<OutboxMessage> messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.Error == null)
            .OrderBy(m => m.OccurredOn)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (!messages.Any())
        {
            return;
        }

        foreach (OutboxMessage message in messages)
        {
            try
            {
                IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    message.Payload,
                    JsonSettings);

                if (domainEvent is null)
                {
                    message.MarkFailed("Deserialization returned null.");
                    continue;
                }
                await _publisher.Publish(domainEvent,cancellationToken);
                message.MarkProcessed();
            }
            catch (Exception e)
            {
                message.MarkFailed(e.ToString());
            }
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
