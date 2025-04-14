namespace DistributedBanking.TransactionalClock.Data.Options;

public record DatabaseOptions(
    string ConnectionString,
    string DatabaseName);