using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class PackConfiguration
    {
        public static void Configure(EntityTypeBuilder<Pack> entityBuilder)
        {
            entityBuilder.ToTable("Pack", "dbo");
            entityBuilder.HasKey(pack => pack.Id);

            entityBuilder
                .Property(pack => pack.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(pack => pack.Name)
                .HasColumnName("Name");

            entityBuilder
                .Property(pack => pack.AllowedDocumentsCount)
                .HasColumnName("AllowedDocumentsCount");

            entityBuilder
                .Property(pack => pack.CurrencyId)
                .HasColumnName("CurrencyId");

            entityBuilder
                .Property(pack => pack.Price)
                .HasColumnName("Price");

            entityBuilder
                .Property(pack => pack.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(pack => pack.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                  .HasOne(pack => pack.Currency)
                  .WithMany();
        }
    }
}
