using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Messaging;

public class RabbitMQMessagingClient : IMessagingClient, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Dictionary<string, TaskCompletionSource<string>> _pendingRequests;
    private readonly string _replyQueueName;

    public RabbitMQMessagingClient(string connectionString)
    {
        _pendingRequests = new Dictionary<string, TaskCompletionSource<string>>();

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Crear cola de respuesta temporal
        _replyQueueName = _channel.QueueDeclare().QueueName;

        // Configurar consumer para respuestas
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnResponseReceived;
        _channel.BasicConsume(_replyQueueName, autoAck: true, consumer: consumer);
    }

    private void OnResponseReceived(object? sender, BasicDeliverEventArgs e)
    {
        var correlationId = e.BasicProperties.CorrelationId;
        var responseBody = Encoding.UTF8.GetString(e.Body.ToArray());

        if (_pendingRequests.TryGetValue(correlationId, out var tcs))
        {
            _pendingRequests.Remove(correlationId);
            tcs.SetResult(responseBody);
        }
    }

    public async Task SendAsync<T>(string exchange, string routingKey, T message, CancellationToken ct = default) where T : notnull
    {
        var messageBody = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageBody);

        _channel.BasicPublish(exchange, routingKey, null, body);
        await Task.CompletedTask;
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request, CancellationToken ct = default)
        where TRequest : class
        where TResponse : class
    {
        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();

        _pendingRequests[correlationId] = tcs;

        // Determinar la cola de destino basada en el tipo de request
        var targetQueue = GetTargetQueueName<TRequest>();

        try
        {
            var messageBody = JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(messageBody);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = _replyQueueName;
            properties.Type = typeof(TRequest).Name;

            _channel.BasicPublish("", targetQueue, properties, body);

            // Esperar respuesta con timeout
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(30));

            var responseJson = await tcs.Task.WaitAsync(cts.Token);

            var response = JsonSerializer.Deserialize<TResponse>(responseJson);
            return response!;
        }
        catch (OperationCanceledException)
        {
            _pendingRequests.Remove(correlationId);
            throw new OperationCanceledException("El servicio no respondió a tiempo");
        }
        catch
        {
            _pendingRequests.Remove(correlationId);
            throw;
        }
    }

    private string GetTargetQueueName<TRequest>()
    {
        var requestType = typeof(TRequest).Name;

        return requestType switch
        {
            // Comandos específicos 
            "LoginCommand" => "customers_queue",
            "DepositCommand" => "accounts_queue",
            "WithdrawCommand" => "accounts_queue",
            "TransferCommand" => "accounts_queue",
            "GetMovementsReportQuery" => "accounts_queue",
            "GetMovementsByAccountQuery" => "accounts_queue",
            "GetAccountsByCustomerQuery" => "accounts_queue",

            // Reglas generales 
            var name when name.Contains("Customer") => "customers_queue",
            var name when name.Contains("Account") => "accounts_queue",
            var name when name.Contains("Movement") => "accounts_queue",
            var name when name.Contains("Auth") => "customers_queue",

            _ => throw new ArgumentException($"No se encontró cola para el tipo de request: {requestType}")
        };
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
