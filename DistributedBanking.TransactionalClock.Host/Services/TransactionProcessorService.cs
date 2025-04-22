using DistributedBanking.TransactionalClock.Data.Services.Abstraction;
using DistributedBanking.TransactionalClock.Domain.Models;
using DistributedBanking.TransactionalClock.Domain.Services.Abstraction;
using DistributedBanking.TransactionalClock.Domain.Utils;
using DistributedBanking.TransactionalClock.Host.Extensions;
using Newtonsoft.Json;
using Shared.Data.Entities;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace DistributedBanking.TransactionalClock.Host.Services;

public class TransactionProcessorService : BackgroundService
{
    private readonly IServiceState _state;
    private readonly ICompositeMongoDbService _mongoDbServices;
    private readonly ILogger<TransactionProcessorService> _logger;

    private IDisposable _mergeLoop;
    private IDisposable _receiveNew;
    
    public TransactionProcessorService(
        IServiceState state,
        ICompositeMongoDbService mongoDbService,
        ILogger<TransactionProcessorService> logger)
    {
        _state = state;
        _mongoDbServices = mongoDbService;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting transaction processor service with Rx...");

        // Subscription for new incoming transactions
        _receiveNew = _state.TransactionStream
            .Select(t => Observable.FromAsync(async ct =>
                {
                    try
                    {
                        await _state.SyncLock.WaitAsync(ct);
                        // try
                        // {
                        /*if (!_state.Transactions.TryGetValue(t.Priority, out var collections))
                        {
                            collections = new Dictionary<string, Dictionary<string, List<LightTransaction>>>();
                            _state.Transactions[t.Priority] = collections;
                            _state.OrderByPriorities();
                        }

                        if (!collections.TryGetValue(t.Collection, out var ids))
                        {
                            ids = new Dictionary<string, List<LightTransaction>>();
                            collections[t.Collection] = ids;
                        }

                        if (!ids.TryGetValue(t.Id, out var queue))
                        {
                            queue = new List<LightTransaction>();
                            ids[t.Id] = queue;
                        }
                        */

                        var key = new LightTransactionKey(t.Priority, t.Collection, t.Id);

                        string json = JsonConvert.SerializeObject(t.Data);
                        var result = JsonConvert.DeserializeObject(json, t.Type);
                        var transaction = new LightTransaction(t.Operation, result, t.CreatedAt);

                        var queue = _state.Transactions.GetOrAdd(
                            key,
                            x => []);

                        queue.Add(transaction);
                        _state.OrderByPriorities();

                        //   queue.Add(new LightTransaction(t.Operation, result, t.CreatedAt));
                        //  queue.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
                        /*}
                        }
                        finally
                        {

                        }

                        finally
                        {
                           // _state.SyncLock.Release();
                        }*/
                    }
                    finally
                    {
                        _state.SyncLock.Release();
                    }
                }))
                .Concat()
                .Subscribe();

        // Periodic merge logic
        _mergeLoop = Observable.Interval(TimeSpan.FromMilliseconds(10))
            .Select(_ => Observable.FromAsync(DoMerge))
            .Concat()
            .SubscribeOn(TaskPoolScheduler.Default)
            .Subscribe();
    }

    private async Task DoMerge()
    {
       // 1try
       // 1{ 
           // 1 await _state.SyncLock.WaitAsync();

           try
           {
               await _state.SyncLock.WaitAsync();
               foreach (var (key, transactions) in _state.Transactions)
               {
                   _logger.LogInformation("DoMerge command run at {Time}", DateTime.UtcNow.TimeOfDay);

                   var (priority, collection, id) = key;
                    
                   if (transactions.Count == 0)
                       continue;

                   var delete = transactions.FirstOrDefault(t => t.Operation == CommandType.Delete);
                   if (delete != null)
                   {
                       foreach (var mongoDbService in _mongoDbServices.MongoDbs)
                       {
                           await mongoDbService.DeleteAsync(collection, id);
                       }
                            
                       _state.Transactions.Remove(key, out _);
                       //ids[id] = [];
                       continue;
                   }

                   var creates = transactions.Where(t => t.Operation == CommandType.Create).ToList();
                   if (creates.Any())
                   {
                       var lastCreate = creates.Last();
                       foreach (var mongoDbService in _mongoDbServices.MongoDbs)
                       {
                           await mongoDbService.AddAsync(collection, lastCreate.Payload);
                       }
                            
                       _state.Transactions.Remove(key, out _);

                       //var result = new ResultingTransaction(id, lastCreate.Data, TransactionType.CREATE, database, collection);
                       //_state.ResultingTransactions.Enqueue(result);
                   }

                   var updates = transactions.Where(t => t.Operation == CommandType.Update).OrderBy(t => t.CreatedAt).ToList();
                   if (updates.Count == 0)
                   {
                       _state.Transactions.Remove(key, out _);
                       continue;
                   }

                        
                   var merged = new Dictionary<string, object>();
                   foreach (var t in updates)
                   {
                       DictUtils.Merge(merged, t.Payload.ToDictionary());
                   }
                        

                   foreach (var mongoDbService in _mongoDbServices.MongoDbs)
                   {
                       await mongoDbService.UpdateAsync(collection, id, updates.Last().Payload);
                   }
                        
                   _state.Transactions.Remove(key, out _);

                   //var updateResult = new ResultingTransaction(id, merged, TransactionType.UPDATE, database, collection);
                   //_state.ResultingTransactions.Enqueue(updateResult);
                
               }
           }
           finally
           {
               _state.SyncLock.Release();
           }
            
           
            /* 1 var sorted = _state.Transactions.OrderBy(kv => kv.Key).ToList(); 
            foreach (var (priority, collections) in sorted)
            {
                foreach (var (collection, ids) in collections)
                {
                    foreach (var (id, transactions) in ids)
                    {
                        if (transactions.Count == 0)
                            continue;

                        var delete = transactions.FirstOrDefault(t => t.Operation == CommandType.Delete);
                        if (delete != null)
                        {
                            foreach (var mongoDbService in _mongoDbServices.MongoDbs)
                            {
                                await mongoDbService.DeleteAsync(collection, id);
                            }
                            
                            transactions.Remove(delete);

                            //_state.ResultingTransactions.Enqueue(result);
                            ids[id] = [];
                            continue;
                        }

                        var creates = transactions.Where(t => t.Operation == CommandType.Create).ToList();
                        if (creates.Any())
                        {
                            var lastCreate = creates.Last();
                            
                            /*string json = JsonConvert.SerializeObject(lastCreate.Data); // or input is already a JSON string
                            var result = JsonConvert.DeserializeObject(json, lastCreate.Type);#1#
                            
                            //var payload = Convert.ChangeType(lastCreate.Data, lastCreate.Type); 
                            
                           // var q = new ApplicationRole("aaaaaaaaa");
                            //var qq = q.GetType();
                            //var x = new BsonDocument(lastCreate.Data);
                            foreach (var mongoDbService in _mongoDbServices.MongoDbs)
                            {
                                await mongoDbService.AddAsync(collection, lastCreate.Payload);
                            }
                            transactions.Remove(lastCreate);
                            //var result = new ResultingTransaction(id, lastCreate.Data, TransactionType.CREATE, database, collection);
                            //_state.ResultingTransactions.Enqueue(result);
                        }

                        var updates = transactions.Where(t => t.Operation == CommandType.Update).ToList();
                        if (updates.Count == 0)
                        {
                            ids[id] = [];
                            continue;
                        }

                        /*
                        var merged = new Dictionary<string, object>();
                        foreach (var t in updates)
                        {
                            DictUtils.Merge(merged, t.Payload.ToDictionary());
                        }
                        #1#

                        foreach (var mongoDbService in _mongoDbServices.MongoDbs)
                        {
                            await mongoDbService.UpdateAsync(collection, id, updates.Last().Payload /*merged#1#);
                        }
                        transactions.RemoveAll(x => updates.Contains(x));

                        //var updateResult = new ResultingTransaction(id, merged, TransactionType.UPDATE, database, collection);
                        //_state.ResultingTransactions.Enqueue(updateResult);
                        ids[id] = [];
                    }
                }
                
            }*/
        /*}
        finally
        {
            //_state.SyncLock.Release();
        }*/
    }
}