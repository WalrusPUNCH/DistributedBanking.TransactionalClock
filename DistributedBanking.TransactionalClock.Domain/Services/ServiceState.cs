using DistributedBanking.TransactionalClock.Domain.Models;
using DistributedBanking.TransactionalClock.Domain.Services.Abstraction;
using System.Reactive.Subjects;

namespace DistributedBanking.TransactionalClock.Domain.Services;

public class ServiceState : IServiceState
{
    public Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>> Transactions { get; } = new();

    public Subject<Transaction> TransactionStream { get; } = new();

    public SemaphoreSlim SyncLock { get; } = new(1, 1);

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