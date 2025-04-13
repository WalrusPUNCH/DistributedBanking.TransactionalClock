using System.Reactive.Linq;
using DistributedBanking.TransactionalClock.Data.Services.Abstraction;
using DistributedBanking.TransactionalClock.Domain.Services;
using DistributedBanking.TransactionalClock.Domain.Utils;

public class TransactionProcessorService : BackgroundService
{
    private readonly ServiceState _state;
    private readonly IMongoDbService _mongoDbService;
    private readonly ILogger<TransactionProcessorService> _logger;

    public TransactionProcessorService(
        ServiceState state,
        IMongoDbService mongoDbService,
        ILogger<TransactionProcessorService> logger)
    {
        _state = state;
        _mongoDbService = mongoDbService;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting transaction processor service with Rx...");

        // Subscription for new incoming transactions
        _state.TransactionStream
            .SelectMany(t => Observable.FromAsync(async ct =>
            {
                await _state.SyncLock.WaitAsync(ct);
                try
                {
                    if (!_state.Transactions.TryGetValue(t.Priority, out var databases))
                    {
                        databases = new Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>();
                        _state.Transactions[t.Priority] = databases;
                        _state.OrderByPriorities();
                    }

                    if (!databases.TryGetValue(t.Database, out var collections))
                    {
                        collections = new Dictionary<string, Dictionary<string, List<Transaction>>>();
                        databases[t.Database] = collections;
                    }

                    if (!collections.TryGetValue(t.Collection, out var ids))
                    {
                        ids = new Dictionary<string, List<Transaction>>();
                        collections[t.Collection] = ids;
                    }

                    if (!ids.TryGetValue(t.Id, out var queue))
                    {
                        queue = new List<Transaction>();
                        ids[t.Id] = queue;
                    }

                    queue.Add(t);
                    queue.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
                }
                finally
                {
                    _state.SyncLock.Release();
                }
            }))
            .Subscribe();

        // Periodic merge logic every 100ms
        Observable.Interval(TimeSpan.FromMilliseconds(100))
            .SelectMany(_ => Observable.FromAsync(DoMerge))
            .Subscribe();
    }

    private async Task DoMerge()
    {
        await _state.SyncLock.WaitAsync();

        var sorted = _state.Transactions.OrderBy(kv => kv.Key).ToList();
        foreach (var (priority, databases) in sorted)
        {
            foreach (var (database, collections) in databases)
            {
                foreach (var (collection, ids) in collections)
                {
                    foreach (var (id, transactions) in ids)
                    {
                        if (transactions.Count == 0)
                            continue;

                        if (transactions.Any(t => t.Operation == TransactionType.DELETE))
                        {
                            await _mongoDbService.DeleteAsync(database, collection, id);
                            //_state.ResultingTransactions.Enqueue(result);
                            ids[id] = [];
                            continue;
                        }

                        var creates = transactions.Where(t => t.Operation == TransactionType.CREATE).ToList();
                        if (creates.Any())
                        {
                            var lastCreate = creates.Last();
                            await _mongoDbService.AddAsync(database, collection, lastCreate.Data);
                            //var result = new ResultingTransaction(id, lastCreate.Data, TransactionType.CREATE, database, collection);
                            //_state.ResultingTransactions.Enqueue(result);
                        }

                        var updates = transactions.Where(t => t.Operation == TransactionType.UPDATE).ToList();
                        if (updates.Count == 0)
                        {
                            ids[id] = [];
                            continue;
                        }

                        var merged = new Dictionary<string, object>();
                        foreach (var t in updates)
                        {
                            DictUtils.Merge(merged, t.Data);
                        }

                        await _mongoDbService.UpdateAsync(database, collection, id, merged);
                        //var updateResult = new ResultingTransaction(id, merged, TransactionType.UPDATE, database, collection);
                        //_state.ResultingTransactions.Enqueue(updateResult);
                        ids[id] = [];
                    }
                }
            }
        }
        
    }
}
