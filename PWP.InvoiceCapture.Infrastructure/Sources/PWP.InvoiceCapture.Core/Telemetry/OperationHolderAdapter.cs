using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using PWP.InvoiceCapture.Core.Utilities;

namespace PWP.InvoiceCapture.Core.Telemetry
{
    public class OperationHolderAdapter : IOperation
    {
        public OperationHolderAdapter(IOperationHolder<RequestTelemetry> operationHolder) 
        {
            Guard.IsNotNull(operationHolder, nameof(operationHolder));

            this.operationHolder = operationHolder;
        }

        public void Dispose() => operationHolder.Dispose();

        private readonly IOperationHolder<RequestTelemetry> operationHolder;
    }
}
