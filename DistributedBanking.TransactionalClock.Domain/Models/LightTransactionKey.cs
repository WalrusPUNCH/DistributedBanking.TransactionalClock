namespace DistributedBanking.TransactionalClock.Domain.Models;

public record LightTransactionKey(
    int Priority,
    string Collection,
    string Id);