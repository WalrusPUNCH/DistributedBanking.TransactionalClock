using DistributedBanking.TransactionalClock.Domain.Models;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace DistributedBanking.TransactionalClock.Domain.Services.Abstraction;

public interface IServiceState
{
    ConcurrentDictionary<LightTransactionKey, ConcurrentBag<LightTransaction>> Transactions { get; }
    Subject<Transaction> TransactionStream { get; }
    SemaphoreSlim SyncLock { get; }
    
    void OrderByPriorities();
}