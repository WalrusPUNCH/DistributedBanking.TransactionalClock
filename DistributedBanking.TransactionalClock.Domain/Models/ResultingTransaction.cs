using DistributedBanking.TransactionalClock.Domain.Models;

public class ResultingTransaction : TransactionBase
{
    private readonly string _id;
    private readonly string _database;
    private readonly string _collection;

    public ResultingTransaction(string id, Dictionary<string, object>? data, TransactionType operation, string database, string collection)
        : base(data, operation)
    {
        _id = id;
        _database = database;
        _collection = collection;
    }

    public string Id => _id;
    public string Database => _database;
    public string Collection => _collection;
}