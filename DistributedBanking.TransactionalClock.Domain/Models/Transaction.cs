public class Transaction : ResultingTransaction
{
    private readonly DateTime _createdAt;
    private readonly int _priority;

    public Transaction(
        string id,
        Dictionary<string, object>? data,
        DateTime createdAt,
        TransactionType operation,
        string database,
        string collection,
        int priority)
        : base(id, data, operation, database, collection)
    {
        if (operation == TransactionType.UPDATE && createdAt == default)
            throw new Exception($"created_at must be present for {operation}");

        _createdAt = createdAt;
        _priority = priority;
    }

    public DateTime CreatedAt => _createdAt;
    public int Priority => _priority;

    public override string ToString()
    {
        return $"{nameof(Id)}={Id}, {nameof(Database)}={Database}, {nameof(Collection)}={Collection}, {nameof(Operation)}={Operation}, CreatedAt={CreatedAt}, Priority={Priority}";
    }
}