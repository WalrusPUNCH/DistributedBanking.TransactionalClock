using DistributedBanking.TransactionalClock.Data.Extensions;
using DistributedBanking.TransactionalClock.Domain.Extensions;
using DistributedBanking.TransactionalClock.Host.Listeners;

namespace DistributedBanking.TransactionalClock.Host.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDatabase(""); //todo
        services.AddDomainServices();
        
        return services;
    }
    
    public static IServiceCollection AddBackgroundListeners(this IServiceCollection services)
    {
        services.AddHostedService<CommandsListener>();
        services.AddHostedService<TransactionProcessorService>();
        
        return services;
    }
}