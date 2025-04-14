using DistributedBanking.TransactionalClock.Data.Services;
using DistributedBanking.TransactionalClock.Data.Services.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DistributedBanking.TransactionalClock.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDatabase(this IServiceCollection services, string connectionString)
    {
        // Add your data-related services here
        // For example:
        // services.AddDbContext<YourDbContext>(options =>
        //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<IMongoClient>(s => new MongoClient(connectionString));
        services.AddSingleton<IMongoDbService, MongoDbService>();
        
        return services;
    }
}