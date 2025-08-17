using Customers;
using MassTransit;
using Customers.Api.Messaging;
using DotNetEnv;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<IMessagingClient, MassTransitMessagingClient>();

// RabbitMQ config
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GeneralConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, host =>
        {
            host.Username(rabbitUser);
            host.Password(rabbitPass);
        });

        cfg.PrefetchCount = 32;

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.Run();
