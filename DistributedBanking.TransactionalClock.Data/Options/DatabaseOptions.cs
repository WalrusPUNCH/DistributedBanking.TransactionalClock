namespace DistributedBanking.TransactionalClock.Data.Options;

public record DatabaseOptions(
    string[] ConnectionStrings,
    string DatabaseName);