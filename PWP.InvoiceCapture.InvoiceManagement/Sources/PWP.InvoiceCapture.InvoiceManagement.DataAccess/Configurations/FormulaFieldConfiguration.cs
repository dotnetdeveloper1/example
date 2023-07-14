using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class FormulaFieldConfiguration
    {
        public static void Configure(EntityTypeBuilder<FormulaField> entityBuilder)
        {
            entityBuilder.ToTable("FormulaField", "dbo");

            entityBuilder.HasKey(invoiceField => invoiceField.Id);

            entityBuilder
                .Property(invoiceField => invoiceField.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(invoiceField => invoiceField.ResultFieldId)
                .HasColumnName("ResultFieldId");

            entityBuilder
                .Property(invoiceField => invoiceField.OperandFieldId)
                .HasColumnName("OperandFieldId");

            entityBuilder
                .Property(invoiceField => invoiceField.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(invoiceField => invoiceField.ModifiedDate)
                .HasColumnName("ModifiedDate");
        }
    }
}
