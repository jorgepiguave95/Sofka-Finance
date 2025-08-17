using MassTransit;
using Account.Api.Messaging;
using Account.Application.Interfaces;
using Account.Application.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Repositories;
using Account.Application.Repositories;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// RabbitMQ config
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<GeneralConsumer>();

    options.UsingRabbitMq((context, cfg) =>
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
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "SofkaAccount";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
    ?? throw new InvalidOperationException("DB_PASSWORD is required");

var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=true;";

builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        b => b.MigrationsAssembly("Account.Infrastructure")
    ));

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IMovementRepository, MovementRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMovementService, MovementService>();

var app = builder.Build();

app.Run();