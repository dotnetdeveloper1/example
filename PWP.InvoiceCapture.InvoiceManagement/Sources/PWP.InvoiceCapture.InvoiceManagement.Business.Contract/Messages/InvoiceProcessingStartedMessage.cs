﻿using PWP.InvoiceCapture.Core.ServiceBus.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Messages
{
    public class InvoiceProcessingStartedMessage : ServiceBusMessageBase
    {
        public int InvoiceId { get; set; }
    }
}
