namespace DistributedBanking.TransactionalClock.Data.Services.Abstraction;

public interface ICompositeMongoDbService
{
    IEnumerable<MongoDbService> MongoDbs { get; }
}