using MassTransit;

namespace Account.Api.Messaging;

public class MassTransitMessagingClient : IMessagingClient
{
    private readonly ISendEndpointProvider _sender;
    public MassTransitMessagingClient(ISendEndpointProvider sender) => _sender = sender;

    public async Task SendAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default) where T : notnull
    {
        var endpoint = await _sender.GetSendEndpoint(new Uri($"exchange:{exchange}?type=topic"));
        await endpoint.Send(message, ctx => ctx.SetRoutingKey(routingKey), ct);
    }
}
