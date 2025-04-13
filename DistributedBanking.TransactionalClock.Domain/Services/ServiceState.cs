using System.Reactive.Subjects;
using DistributedBanking.TransactionalClock.Domain.Services.Abstraction;

namespace DistributedBanking.TransactionalClock.Domain.Services;

public class ServiceState : IServiceState
{
    public Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>> Transactions { get; } = new();
    
    public readonly Subject<Transaction> TransactionStream = new();

    public readonly SemaphoreSlim SyncLock = new(1, 1);

    public void OrderByPriorities()
    {
        var ordered = Transactions.OrderBy(kv => kv.Key).ToList();
        Transactions.Clear();
        foreach (var kv in ordered)
        {
            Transactions[kv.Key] = kv.Value;
        }
    }
}