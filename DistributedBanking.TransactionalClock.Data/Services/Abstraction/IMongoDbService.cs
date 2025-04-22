namespace DistributedBanking.TransactionalClock.Data.Services.Abstraction;

public interface IMongoDbService
{
    Task AddAsync<T>(string collectionName, T document);
    Task UpdateAsync<T>(string collectionName, string id, T document);
    Task DeleteAsync(string collectionName, string id);
}