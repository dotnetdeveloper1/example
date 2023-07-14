using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.Core.Telemetry;
using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Services
{
    public abstract class MessageHandlerBase<TMessage> : IMessageHandler where TMessage : class
    {
        public MessageHandlerBase(ITelemetryClient telemetryClient, IApplicationContext applicationContext)
        {
            Guard.IsNotNull(telemetryClient, nameof(telemetryClient));
            Guard.IsNotNull(applicationContext, nameof(applicationContext));

            this.telemetryClient = telemetryClient;
            this.applicationContext = applicationContext;
            operationName = GetType().Name;
        }

        public Type MessageType => typeof(TMessage);

        public Task HandleAsync(BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            using (telemetryClient.StartOperation(operationName))
            {
                Guard.IsNotNull(brokeredMessage, nameof(brokeredMessage));

                var innerMessage = GetInnerMessage(brokeredMessage);
                SetApplicationContextProperties(innerMessage);

                return HandleMessageAsync(innerMessage, brokeredMessage, cancellationToken);
            }
        }

        protected virtual Task HandleMessageAsync(TMessage message, BrokeredMessage brokeredMessage, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected TMessage GetInnerMessage(BrokeredMessage brokeredMessage)
        {
            var message = brokeredMessage.InnerMessage as TMessage;

            if (message == null)
            {
                throw new InvalidOperationException(
                    $"Inner message cannot be null. MessageId: {brokeredMessage.Id}, CorrelationId: {brokeredMessage.CorrelationId}, MessageType: {brokeredMessage.MessageType}.");
            }

            return message;
        }

        protected string operationName;
        protected readonly ITelemetryClient telemetryClient;
        protected readonly IApplicationContext applicationContext;

        private void SetApplicationContextProperties(TMessage message) 
        {
            if (message is ServiceBusMessageBase baseMessage)
            {
                applicationContext.TenantId = baseMessage.TenantId;
            }
        }
    }
}
