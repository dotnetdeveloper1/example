using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class CulturesConfiguration
    {
        public static void Configure(EntityTypeBuilder<Culture> entityBuilder)
        {
            entityBuilder.ToTable("Culture", "dbo");
            entityBuilder.HasKey(culture => culture.Id);

            entityBuilder
                .Property(culture => culture.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(culture => culture.Name)
                .HasColumnName("Name");

            entityBuilder
                .Property(culture => culture.EnglishName)
                .HasColumnName("EnglishName");

            entityBuilder
                .Property(culture => culture.NativeName)
                .HasColumnName("NativeName");
        }
    }
}
