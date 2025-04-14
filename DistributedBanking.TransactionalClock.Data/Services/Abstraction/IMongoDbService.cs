namespace DistributedBanking.TransactionalClock.Data.Services.Abstraction;

public interface IMongoDbService
{
    Task AddAsync(string databaseName, string collectionName, object document);
    Task UpdateAsync(string databaseName, string collectionName, string id, object document);
    Task DeleteAsync(string databaseName, string collectionName, string id);
}