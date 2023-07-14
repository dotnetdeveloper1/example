using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class ApplicationClientConfiguration
    {
        public static void Configure(EntityTypeBuilder<ApplicationClient> entityBuilder)
        {
            entityBuilder.ToTable("Client", "dbo");
            entityBuilder.HasKey(client => client.Id);

            entityBuilder
                .Property(client => client.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(client => client.ClientId)
                .HasColumnName("ClientId");

            entityBuilder
                .Property(client => client.SecretHash)
                .HasColumnName("SecretHash");

            entityBuilder
                .Property(client => client.IsActive)
                .HasColumnName("IsActive");

            entityBuilder
                .Property(client => client.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(client => client.ModifiedDate)
                .HasColumnName("ModifiedDate");
        }
    }
}
