using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.OutputCaching.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace BusinessLayer.Caching;

public static class RedisOutputCacheServiceCollectionExtensions
{
    public static IServiceCollection AddRedisOutputCache(this IServiceCollection services)
    {
        
        ArgumentNullException.ThrowIfNull(services);

        services.AddOutputCache();

        services.RemoveAll<IOutputCacheStore>();

        services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
        
        return services;
    }
    
    public static IServiceCollection AddRedisOutputCache(
        this IServiceCollection services,
        Action<OutputCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull((object) services, nameof (services));
        
        ArgumentNullException.ThrowIfNull((object) configureOptions, nameof (configureOptions));
        
        services.Configure<OutputCacheOptions>(configureOptions);
        
        services.AddOutputCache();
        
        services.RemoveAll<IOutputCacheStore>();

        services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();
        
        
        return services;
    }
}