using Shared.Data.Entities;

namespace DistributedBanking.TransactionalClock.Domain.Models;

public record LightTransaction(
    CommandType Operation,
    object Payload,
    DateTime CreatedAt);
