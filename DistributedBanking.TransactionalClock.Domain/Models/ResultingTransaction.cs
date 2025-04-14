using Shared.Data.Entities;

namespace DistributedBanking.TransactionalClock.Domain.Models;

public class ResultingTransaction : TransactionBase
{
    public string Id { get; init; }

    public string Database { get; init; }

    public string Collection { get; init; }

    protected ResultingTransaction(string id, Dictionary<string, object?> data, CommandType operation, string database, string collection)
        : base(data, operation)
    {
        Id = id;
        Database = database;
        Collection = collection;
    }
}