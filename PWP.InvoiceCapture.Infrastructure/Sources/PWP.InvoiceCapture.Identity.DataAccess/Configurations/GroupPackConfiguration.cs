using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class GroupPackConfiguration
    {
        public static void Configure(EntityTypeBuilder<GroupPack> entityBuilder)
        {
            entityBuilder.ToTable("GroupPack", "dbo");
            entityBuilder.HasKey(groupPack => groupPack.Id);

            entityBuilder
                .Property(groupPack => groupPack.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(groupPack => groupPack.PackId)
                .HasColumnName("PackId");

            entityBuilder
                .Property(groupPack => groupPack.GroupId)
                .HasColumnName("GroupId");

            entityBuilder
                .Property(groupPack => groupPack.UploadedDocumentsCount)
                .HasColumnName("UploadedDocumentsCount");

            entityBuilder
              .Property(groupPack => groupPack.CreatedDate)
              .HasColumnName("StartDate");

            entityBuilder
                .Property(groupPack => groupPack.ModifiedDate)
                .HasColumnName("EndDate");

            entityBuilder
                .Property(groupPack => groupPack.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(groupPack => groupPack.ModifiedDate)
                .HasColumnName("ModifiedDate");
        }
    }
}
