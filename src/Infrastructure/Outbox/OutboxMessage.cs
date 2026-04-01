using Newtonsoft.Json;
using SharedKernel;

namespace Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }
    public string? Error { get; private set; }

    private OutboxMessage(){}

    private OutboxMessage(Guid id, string type, string payload, DateTime occurredOn)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredOn = occurredOn;
        ProcessedOn = null;
        Error = null;
    }
    public static OutboxMessage Create(IDomainEvent domainEvent)
    {
        string type = domainEvent.GetType().AssemblyQualifiedName!;
        string payload = JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        });

        return new OutboxMessage(Guid.NewGuid(), type, payload, domainEvent.OccurredOn);
    }
    public void MarkProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
    }
    public void MarkFailed(string error)
    {
        Error = error;
    }
}
