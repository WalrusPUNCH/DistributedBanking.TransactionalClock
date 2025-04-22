using DistributedBanking.TransactionalClock.Domain.Models;
using DistributedBanking.TransactionalClock.Domain.Services.Abstraction;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace DistributedBanking.TransactionalClock.Domain.Services;

public class ServiceState : IServiceState
{
   // public ConcurrentDictionary<int, Dictionary<string, Dictionary<string, List<LightTransaction>>>> Transactions { get; } = new();
    public ConcurrentDictionary<LightTransactionKey, ConcurrentBag<LightTransaction>> Transactions { get; private set; } = new();

    public Subject<Transaction> TransactionStream { get; } = new();

    public SemaphoreSlim SyncLock { get; } = new(1, 1);

    public void OrderByPriorities()
    {
        
        Transactions = new ConcurrentDictionary<LightTransactionKey, ConcurrentBag<LightTransaction>>(Transactions
            .OrderBy(kv => kv.Key.Priority)
            .ToDictionary(x => x.Key, x => x.Value));
        
        /*Transactions.Clear();
        foreach (var kv in ordered)
        {
            Transactions[kv.Key] = kv.Value;
        }*/
    }
}