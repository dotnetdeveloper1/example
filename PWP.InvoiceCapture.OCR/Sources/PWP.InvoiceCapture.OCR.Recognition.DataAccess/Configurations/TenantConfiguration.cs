using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Configurations
{
    public class TenantConfiguration
    {
        public static void Configure(EntityTypeBuilder<Tenant> entityBuilder)
        {
            entityBuilder.ToTable("LabelsOfInterest", "dbo");
            entityBuilder.HasKey(label => label.Id);

            entityBuilder
                .Property(label => label.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(label => label.TenantId)
                .HasColumnName("TenantId");
        }
    }
}
