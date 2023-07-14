﻿using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace PWP.InvoiceCapture.OCR.Recognition.Service
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

            ServiceRuntime.RegisterServiceAsync("PWP.InvoiceCapture.OCR.Recognition.ServiceType",
                context => new RecognitionService(context)).GetAwaiter().GetResult();

            // Prevents this host process from terminating so services keep running.
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
