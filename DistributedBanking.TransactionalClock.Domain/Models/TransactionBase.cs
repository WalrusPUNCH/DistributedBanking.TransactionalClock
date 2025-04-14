namespace DistributedBanking.TransactionalClock.Domain.Models;

public class TransactionBase
{
    public TransactionType Operation { get; init; }
    public Dictionary<string, object>? Data { get; init; }

    public TransactionBase(Dictionary<string, object>? data, TransactionType operation)
    {
        if (data == null && operation != TransactionType.DELETE)
            throw new Exception($"data must be present if operation is {operation}");

        Data = data;
        Operation = operation;

        Cleanup();
    }

    private void Cleanup()
    {
        const string key = "_id";
        if (Data != null && Data.ContainsKey(key))
        {
            Data.Remove(key);
        }
    }
}