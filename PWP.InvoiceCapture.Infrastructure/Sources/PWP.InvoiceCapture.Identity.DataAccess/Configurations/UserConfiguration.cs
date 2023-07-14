using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class UserConfiguration
    {
        public static void Configure(EntityTypeBuilder<User> entityBuilder)
        {
            entityBuilder.ToTable("User", "dbo");
            entityBuilder.HasKey(user => user.Id);

            entityBuilder
                .Property(user => user.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(user => user.Username)
                .HasColumnName("Username");

            entityBuilder
                .Property(user => user.PasswordHash)
                .HasColumnName("PasswordHash");

            entityBuilder
                .Property(user => user.GroupId)
                .HasColumnName("GroupId");

            entityBuilder
                .Property(user => user.IsActive)
                .HasColumnName("IsActive");

            entityBuilder
                .Property(user => user.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(user => user.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                .HasIndex(user => new { user.Id, user.GroupId })
                .IsUnique(true);
        }
    }
}
