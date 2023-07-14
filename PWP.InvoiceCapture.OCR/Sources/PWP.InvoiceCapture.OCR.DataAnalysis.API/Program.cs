using Microsoft.ServiceFabric.Services.Runtime;
using System.Threading;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.API
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            // The ServiceManifest.XML file defines one or more service type names.
            // Registering a service maps a service type name to a .NET type.
            // When Service Fabric creates an instance of this service type,
            // an instance of the class is created in this host process.

            ServiceRuntime.RegisterServiceAsync("PWP.InvoiceCapture.OCR.DataAnalysis.APIType",
            context => new DataAnalysisAPI(context)).GetAwaiter().GetResult();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
