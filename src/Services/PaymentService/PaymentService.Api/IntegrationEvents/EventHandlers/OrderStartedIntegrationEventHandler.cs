using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace PaymentService.Api.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

        public OrderStartedIntegrationEventHandler(IEventBus eventBus, IConfiguration configuration, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            _eventBus = eventBus;
            _configuration = configuration;
            _logger = logger;
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = _configuration.GetValue<bool>(keyword);

          IntegrationEvent paymentEvent=  paymentSuccessFlag ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId) : new OrderPaymentFailedIntegrationEvent(@event.OrderId,"Fake Error Message");

            _logger.LogInformation($"OrderCreatedIntegrationEventHandler in PaymentService fired PaymentSuccess:{paymentSuccessFlag} , orderId: {@event.OrderId} id: {@event.Id}");

           _eventBus.Publish(paymentEvent);

            return Task.CompletedTask;
        }
    }
}
