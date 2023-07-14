using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class InvoicePageConfiguration
    {
        public static void Configure(EntityTypeBuilder<InvoicePage> entityBuilder)
        {
            entityBuilder.ToTable("InvoicePage", "dbo");
            entityBuilder.HasKey(contact => contact.Id);

            entityBuilder
                .Property(contact => contact.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(contact => contact.InvoiceId)
                .HasColumnName("InvoiceId");

            entityBuilder
                .Property(contact => contact.Number)
                .HasColumnName("Number");

            entityBuilder
                .Property(contact => contact.ImageFileId)
                .HasColumnName("ImageFileId");

            entityBuilder
                .Property(contact => contact.Height)
                .HasColumnName("Height");

            entityBuilder
                .Property(contact => contact.Width)
                .HasColumnName("Width");
        }
    }
}
