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

// RabbitMQ config
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";
var rabbitConnectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:5672/";

// Registrar los consumers RabbitMQ especializados
builder.Services.AddSingleton(provider =>
    new AuthRabbitMQConsumer(provider, rabbitConnectionString));
builder.Services.AddHostedService(provider =>
    provider.GetRequiredService<AuthRabbitMQConsumer>());

builder.Services.AddSingleton(provider =>
    new CustomerRabbitMQConsumer(provider, rabbitConnectionString));
builder.Services.AddHostedService(provider =>
    provider.GetRequiredService<CustomerRabbitMQConsumer>());

Console.WriteLine($"[Customer Service] RabbitMQ configurado en: {rabbitConnectionString}");

// SQL Server Config
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? throw new InvalidOperationException("DB_HOST is required");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var dbName = Environment.GetEnvironmentVariable("DB_NAME_CONSUMER") ?? throw new InvalidOperationException("DB_NAME_CONSUMER is required");
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
    ?? throw new InvalidOperationException("DB_PASSWORD is required");

Console.WriteLine($"[CONFIG] Conectando a base de datos: {dbName} en {dbHost}:{dbPort}");

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

// Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
    try
    {
        context.Database.Migrate();
        Console.WriteLine("Migraciones aplicadas exitosamente para CustomerSofka");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error aplicando migraciones: {ex.Message}");
    }
}

app.Run();
