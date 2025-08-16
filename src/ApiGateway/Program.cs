using Microsoft.OpenApi.Models;
using MassTransit;
using ApiGateway.Messaging;
using DotNetEnv;
using SofkaFinance.Contracts.Accounts;
using SofkaFinance.Contracts.Customers;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RabbitMQ config 
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

builder.Services.AddMassTransit(bus =>
{
    bus.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, host =>
        {
            host.Username(rabbitUser);
            host.Password(rabbitPass);
        });

        cfg.UseTimeout(x => x.Timeout = TimeSpan.FromSeconds(30));
    });

    // Registrar request accounts
    bus.AddRequestClient<CreateAccountCommand>();
    bus.AddRequestClient<GetAccountByIdQuery>();
    bus.AddRequestClient<GetAllAccountsQuery>();
    bus.AddRequestClient<GetAccountsByCustomerQuery>();
    bus.AddRequestClient<CloseAccountCommand>();
    bus.AddRequestClient<DepositCommand>();
    bus.AddRequestClient<WithdrawCommand>();
    bus.AddRequestClient<TransferCommand>();
    bus.AddRequestClient<GetMovementsByAccountQuery>();
    bus.AddRequestClient<GetMovementsReportQuery>();

    // Customer request clients
    bus.AddRequestClient<CreateCustomerCommand>();
    bus.AddRequestClient<UpdateCustomerCommand>();
    bus.AddRequestClient<DeleteCustomerCommand>();
    bus.AddRequestClient<GetCustomerByIdQuery>();
    bus.AddRequestClient<GetAllCustomersQuery>();
    bus.AddRequestClient<LoginCommand>();
});

builder.Services.AddScoped<IMessagingClient, MassTransitMessagingClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SofkaFinance Gateway v1");
        options.RoutePrefix = string.Empty;
    });
}

if (!app.Environment.IsDevelopment())
{
    //Se comenta para no forzar HTTPS en producción
    //app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
