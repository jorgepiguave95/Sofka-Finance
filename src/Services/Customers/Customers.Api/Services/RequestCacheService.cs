using System.Collections.Concurrent;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Services;

public interface IRequestCacheService
{
    Task<CreateCustomerResponse?> GetOrSetAsync(Guid operationId, Func<Task<CreateCustomerResponse>> factory);
    void Remove(Guid operationId);
}

public class RequestCacheService : IRequestCacheService
{
    private readonly ConcurrentDictionary<Guid, Task<CreateCustomerResponse>> _cache = new();
    private readonly Timer _cleanupTimer;

    public RequestCacheService()
    {
        // Limpiar cache cada 5 minutos
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public async Task<CreateCustomerResponse?> GetOrSetAsync(Guid operationId, Func<Task<CreateCustomerResponse>> factory)
    {
        // Si ya existe una operación en progreso con el mismo ID, esperar su resultado
        var task = _cache.GetOrAdd(operationId, _ => ExecuteWithCleanup(operationId, factory));
        return await task;
    }

    private async Task<CreateCustomerResponse> ExecuteWithCleanup(Guid operationId, Func<Task<CreateCustomerResponse>> factory)
    {
        try
        {
            var result = await factory();
            return result;
        }
        finally
        {
            // Limpiar el cache después de completar
            _cache.TryRemove(operationId, out _);
        }
    }

    public void Remove(Guid operationId)
    {
        _cache.TryRemove(operationId, out _);
    }

    private void CleanupExpiredEntries(object? state)
    {
        var expiredKeys = new List<Guid>();

        foreach (var kvp in _cache)
        {
            if (kvp.Value.IsCompleted)
            {
                expiredKeys.Add(kvp.Key);
            }
        }

        foreach (var key in expiredKeys)
        {
            _cache.TryRemove(key, out _);
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}
