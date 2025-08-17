using MassTransit;
using Customers.Api.Messaging;
using Customers.Application.Interfaces;
using Customers.Application.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Customers.Infrastructure.Persistence;
using Customers.Infrastructure.Repositories;
using Customers.Application.Repositories;

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

// SQL Server Config
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var dbName = Environment.GetEnvironmentVariable("DB_NAME_CUSTOMERS") ?? "SofkaCustomers";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
    ?? throw new InvalidOperationException("DB_PASSWORD is required");

var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=true;";

builder.Services.AddDbContext<CustomersDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        b => b.MigrationsAssembly("Customers.Infrastructure")
    ));

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.Run();
