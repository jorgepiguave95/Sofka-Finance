namespace Account.Api.Messaging;

public interface IMessagingClient
{
    Task SendAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default) where T : notnull;
}
