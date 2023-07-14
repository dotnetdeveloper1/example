using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using PWP.InvoiceCapture.Core.ServiceBus.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.IntegrationTests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal class FakeMessageHandler : IMessageHandler
    {
        public FakeMessageHandler(SemaphoreSlim sync) 
        {
            this.sync = sync;
            sync.Wait();
        }

        public List<BrokeredMessage> HandledMessages { get; } = new List<BrokeredMessage>();

        public Type MessageType => typeof(InvoiceDocumentUploadedMessage);

        public Task HandleAsync(BrokeredMessage brokeredMessage, CancellationToken token)
        {
            HandledMessages.Add(brokeredMessage);
            sync.Release();

            return Task.CompletedTask;
        }

        private readonly SemaphoreSlim sync;
    }
}
