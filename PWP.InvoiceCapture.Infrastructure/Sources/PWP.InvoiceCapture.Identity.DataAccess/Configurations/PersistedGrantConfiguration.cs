using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class PersistedGrantConfiguration
    {
        public static void Configure(EntityTypeBuilder<PersistedGrant> entityBuilder)
        {
            entityBuilder.ToTable("PersistedGrant", "dbo");

            entityBuilder.HasKey(persistedGrant => persistedGrant.Key);

            entityBuilder
                .Property(persistedGrant => persistedGrant.Key)
                .HasColumnName("Key");

            entityBuilder
                .Property(persistedGrant => persistedGrant.Type)
                .HasColumnName("Type");

            entityBuilder
                .Property(persistedGrant => persistedGrant.SubjectId)
                .HasColumnName("SubjectId");

            entityBuilder
                .Property(persistedGrant => persistedGrant.SessionId)
                .HasColumnName("SessionId");

            entityBuilder
                .Property(persistedGrant => persistedGrant.ClientId)
                .HasColumnName("ClientId");

            entityBuilder
                .Property(persistedGrant => persistedGrant.Description)
                .HasColumnName("Description");

            entityBuilder
               .Property(persistedGrant => persistedGrant.Data)
               .HasColumnName("Data");

            entityBuilder
               .Property(persistedGrant => persistedGrant.CreationTime)
               .HasColumnName("CreationTime");

            entityBuilder
               .Property(persistedGrant => persistedGrant.Expiration)
               .HasColumnName("Expiration");

            entityBuilder
               .Property(persistedGrant => persistedGrant.ConsumedTime)
               .HasColumnName("ConsumedTime");
        }
    }
}
