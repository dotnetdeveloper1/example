namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IPasswordHashService
    {
        string GetHash(string password);
    }
}
