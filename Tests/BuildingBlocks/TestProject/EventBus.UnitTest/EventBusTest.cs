using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.Events;
using EventBus.UnitTest.Events.EventsHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Security.Authentication.ExtendedProtection;

namespace EventBus.UnitTest
{
    public class Tests
    {
        private readonly ServiceCollection _services;
        public Tests()
        {
            _services = new ServiceCollection();
            _services.AddLogging(conf => conf.AddConsole());

        }


        [Test]
        public void Subscribe_event_on_rabbitmq_test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = GetRabbitMQConfig();
                return EventBusFactory.Create(config, sp);
            });
            var sp = _services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            Assert.Pass();
        }


        [Test]
        public void Subscribe_event_on_azure_test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = GetAzureConfig();
                return EventBusFactory.Create(config, sp);
            });
            var sp = _services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            Assert.Pass();
        }

        [Test]
        public void Send_message_to_rabbitMQ_test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            { 
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });
            var sp = _services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();
            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }
       

        [Test]
        public void Send_message_to_azure_test()
        {
            _services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });
            var sp = _services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();
            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }

        private EventBusConfig GetAzureConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBusUnitTest",
                DefaultTopicName = "SellingStarTopicName",
                EventBusType = EventBusType.AzureService,
                EventNameSuffix = "IntegrationEvent",
                EventBusConnectionString = "Endpoint=sb://sellingstar.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=MI3PWT5X0GPa24ux0EAZnVpuV+z9/0kw0+ASbAiGxc0="
            };
        }

        private EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "SellingStarTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",
                //Connection=new ConnectionFactory
                //{
                //    HostName="localhost",
                //    Port= 15672,
                //    UserName="guest",
                //    Password="guest"
                //}
            };
        }
    }
}