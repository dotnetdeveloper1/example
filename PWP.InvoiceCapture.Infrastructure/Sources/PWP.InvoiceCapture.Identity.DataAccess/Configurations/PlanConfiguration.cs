using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class PlanConfiguration
    {
        public static void Configure(EntityTypeBuilder<Plan> entityBuilder)
        {
            entityBuilder.ToTable("Plan", "dbo");
            entityBuilder.HasKey(plan => plan.Id);

            entityBuilder
                .Property(plan => plan.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(plan => plan.Name)
                .HasColumnName("Name");

            entityBuilder
                .Property(plan => plan.Type)
                .HasColumnName("Type");

            entityBuilder
                .Property(plan => plan.AllowedDocumentsCount)
                .HasColumnName("AllowedDocumentsCount");

            entityBuilder
                .Property(plan => plan.CurrencyId)
                .HasColumnName("CurrencyId");

            entityBuilder
                .Property(plan => plan.Price)
                .HasColumnName("Price");

            entityBuilder
                .Property(plan => plan.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(plan => plan.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                  .HasOne(plan => plan.Currency)
                  .WithMany();
        }
    }
}
