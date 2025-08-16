using Customers;
using MassTransit;
using Customers.Api.Messaging;
using Customers.Api.Controllers;
using DotNetEnv;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<CustomersController>();
builder.Services.AddScoped<AuthController>();

builder.Services.AddScoped<IMessagingClient, MassTransitMessagingClient>();

// RabbitMQ config
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

builder.Services.AddMassTransit(x =>
{
    // Solo registrar el consumer general
    x.AddConsumer<GeneralConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, host =>
        {
            host.Username(rabbitUser);
            host.Password(rabbitPass);
        });

        cfg.PrefetchCount = 32;

        // Exchange base del dominio
        const string exchange = "finance.customers";

        // Una sola cola que maneja todos los routing keys
        cfg.ReceiveEndpoint("customers.general", e =>
        {
            e.ConfigureConsumeTopology = false;

            // Bind todos los routing keys al mismo consumer
            e.Bind(exchange, s => { s.RoutingKey = "customer.create"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "customer.update"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "customer.delete"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "customer.activate"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "customer.deactivate"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "customer.getById"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "customer.getAll"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "customer.search"; s.ExchangeType = "topic"; });
            e.Bind(exchange, s => { s.RoutingKey = "auth.login"; s.ExchangeType = "topic"; });

            e.Bind(exchange, s => { s.RoutingKey = "customer.accountCreated"; s.ExchangeType = "topic"; });

            e.ConfigureConsumer<GeneralConsumer>(context);
        });
    });
});

var app = builder.Build();

app.Run();
