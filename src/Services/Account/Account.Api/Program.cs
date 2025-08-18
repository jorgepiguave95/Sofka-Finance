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
var rabbitConnectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:5672/";

// Registrar el RabbitMQ Consumer unificado como Hosted Service
builder.Services.AddSingleton<RabbitMQConsumer>(provider =>
    new RabbitMQConsumer(provider, rabbitConnectionString));
builder.Services.AddHostedService(provider => provider.GetRequiredService<RabbitMQConsumer>());

// SQL Server Config
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? throw new InvalidOperationException("DB_HOST is required");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var dbName = Environment.GetEnvironmentVariable("DB_NAME_ACCOUNT") ?? throw new InvalidOperationException("DB_NAME_ACCOUNT is required");
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

// Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error aplicando migraciones: {ex.Message}");
    }
}

app.Run();