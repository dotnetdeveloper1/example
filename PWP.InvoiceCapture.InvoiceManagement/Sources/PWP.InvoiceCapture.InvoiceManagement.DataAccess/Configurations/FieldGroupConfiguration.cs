using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class FieldGroupConfiguration
    {
        public static void Configure(EntityTypeBuilder<FieldGroup> entityBuilder)
        {
            entityBuilder.ToTable("FieldGroup", "dbo");

            entityBuilder.HasKey(fieldGroup => fieldGroup.Id);

            entityBuilder
                .Property(fieldGroup => fieldGroup.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(fieldGroup => fieldGroup.DisplayName)
                .HasColumnName("DisplayName");

            entityBuilder
                .Property(fieldGroup => fieldGroup.OrderNumber)
                .HasColumnName("OrderNumber");

            entityBuilder
                .Property(fieldGroup => fieldGroup.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(fieldGroup => fieldGroup.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                .HasMany(fieldGroup => fieldGroup.Fields)
                .WithOne()
                .HasForeignKey(field => field.GroupId)
                .IsRequired(false);
        }
    }
}
