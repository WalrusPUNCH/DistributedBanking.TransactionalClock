using DistributedBanking.TransactionalClock.Domain.Models;
using DistributedBanking.TransactionalClock.Domain.Services.Abstraction;
using DistributedBanking.TransactionalClock.Host.Listeners.Base;
using Shared.Kafka.Messages;
using Shared.Kafka.Services;

namespace DistributedBanking.TransactionalClock.Host.Listeners;

public class CommandsListener : BaseListener<string, Command>
{
    private readonly IServiceState _serviceState;

    public CommandsListener(
        IKafkaConsumerService<string, Command> consumer,
        IServiceState serviceState,
        ILogger<CommandsListener> logger) 
            : base(consumer, logger)
    {
        _serviceState = serviceState;
    }

    protected override Task ProcessMessage(MessageWrapper<Command> message)
    {
        var transaction = new Transaction(
            message.Message.Id,
            message.Message.Payload,
            message.Message.PayloadType,
            message.Message.CreatedAt,
            message.Message.Operation,
            message.Message.Collection,
            message.Message.Priority);
        
        _serviceState.TransactionStream.OnNext(transaction);

        return Task.CompletedTask;
    }
}