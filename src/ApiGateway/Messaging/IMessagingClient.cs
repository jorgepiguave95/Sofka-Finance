namespace ApiGateway.Messaging;

public interface IMessagingClient
{
    // Fire-and-forget
    Task SendAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default) where T : notnull;

    // Request-Response
    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken ct = default)
        where TRequest : class
        where TResponse : class;
}
