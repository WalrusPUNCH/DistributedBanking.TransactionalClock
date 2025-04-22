using DistributedBanking.TransactionalClock.Data.Extensions;
using DistributedBanking.TransactionalClock.Data.Options;
using DistributedBanking.TransactionalClock.Domain.Extensions;
using DistributedBanking.TransactionalClock.Host.Listeners;
using DistributedBanking.TransactionalClock.Host.Services;
using Shared.Kafka.Extensions;
using Shared.Kafka.Messages;
using Shared.Kafka.Options;

namespace DistributedBanking.TransactionalClock.Host.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        ArgumentNullException.ThrowIfNull(databaseOptions);
        
        services.AddMongoDatabase(databaseOptions.ConnectionStrings, databaseOptions.DatabaseName);
        services.AddDomainServices();
        
        return services;
    }
    
    public static IServiceCollection AddBackgroundListeners(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafkaConsumers(configuration);
        
        services.AddHostedService<CommandsListener>();
        services.AddHostedService<TransactionProcessorService>();
        
        return services;
    }
    
    private static IServiceCollection AddKafkaConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKafkaConsumer<string, Command>(configuration, KafkaTopicSource.Commands);
        
        return services;
    }
    
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
        
        return services;
    }
}