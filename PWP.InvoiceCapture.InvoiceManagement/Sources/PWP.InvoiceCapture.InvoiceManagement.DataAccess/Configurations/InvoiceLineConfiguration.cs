using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class InvoiceLineConfiguration
    {
        public static void Configure(EntityTypeBuilder<InvoiceLine> entityBuilder)
        {
            entityBuilder.ToTable("InvoiceLine", "dbo");

            entityBuilder.HasKey(invoiceLine => invoiceLine.Id);

            entityBuilder
                .Property(invoiceLine => invoiceLine.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(invoiceLine => invoiceLine.InvoiceId)
                .HasColumnName("InvoiceId");

            entityBuilder
                .Property(invoiceLine => invoiceLine.OrderNumber)
                .HasColumnName("OrderNumber");

            entityBuilder
                .Property(invoiceLine => invoiceLine.Number)
                .HasColumnName("Number");

            entityBuilder
                .Property(invoiceLine => invoiceLine.Description)
                .HasColumnName("Description");

            entityBuilder
                .Property(invoiceLine => invoiceLine.Quantity)
                .HasColumnName("Quantity");

            entityBuilder
                .Property(invoiceLine => invoiceLine.Price)
                .HasColumnName("Price");

            entityBuilder
                .Property(invoiceLine => invoiceLine.Total)
                .HasColumnName("Total");

            entityBuilder
                .Property(invoiceLine => invoiceLine.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(invoiceLine => invoiceLine.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                .HasIndex(invoiceLine => new { invoiceLine.InvoiceId, invoiceLine.OrderNumber })
                .IsUnique(true);
        }
    }
}
