/*using DistributedBanking.TransactionalClock.Domain.Services;

namespace DistributedBanking.TransactionalClock.Host.Services;

public class CommandsBatcher : BackgroundService
{
    private readonly ServiceState _state;
    private readonly ILogger<CommandsBatcher> _logger;

    public CommandsBatcher(
        ServiceState state,
        ILogger<CommandsBatcher> logger)
    {
        _state = state;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _state.SyncLock.WaitAsync(stoppingToken);

            while (_state.UnprocessedTransactions.TryDequeue(out var t))
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

                if (!ids.TryGetValue(t.Id, out var list))
                {
                    list = [];
                    ids[t.Id] = list;
                }

                list.Add(t);
                list.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
            }
            
            Thread.Sleep(10); // prevent tight loop
        }
    }
}*/