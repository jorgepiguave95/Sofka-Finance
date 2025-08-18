using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SofkaFinance.Contracts.Customers;
using Customers.Application.Interfaces;

namespace Customers.Api.Messaging;

public class CustomerRabbitMQConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public CustomerRabbitMQConsumer(IServiceProvider serviceProvider, string connectionString)
    {
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declarar la cola para customers
        _channel.QueueDeclare(queue: "customers_queue", durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        Console.WriteLine("[RabbitMQ Customer] Consumer inicializado");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, e) =>
        {
            // Solo procesar mensajes de customer
            var messageType = e.BasicProperties.Type;
            if (IsCustomerMessage(messageType))
            {
                _ = Task.Run(() => ProcessCustomerMessage(sender, e));
            }
            else
            {
                // Si no es un mensaje de customer, hacer Nack para que otro consumer lo procese
                _channel.BasicNack(e.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: "customers_queue", autoAck: false, consumer: consumer);

        Console.WriteLine("[RabbitMQ Customer] Esperando mensajes de clientes...");

        // Mantener el servicio corriendo
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private bool IsCustomerMessage(string messageType)
    {
        return messageType switch
        {
            "CreateCustomerCommand" => true,
            "GetAllCustomersQuery" => true,
            "GetCustomerByIdQuery" => true,
            "UpdateCustomerCommand" => true,
            "DeleteCustomerCommand" => true,
            _ => false
        };
    }

    private async Task ProcessCustomerMessage(object? sender, BasicDeliverEventArgs e)
    {
        var correlationId = e.BasicProperties.CorrelationId;
        var replyTo = e.BasicProperties.ReplyTo;
        var messageType = e.BasicProperties.Type;
        var messageBody = Encoding.UTF8.GetString(e.Body.ToArray());

        Console.WriteLine($"[RabbitMQ Customer] Mensaje recibido: {messageType}, CorrelationId: {correlationId}");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            object response = messageType switch
            {
                "CreateCustomerCommand" => await HandleCreateCustomer(JsonSerializer.Deserialize<CreateCustomerCommand>(messageBody)!, customerService),
                "GetAllCustomersQuery" => await HandleGetAllCustomers(JsonSerializer.Deserialize<GetAllCustomersQuery>(messageBody)!, customerService),
                "GetCustomerByIdQuery" => await HandleGetCustomerById(JsonSerializer.Deserialize<GetCustomerByIdQuery>(messageBody)!, customerService),
                "UpdateCustomerCommand" => await HandleUpdateCustomer(JsonSerializer.Deserialize<UpdateCustomerCommand>(messageBody)!, customerService),
                "DeleteCustomerCommand" => await HandleDeleteCustomer(JsonSerializer.Deserialize<DeleteCustomerCommand>(messageBody)!, customerService),
                _ => throw new ArgumentException($"Tipo de mensaje de customer no soportado: {messageType}")
            };

            // Enviar respuesta
            var responseJson = JsonSerializer.Serialize(response);
            var responseBody = Encoding.UTF8.GetBytes(responseJson);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;

            _channel.BasicPublish("", replyTo, properties, responseBody);
            _channel.BasicAck(e.DeliveryTag, false);

            Console.WriteLine($"[RabbitMQ Customer] Respuesta enviada para {messageType}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RabbitMQ Customer] Error procesando mensaje: {ex.Message}");

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

    private async Task<CreateCustomerResponse> HandleCreateCustomer(CreateCustomerCommand command, ICustomerService customerService)
    {
        try
        {
            var result = await customerService.CreateAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return new CreateCustomerResponse(command.OperationId, ex.Message, null);
        }
    }

    private async Task<GetAllCustomersResponse> HandleGetAllCustomers(GetAllCustomersQuery query, ICustomerService customerService)
    {
        try
        {
            var result = await customerService.GetAllAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            return new GetAllCustomersResponse(query.OperationId, ex.Message, null);
        }
    }

    private async Task<GetCustomerResponse> HandleGetCustomerById(GetCustomerByIdQuery query, ICustomerService customerService)
    {
        try
        {
            var result = await customerService.GetByIdAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            return new GetCustomerResponse(query.OperationId, ex.Message, null);
        }
    }

    private async Task<UpdateCustomerResponse> HandleUpdateCustomer(UpdateCustomerCommand command, ICustomerService customerService)
    {
        try
        {
            var result = await customerService.UpdateAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return new UpdateCustomerResponse(command.OperationId, ex.Message, null);
        }
    }

    private async Task<DeleteCustomerResponse> HandleDeleteCustomer(DeleteCustomerCommand command, ICustomerService customerService)
    {
        try
        {
            await customerService.DeleteAsync(command);
            return new DeleteCustomerResponse(command.OperationId, "Cliente eliminado exitosamente", command.CustomerId);
        }
        catch (Exception ex)
        {
            return new DeleteCustomerResponse(command.OperationId, ex.Message, null);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
