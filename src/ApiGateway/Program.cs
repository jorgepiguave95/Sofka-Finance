using Microsoft.OpenApi.Models;
using ApiGateway.Messaging;
using DotNetEnv;

Env.Load(".env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RabbitMQ config
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";
var rabbitConnectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:5672/";

// Registrar el cliente RabbitMQ unificado
builder.Services.AddSingleton<IMessagingClient>(provider =>
    new RabbitMQMessagingClient(rabbitConnectionString));

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
