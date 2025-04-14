using Shared.Data.Entities;

namespace DistributedBanking.TransactionalClock.Domain.Models;

public class Transaction : ResultingTransaction
{
    public DateTime CreatedAt { get; init;  }

    public int Priority { get; init; }
    
    public Transaction(
        string id,
        Dictionary<string, object?> data,
        DateTime createdAt,
        CommandType operation,
        string database,
        string collection,
        int priority)
            : base(id, data, operation, database, collection)
    {
        if (operation == CommandType.Update && createdAt == default)
            throw new Exception($"created_at must be present for {operation}");

        CreatedAt = createdAt;
        Priority = priority;
    }
}