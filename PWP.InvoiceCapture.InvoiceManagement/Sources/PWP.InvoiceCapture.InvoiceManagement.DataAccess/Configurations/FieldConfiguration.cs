using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class FieldConfiguration
    {
        public static void Configure(EntityTypeBuilder<Field> entityBuilder)
        {
            entityBuilder.ToTable("Field", "dbo");

            entityBuilder.HasKey(field => field.Id);

            entityBuilder
                .Property(field => field.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
               .Property(field => field.IsProtected)
               .HasColumnName("IsProtected");

            entityBuilder
                .Property(field => field.DisplayName)
                .HasColumnName("DisplayName");

            entityBuilder
                .Property(field => field.DefaultValue)
                .HasColumnName("DefaultValue");

            entityBuilder
                .Property(field => field.Type)
                .HasColumnName("Type");

            entityBuilder
                .Property(field => field.TargetFieldType)
                .HasColumnName("TargetFieldType");

            entityBuilder
                .Property(field => field.OrderNumber)
                .HasColumnName("OrderNumber");

            entityBuilder
               .Property(field => field.MinValue)
               .HasColumnName("MinValue");

            entityBuilder
               .Property(field => field.MaxValue)
               .HasColumnName("MaxValue");

            entityBuilder
               .Property(field => field.MinLength)
               .HasColumnName("MinLength");

            entityBuilder
               .Property(field => field.MaxLength)
               .HasColumnName("MaxLength");

            entityBuilder
               .Property(field => field.IsRequired)
               .HasColumnName("IsRequired");

            entityBuilder
                .Property(field => field.Formula)
                .HasColumnName("Formula");

            entityBuilder
                .Property(field => field.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(field => field.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
               .Property(field => field.GroupId)
               .HasColumnName("GroupId");
        }
    }
}
