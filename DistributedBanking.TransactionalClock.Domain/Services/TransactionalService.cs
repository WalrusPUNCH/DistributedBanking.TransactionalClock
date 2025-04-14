/*
using System.Collections.Concurrent;
using DistributedBanking.TransactionalClock.Domain.Utils;
using Microsoft.Extensions.Logging;

public class TransactionalService
{
    private const int DEFAULT_PRIORITY = 50;

    private readonly ConcurrentQueue<Transaction> _unprocessedTransactions = new();
    private readonly Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>> _transactions = new();
    private readonly ConcurrentQueue<ResultingTransaction> _resultingTransactions = new();
    private bool _isPaused;

    private readonly object syncLock = new();
    private readonly ILogger<TransactionalService> _logger;

    public TransactionalService(ILogger<TransactionalService> logger)
    {
        _logger = logger;
    }

    public void AddNewTransaction(Transaction t)
    {
        lock (syncLock)
        {
            _unprocessedTransactions.Enqueue(t);
            Console.WriteLine($"Adding: {t}");
        }
    }

    public void Reset()
    {
        lock (syncLock)
        {
            _unprocessedTransactions.Clear();
            _transactions.Clear();
            while (_resultingTransactions.TryDequeue(out _)) { }
        }
    }

    public void On()
    {
        lock (syncLock)
        {
            _isPaused = false;
        }
    }

    public void Off()
    {
        lock (syncLock)
        {
            _isPaused = true;
        }
    }

    private void AddNewcomes()
    {
        while (true)
        {
            lock (syncLock)
            {
                while (_unprocessedTransactions.TryDequeue(out var t))
                {
                    if (!_transactions.ContainsKey(t.Priority))
                    {
                        _transactions[t.Priority] = new Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>();
                        OrderByPriorities();
                    }

                    var databases = _transactions[t.Priority];
                    if (!databases.ContainsKey(t.Database))
                        databases[t.Database] = new Dictionary<string, Dictionary<string, List<Transaction>>>();

                    var collections = databases[t.Database];
                    if (!collections.ContainsKey(t.Collection))
                        collections[t.Collection] = new Dictionary<string, List<Transaction>>();

                    var ids = collections[t.Collection];
                    if (!ids.ContainsKey(t.Id))
                        ids[t.Id] = new List<Transaction>();

                    var queue = ids[t.Id];
                    queue.Add(t);
                    ids[t.Id] = queue.OrderBy(x => x.CreatedAt).ToList();
                }
            }
        }
    }

    private void MergeTransactions()
    {
        void DoIteration()
        {
            var sorted = _transactions.OrderBy(kv => kv.Key).ToList();
            _logger.LogDebug("Sorted: {0}", sorted);

            var isProcessed = false;

            foreach (var (priority, databases) in sorted)
            {
                _logger.LogDebug("Databases: {0}", databases.Keys);

                foreach (var (database, collections) in databases)
                {
                    foreach (var (collection, ids) in collections)
                    {
                        foreach (var (_id, transactions) in ids)
                        {
                            if (transactions.Count == 0)
                                continue;

                            var deletes = transactions.Where(t => t.Operation == TransactionType.DELETE).ToList();
                            if (deletes.Count > 0)
                            {
                                _resultingTransactions.Enqueue(new ResultingTransaction(_id, null, TransactionType.DELETE, database, collection));
                                ids[_id] = new List<Transaction>();
                                continue;
                            }

                            var creates = transactions.Where(t => t.Operation == TransactionType.CREATE).ToList();
                            if (creates.Count > 0)
                            {
                                var t = creates.Last();
                                _resultingTransactions.Enqueue(new ResultingTransaction(_id, t.Data, TransactionType.CREATE, database, collection));
                            }

                            var updates = transactions.Where(t => t.Operation == TransactionType.UPDATE).ToList();
                            if (updates.Count == 0)
                            {
                                ids[_id] = new List<Transaction>();
                                continue;
                            }

                            var res = new Dictionary<string, object>();
                            foreach (var t in updates)
                            {
                                DictUtils.Merge(res, t.Data);
                            }

                            _resultingTransactions.Enqueue(new ResultingTransaction(_id, res, TransactionType.UPDATE, database, collection));
                            ids[_id] = new List<Transaction>();

                            isProcessed = true;
                        }
                    }
                }

                if (isProcessed) break;
            }
        }

        while (true)
        {
            if (_isPaused)
                continue;

            lock (syncLock)
            {
                DoIteration();
            }
        }
    }

    public void PushResultingTransactions()
    {
        // Not implemented
    }

    public IEnumerable<Transaction> UnprocessedTransactions => _unprocessedTransactions;

    public Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>> Transactions => _transactions;

    public IEnumerable<ResultingTransaction> ResultingTransactions => _resultingTransactions;

    private void OrderByPriorities()
    {
        var keys = _transactions.Keys.OrderBy(k => k).ToList();
        var ordered = new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<string, List<Transaction>>>>>();

        foreach (var k in keys)
        {
            ordered[k] = _transactions[k];
        }

        _transactions.Clear();
        foreach (var kv in ordered)
        {
            _transactions[kv.Key] = kv.Value;
        }
    }

    public string GenerateId()
    {
        // Not implemented
        return Guid.NewGuid().ToString();
    }
}
*/
