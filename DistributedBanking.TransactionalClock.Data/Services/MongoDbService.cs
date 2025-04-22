using DistributedBanking.TransactionalClock.Data.Services.Abstraction;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Data.Services;

namespace DistributedBanking.TransactionalClock.Data.Services;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoDbFactory _client;

    public MongoDbService(IMongoDbFactory client)
    {
        _client = client;
    }

    public async Task AddAsync<T>(string collectionName, T document)
    {
        var collection = GetCollection<T>(collectionName);
        await collection.InsertOneAsync(document);
    }

    public async Task UpdateAsync<T>(string collectionName, string id, T document)
    {
        var collection = GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
        await collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = false });
    }

    public async Task DeleteAsync(string collectionName, string id)
    {
        var collection = GetCollection<object>(collectionName);
        var filter = Builders<object>.Filter.Eq("_id", new ObjectId(id));
        await collection.DeleteOneAsync(filter);
    }

    private IMongoCollection<T> GetCollection<T>(string collection)
    {
        return _client.GetDatabase().GetCollection<T>(collection);
    }
}