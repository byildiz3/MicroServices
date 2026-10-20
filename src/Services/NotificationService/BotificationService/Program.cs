
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static void Main(string[] args)
    {
        ServiceCollection services = new ServiceCollection();
        ConfiguraServices(services);
    }

    private static void ConfiguraServices(ServiceCollection services)
    {
        services.AddLogging(conf => conf.AddConsole());
        services.AddSingleton<IEventBus>(sp =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "NotificationService",
                DefaultTopicName = "SellingStarTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent"

            };
            return EventBusFactory.Create(config, sp);
        });
    }
}