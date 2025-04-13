using DistributedBanking.TransactionalClock.Host.Listeners.Base;
using Shared.Kafka.Messages;
using Shared.Kafka.Services;

namespace DistributedBanking.TransactionalClock.Host.Listeners;

public class CommandsListener : BaseListener<string, Command>
{
    public CommandsListener(
        IKafkaConsumerService<string, Command> consumer,
        ILogger<CommandsListener> logger) 
            : base(consumer, logger)
    {
        
    }

    protected async override Task ProcessMessage(MessageWrapper<Command> message)
    {
        
    }
}