using DistributedBanking.TransactionalClock.Domain.Services;
using DistributedBanking.TransactionalClock.Domain.Services.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedBanking.TransactionalClock.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IServiceState, ServiceState>();
        
        return services;
    }
}