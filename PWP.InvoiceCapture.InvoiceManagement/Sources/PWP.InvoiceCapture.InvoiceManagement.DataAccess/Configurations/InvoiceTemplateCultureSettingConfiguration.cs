using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class InvoiceTemplateCultureSettingConfiguration
    {
        public static void Configure(EntityTypeBuilder<InvoiceTemplateCultureSetting> entityBuilder)
        {
            entityBuilder.ToTable("InvoiceTemplateCultureSetting", "dbo");
            entityBuilder.HasKey(invoiceTemplateCultureSetting => invoiceTemplateCultureSetting.Id);

            entityBuilder
                .Property(invoiceTemplateCultureSetting => invoiceTemplateCultureSetting.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(invoiceTemplateCultureSetting => invoiceTemplateCultureSetting.CultureName)
                .HasColumnName("CultureName");
            
            entityBuilder
               .Property(invoiceTemplateCultureSetting => invoiceTemplateCultureSetting.TemplateId)
               .HasColumnName("TemplateId");

            entityBuilder
                .Property(invoiceTemplateCultureSetting => invoiceTemplateCultureSetting.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(invoiceTemplateCultureSetting => invoiceTemplateCultureSetting.ModifiedDate)
                .HasColumnName("ModifiedDate");
        }
    }
}
