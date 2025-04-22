using DistributedBanking.TransactionalClock.Data.Services;
using DistributedBanking.TransactionalClock.Data.Services.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Shared.Data.Services.Implementation.MongoDb;

namespace DistributedBanking.TransactionalClock.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDatabase(
        this IServiceCollection services,
        IEnumerable<string> connectionStrings,
        string database)
    {
        services.AddSingleton<ICompositeMongoDbService>(_ => 
        {
            return new CompositeMongoDbService(connectionStrings.
                Select(connectionString => new MongoDbService(new MongoDbFactory(connectionString, database))));
        });
        
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreIfNullConvention(true)
        };
        ConventionRegistry.Register("CamelCase_StringEnum_IgnoreNull_Convention", pack, _ => true);

        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("Shared.Data.Entities"));
        BsonSerializer.RegisterSerializer(objectSerializer);
        
        BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        
        return services;
    }
}