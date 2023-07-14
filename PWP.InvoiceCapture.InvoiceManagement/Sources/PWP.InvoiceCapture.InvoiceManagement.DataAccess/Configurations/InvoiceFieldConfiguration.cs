using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class InvoiceFieldConfiguration
    {
        public static void Configure(EntityTypeBuilder<InvoiceField> entityBuilder)
        {
            entityBuilder.ToTable("InvoiceField", "dbo");

            entityBuilder.HasKey(invoiceField => invoiceField.Id);

            entityBuilder
                .Property(invoiceField => invoiceField.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(invoiceField => invoiceField.InvoiceId)
                .HasColumnName("InvoiceId");

            entityBuilder
                .Property(invoiceField => invoiceField.FieldId)
                .HasColumnName("FieldId");

            entityBuilder
                .Property(invoiceField => invoiceField.Value)
                .HasColumnName("Value");

            entityBuilder
                .Property(invoiceField => invoiceField.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(invoiceField => invoiceField.ModifiedDate)
                .HasColumnName("ModifiedDate");
        }
    }
}
