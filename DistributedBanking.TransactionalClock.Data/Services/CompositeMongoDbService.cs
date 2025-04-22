using DistributedBanking.TransactionalClock.Data.Services.Abstraction;

namespace DistributedBanking.TransactionalClock.Data.Services;

public class CompositeMongoDbService : ICompositeMongoDbService
{
    public CompositeMongoDbService(IEnumerable<MongoDbService> mongoDbs)
    {
        MongoDbs = mongoDbs;
    }

    public IEnumerable<MongoDbService> MongoDbs { get; init; }
}