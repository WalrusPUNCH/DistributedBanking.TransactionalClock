using DistributedBanking.TransactionalClock.Data.Services.Abstraction;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DistributedBanking.TransactionalClock.Data.Services;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoClient _client;

    public MongoDbService(IMongoClient client)
    {
        _client = client;
    }

    public async Task AddAsync(string databaseName, string collectionName, object document)
    {
        var collection = GetCollection(databaseName, collectionName);
        await collection.InsertOneAsync(document);
    }

    public async Task UpdateAsync(string databaseName, string collectionName, string id, object document)
    {
        var collection = GetCollection(databaseName, collectionName);
        var filter = Builders<object>.Filter.Eq("_id", new ObjectId(id));
        await collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = false });
    }

    public async Task DeleteAsync(string databaseName, string collectionName, string id)
    {
        var collection = GetCollection(databaseName, collectionName);
        var filter = Builders<object>.Filter.Eq("_id", new ObjectId(id));
        await collection.DeleteOneAsync(filter);
    }

    private IMongoCollection<object> GetCollection(string db, string collection)
    {
        return _client.GetDatabase(db).GetCollection<object>(collection);
    }
}