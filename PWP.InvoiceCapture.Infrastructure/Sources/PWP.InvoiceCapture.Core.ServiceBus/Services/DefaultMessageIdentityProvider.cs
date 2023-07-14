using PWP.InvoiceCapture.Core.ServiceBus.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Core.ServiceBus.Services
{
    public class DefaultMessageIdentityProvider : IMessageIdentityProvider
    {
        public Task<string> GetNextAsync(CancellationToken cancellationToken)
        {
            var messageId = Guid.NewGuid().ToString();

            return Task.FromResult(messageId);
        }
    }
}
