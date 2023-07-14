using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class CurrencyConfiguration
    {
        public static void Configure(EntityTypeBuilder<Currency> entityBuilder)
        {
            entityBuilder.ToTable("Currency", "dbo");
            entityBuilder.HasKey(currency => currency.Id);

            entityBuilder
                .Property(currency => currency.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(currency => currency.Code)
                .HasColumnName("Code");

            entityBuilder
                .Property(currency => currency.Name)
                .HasColumnName("Name");
        }
    }
}
