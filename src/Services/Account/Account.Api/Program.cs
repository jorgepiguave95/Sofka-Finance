using Account;
using MassTransit;
using Account.Api.Messaging;
using Account.Api.Controllers;
using DotNetEnv;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<AccountsController>();
builder.Services.AddScoped<MovementsController>();


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
        cfg.Host(rabbitHost, h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.PrefetchCount = 32;

        // Exchange base del dominio
        const string exchange = "finance.accounts";

        // Una sola cola que maneja todos los routing keys
        cfg.ReceiveEndpoint("accounts.general", e =>
        {
            // Habilitar topology automática para Request-Response
            e.ConfigureConsumeTopology = true;

            // Configurar el consumer
            e.ConfigureConsumer<GeneralConsumer>(context);
        });
    });
});

var app = builder.Build();

app.Run();
