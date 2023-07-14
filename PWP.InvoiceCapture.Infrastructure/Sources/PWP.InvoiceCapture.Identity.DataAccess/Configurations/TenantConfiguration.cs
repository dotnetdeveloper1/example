using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class TenantConfiguration
    {
        public static void Configure(EntityTypeBuilder<Tenant> entityBuilder)
        {
            entityBuilder.ToTable("Tenant", "dbo");
            entityBuilder.HasKey(tenant => tenant.Id);

            entityBuilder
                .Property(tenant => tenant.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(tenant => tenant.Name)
                .HasColumnName("Name");
          
            entityBuilder
               .Property(tenant => tenant.GroupId)
               .HasColumnName("GroupId");

            entityBuilder
                .Property(tenant => tenant.Status)
                .HasColumnName("Status");

            entityBuilder
                .Property(tenant => tenant.DatabaseName)
                .HasColumnName("DatabaseName");
            
            entityBuilder
                .Property(tenant => tenant.DocumentUploadEmail)
                .HasColumnName("DocumentUploadEmail");

            entityBuilder
                .Property(tenant => tenant.IsActive)
                .HasColumnName("IsActive");

            entityBuilder
                .Property(tenant => tenant.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(tenant => tenant.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                .Property(tenant => tenant.GroupId)
                .HasColumnName("GroupId");  
        }
    }
}
