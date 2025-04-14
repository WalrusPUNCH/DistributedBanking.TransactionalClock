using Shared.Data.Entities;

namespace DistributedBanking.TransactionalClock.Domain.Models;

public abstract class TransactionBase
{
    public CommandType Operation { get; init; }
    public Dictionary<string, object?> Data { get; init; }

    protected TransactionBase(Dictionary<string, object?> data, CommandType operation)
    {
        if (data == null && operation != CommandType.Delete)
            throw new Exception($"data must be present if operation is {operation}");

        Data = data;
        Operation = operation;

        Cleanup();
    }

    private void Cleanup()
    {
        Data.Remove("_id");
    }
}