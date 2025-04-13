using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Shared.Kafka.Messages;
using Shared.Kafka.Services;

namespace DistributedBanking.TransactionalClock.Host.Listeners.Base;

public abstract class BaseListener<TMessageKey, TMessageValue> : BackgroundService where TMessageValue : class
{
    private const int MaxDelaySeconds = 60;
    private readonly IKafkaConsumerService<TMessageKey, TMessageValue> _consumer;
    protected readonly ILogger<BaseListener<TMessageKey, TMessageValue>> Logger;

    protected BaseListener(
        IKafkaConsumerService<TMessageKey, TMessageValue> workerRegistrationConsumer,
        ILogger<BaseListener<TMessageKey, TMessageValue>> logger)
    {
        _consumer = workerRegistrationConsumer;
        Logger = logger;
    }

    protected virtual bool FilterMessage(MessageWrapper<TMessageValue> message)
    {
        return true;
    }
    
    protected abstract Task ProcessMessage(MessageWrapper<TMessageValue> message);

    protected virtual void OnMessageProcessingException(Exception exception, TimeSpan delay, MessageWrapper<TMessageValue> message)
    {
        Logger.LogError(exception, "Error while trying to process '{MessageType}' message. Retry in {Delay}",
            typeof(TMessageValue).Name, delay);
    }
   
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            Logger.LogInformation("Listener {Listener} has received a stop signal", this.GetType().Name);
        });

        _consumer
            .Consume(stoppingToken)
            .Where(FilterMessage)
            .Select(message => Observable.FromAsync(async () =>
                {
                    Logger.LogInformation("Listener {Listener} has received a message", this.GetType().Name);
                    
                    await ProcessMessage(message);
                })
                .RetryWhen(errors => errors.SelectMany((exception, retry) => 
                {
                    var delay = TimeSpan.FromSeconds(Math.Max(MaxDelaySeconds, retry * 2));
                    OnMessageProcessingException(exception, delay, message);
                    return Observable.Timer(delay); 
                }))
            )
            .Concat()
            .RetryWhen(errors => errors.SelectMany((exception, retry) => 
            {
                var delay = TimeSpan.FromSeconds(Math.Max(MaxDelaySeconds, retry * 10));
                Logger.LogError(exception, "Error while listening to '{MessageType}' messages. Retry in {Delay} seconds", 
                    typeof(TMessageValue).Name, delay);
                
                return Observable.Timer(delay);
            }))
            .SubscribeOn(TaskPoolScheduler.Default)
            .Subscribe(
                onNext: _ => { Logger.LogInformation("{Listener} has processed a message", this.GetType().Name);  },
                onError: exception => 
                { 
                    Logger.LogError(exception, "An unexpected error occurred while listening to '{MessageType}' messages", typeof(TMessageValue).Name);
                },
                onCompleted: () => { Logger.LogInformation("{Listener} has ended its work", this.GetType().Name); },
                token: stoppingToken);

        return Task.CompletedTask;
    }
}