using PWP.InvoiceCapture.Core.Contracts;
using System;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.IntegrationTests.Fakes
{
    internal class FakeApplicationContext : IApplicationContext
    {
        public string TenantId
        {
            get { return "Default"; }   
            set { throw new NotImplementedException(); }  
        }

        public string Culture 
        {
            get { return "en-US"; }
            set => throw new NotImplementedException(); 
        }
    }
}
