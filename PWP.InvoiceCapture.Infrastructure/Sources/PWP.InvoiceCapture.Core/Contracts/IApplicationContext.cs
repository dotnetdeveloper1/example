namespace PWP.InvoiceCapture.Core.Contracts
{
    public interface IApplicationContext
    {
        string TenantId { get; set; }
        string Culture { get; set; }
    }
}
