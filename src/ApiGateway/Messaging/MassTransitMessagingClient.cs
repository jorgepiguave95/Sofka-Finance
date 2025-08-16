using MassTransit;

namespace ApiGateway.Messaging;

public class MassTransitMessagingClient : IMessagingClient
{
    private readonly ISendEndpointProvider _sender;
    private readonly IBus _bus;

    public MassTransitMessagingClient(ISendEndpointProvider sender, IBus bus)
    {
        _sender = sender;
        _bus = bus;
    }

    public async Task SendAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default) where T : notnull
    {
        var endpoint = await _sender.GetSendEndpoint(new Uri($"exchange:{exchange}?type=topic"));
        await endpoint.Send(message, ctx => ctx.SetRoutingKey(routingKey), ct);
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken ct = default)
        where TRequest : class
        where TResponse : class
    {
        var client = _bus.CreateRequestClient<TRequest>();
        var response = await client.GetResponse<TResponse>(request, ct);
        return response.Message;
    }
}
