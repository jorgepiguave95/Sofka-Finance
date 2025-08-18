using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SofkaFinance.Contracts.Customers;
using Customers.Application.Interfaces;

namespace Customers.Api.Messaging;

public class AuthRabbitMQConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public AuthRabbitMQConsumer(IServiceProvider serviceProvider, string connectionString)
    {
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declarar la cola para autenticación (usamos la misma cola que customers por ahora)
        _channel.QueueDeclare(queue: "customers_queue", durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        Console.WriteLine("[RabbitMQ Auth] Consumer inicializado");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, e) =>
        {
            // Solo procesar mensajes de autenticación
            var messageType = e.BasicProperties.Type;
            if (IsAuthMessage(messageType))
            {
                _ = Task.Run(() => ProcessAuthMessage(sender, e));
            }
            else
            {
                // Si no es un mensaje de auth, hacer Nack para que otro consumer lo procese
                _channel.BasicNack(e.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: "customers_queue", autoAck: false, consumer: consumer);

        Console.WriteLine("[RabbitMQ Auth] Esperando mensajes de autenticación...");

        // Mantener el servicio corriendo
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private bool IsAuthMessage(string messageType)
    {
        return messageType switch
        {
            "LoginCommand" => true,
            _ => false
        };
    }

    private async Task ProcessAuthMessage(object? sender, BasicDeliverEventArgs e)
    {
        var correlationId = e.BasicProperties.CorrelationId;
        var replyTo = e.BasicProperties.ReplyTo;
        var messageType = e.BasicProperties.Type;
        var messageBody = Encoding.UTF8.GetString(e.Body.ToArray());

        Console.WriteLine($"[RabbitMQ Auth] Mensaje recibido: {messageType}, CorrelationId: {correlationId}");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            object response = messageType switch
            {
                "LoginCommand" => await HandleLogin(JsonSerializer.Deserialize<LoginCommand>(messageBody)!, authService),
                _ => throw new ArgumentException($"Tipo de mensaje de auth no soportado: {messageType}")
            };

            // Enviar respuesta
            var responseJson = JsonSerializer.Serialize(response);
            var responseBody = Encoding.UTF8.GetBytes(responseJson);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;

            _channel.BasicPublish("", replyTo, properties, responseBody);
            _channel.BasicAck(e.DeliveryTag, false);

            Console.WriteLine($"[RabbitMQ Auth] Respuesta enviada para {messageType}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RabbitMQ Auth] Error procesando mensaje: {ex.Message}");

            // Enviar respuesta de error
            var errorResponse = new { Success = false, ErrorMessage = ex.Message };
            var responseJson = JsonSerializer.Serialize(errorResponse);
            var responseBody = Encoding.UTF8.GetBytes(responseJson);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;

            _channel.BasicPublish("", replyTo, properties, responseBody);
            _channel.BasicAck(e.DeliveryTag, false);
        }
    }

    private async Task<LoginResponse> HandleLogin(LoginCommand command, IAuthService authService)
    {
        try
        {
            var result = await authService.LoginAsync(command);
            return result; // Retornar la respuesta original del servicio
        }
        catch (Exception ex)
        {
            return new LoginResponse(command.OperationId, ex.Message, null, null);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
