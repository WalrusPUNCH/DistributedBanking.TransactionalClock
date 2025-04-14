using DistributedBanking.TransactionalClock.Domain.Models;
using System.Reactive.Subjects;

namespace DistributedBanking.TransactionalClock.Domain.Services.Abstraction;

public interface IServiceState
{
    Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>> Transactions { get; }
    Subject<Transaction> TransactionStream { get; }
    SemaphoreSlim SyncLock { get; }
    
    void OrderByPriorities();
}