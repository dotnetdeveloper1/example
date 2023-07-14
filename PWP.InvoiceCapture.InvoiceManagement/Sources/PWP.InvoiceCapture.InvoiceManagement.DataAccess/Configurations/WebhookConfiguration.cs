using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class WebhookConfiguration
    {
        public static void Configure(EntityTypeBuilder<Webhook> entityBuilder)
        {
            entityBuilder.ToTable("WebHook", "dbo");

            entityBuilder.HasKey(webhook => webhook.Id);

            entityBuilder
                .Property(webhook => webhook.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
               .Property(webhook => webhook.TriggerType)
               .HasColumnName("TriggerType");

            entityBuilder
              .Property(webhook => webhook.Url)
              .HasColumnName("Url");

            entityBuilder
              .Property(webhook => webhook.CreatedDate)
              .HasColumnName("CreatedDate");

            entityBuilder
              .Property(webhook => webhook.ModifiedDate)
              .HasColumnName("ModifiedDate");
        }
    }
}
