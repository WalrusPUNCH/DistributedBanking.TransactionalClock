using Shared.Data.Entities;

namespace DistributedBanking.TransactionalClock.Domain.Models;

public abstract class TransactionBase
{
    public CommandType Operation { get; init; }
    public object Data { get; init; }
    public Type Type { get; init; }

    protected TransactionBase(object data, Type type, CommandType operation)
    {
        if (data == null && operation != CommandType.Delete)
            throw new Exception($"data must be present if operation is {operation}");

        Data = data;
        Type = type;
        Operation = operation;
    }
}