using Account.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SofkaFinance.Contracts.Accounts;
using System.Text;
using System.Text.Json;

namespace Account.Api.Messaging;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(IServiceProvider serviceProvider, string connectionString)
    {
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declarar la cola principal para accounts
        _channel.QueueDeclare(queue: "accounts_queue", durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, e) =>
        {
            _ = Task.Run(() => ProcessMessage(sender, e));
        };

        _channel.BasicConsume(queue: "accounts_queue", autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessMessage(object? sender, BasicDeliverEventArgs e)
    {
        var correlationId = e.BasicProperties.CorrelationId;
        var replyTo = e.BasicProperties.ReplyTo;
        var messageType = e.BasicProperties.Type;
        var messageBody = Encoding.UTF8.GetString(e.Body.ToArray());

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            var movementService = scope.ServiceProvider.GetRequiredService<IMovementService>();

            object response = messageType switch
            {
                // Account Commands
                "CreateAccountCommand" => await HandleCreateAccount(JsonSerializer.Deserialize<CreateAccountCommand>(messageBody)!, accountService),
                "CloseAccountCommand" => await HandleCloseAccount(JsonSerializer.Deserialize<CloseAccountCommand>(messageBody)!, accountService),

                // Account Queries
                "GetAllAccountsQuery" => await HandleGetAllAccounts(JsonSerializer.Deserialize<GetAllAccountsQuery>(messageBody)!, accountService),
                "GetAccountByIdQuery" => await HandleGetAccountById(JsonSerializer.Deserialize<GetAccountByIdQuery>(messageBody)!, accountService),
                "GetAccountsByCustomerQuery" => await HandleGetAccountsByCustomer(JsonSerializer.Deserialize<GetAccountsByCustomerQuery>(messageBody)!, accountService),

                // Movement Commands
                "DepositCommand" => await HandleDeposit(JsonSerializer.Deserialize<DepositCommand>(messageBody)!, movementService),
                "WithdrawCommand" => await HandleWithdraw(JsonSerializer.Deserialize<WithdrawCommand>(messageBody)!, movementService),
                "TransferCommand" => await HandleTransfer(JsonSerializer.Deserialize<TransferCommand>(messageBody)!, movementService),

                // Movement Queries
                "GetMovementsByAccountQuery" => await HandleGetMovementsByAccount(JsonSerializer.Deserialize<GetMovementsByAccountQuery>(messageBody)!, movementService),
                "GetMovementsReportQuery" => await HandleGetMovementReport(JsonSerializer.Deserialize<GetMovementsReportQuery>(messageBody)!, movementService),

                _ => throw new ArgumentException($"Tipo de mensaje no soportado: {messageType}")
            };

            // Enviar respuesta
            var responseJson = JsonSerializer.Serialize(response);
            var responseBody = Encoding.UTF8.GetBytes(responseJson);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;

            _channel.BasicPublish("", replyTo, properties, responseBody);
            _channel.BasicAck(e.DeliveryTag, false);
        }
        catch (Exception ex)
        {
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

    // Account Command Handlers
    private async Task<CreateAccountResponse> HandleCreateAccount(CreateAccountCommand command, IAccountService accountService)
    {
        try
        {
            var result = await accountService.CreateAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return new CreateAccountResponse(command.OperationId, ex.Message, null);
        }
    }

    private async Task<CloseAccountResponse> HandleCloseAccount(CloseAccountCommand command, IAccountService accountService)
    {
        try
        {
            var result = await accountService.CloseAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return new CloseAccountResponse(command.OperationId, ex.Message, null);
        }
    }

    // Account Query Handlers
    private async Task<GetAllAccountsResponse> HandleGetAllAccounts(GetAllAccountsQuery query, IAccountService accountService)
    {
        try
        {
            var result = await accountService.GetAllAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            return new GetAllAccountsResponse(query.OperationId, ex.Message, null);
        }
    }

    private async Task<GetAccountResponse> HandleGetAccountById(GetAccountByIdQuery query, IAccountService accountService)
    {
        try
        {
            var result = await accountService.GetByIdAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            return new GetAccountResponse(query.OperationId, ex.Message, null);
        }
    }

    private async Task<GetAccountsByCustomerResponse> HandleGetAccountsByCustomer(GetAccountsByCustomerQuery query, IAccountService accountService)
    {
        try
        {
            var result = await accountService.GetByCustomerAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            return new GetAccountsByCustomerResponse(query.OperationId, ex.Message, null);
        }
    }

    // Movement Command Handlers
    private async Task<DepositResponse> HandleDeposit(DepositCommand command, IMovementService movementService)
    {
        try
        {
            var result = await movementService.DepositAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return new DepositResponse(command.OperationId, ex.Message, null, null);
        }
    }

    private async Task<WithdrawResponse> HandleWithdraw(WithdrawCommand command, IMovementService movementService)
    {
        try
        {
            var result = await movementService.WithdrawAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return new WithdrawResponse(command.OperationId, ex.Message, null, null);
        }
    }

    private async Task<TransferResponse> HandleTransfer(TransferCommand command, IMovementService movementService)
    {
        try
        {
            var result = await movementService.TransferAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return new TransferResponse(command.OperationId, ex.Message, null, null);
        }
    }

    // Movement Query Handlers
    private async Task<GetMovementsByAccountResponse> HandleGetMovementsByAccount(GetMovementsByAccountQuery query, IMovementService movementService)
    {
        try
        {
            var result = await movementService.GetByAccountAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            return new GetMovementsByAccountResponse(query.OperationId, ex.Message, null);
        }
    }

    private async Task<GetMovementsReportResponse> HandleGetMovementReport(GetMovementsReportQuery query, IMovementService movementService)
    {
        try
        {
            var result = await movementService.GetReportAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            return new GetMovementsReportResponse(query.OperationId, ex.Message, null, null);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
