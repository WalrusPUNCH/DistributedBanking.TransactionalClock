using Shared.Data.Entities;

namespace DistributedBanking.TransactionalClock.Domain.Models;

public class Transaction : TransactionBase
{
    public string Id { get; init; }

    public string Collection { get; init; }
    
    public DateTime CreatedAt { get; init; }

    public int Priority { get; init; }
    
    public Transaction(
        string id,
        object data,
        Type type,
        DateTime createdAt,
        CommandType operation,
        string collection,
        int priority)
            : base(data, type, operation)
    {
        if (operation == CommandType.Update && createdAt == default)
            throw new Exception($"created_at must be present for {operation}");

        Id = id;
        Collection = collection;
        CreatedAt = createdAt;
        Priority = priority;
    }
}