using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class GroupPlanConfiguration
    {
        public static void Configure(EntityTypeBuilder<GroupPlan> entityBuilder)
        {
            entityBuilder.ToTable("GroupPlan", "dbo");
            entityBuilder.HasKey(groupPlan => groupPlan.Id);

            entityBuilder
                .Property(groupPlan => groupPlan.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(groupPlan => groupPlan.PlanId)
                .HasColumnName("PlanId");

            entityBuilder
                .Property(groupPlan => groupPlan.GroupId)
                .HasColumnName("GroupId");

            entityBuilder
                .Property(groupPlan => groupPlan.UploadedDocumentsCount)
                .HasColumnName("UploadedDocumentsCount");

            entityBuilder
                .Property(groupPlan => groupPlan.IsRenewalCancelled)
                .HasColumnName("IsRenewalCancelled");

            entityBuilder
              .Property(groupPlan => groupPlan.CreatedDate)
              .HasColumnName("StartDate");

            entityBuilder
                .Property(groupPlan => groupPlan.ModifiedDate)
                .HasColumnName("EndDate");

            entityBuilder
                .Property(groupPlan => groupPlan.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(groupPlan => groupPlan.ModifiedDate)
                .HasColumnName("ModifiedDate");
        }
    }
}
