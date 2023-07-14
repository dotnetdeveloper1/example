namespace PWP.InvoiceCapture.Identity.Business.Contract.Models
{
    public class AuthenticationServerOptions
    {
        public int AccessTokenLifetimeInSeconds { get; set; }
        public int RefreshTokenLifetimeInSeconds { get; set; }
        public string SigningKey { get; set; }
    }
}
