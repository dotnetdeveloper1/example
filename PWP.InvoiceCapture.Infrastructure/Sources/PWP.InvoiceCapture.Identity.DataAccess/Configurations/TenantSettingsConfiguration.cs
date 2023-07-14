using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class TenantSettingsConfiguration
    {
        public static void Configure(EntityTypeBuilder<TenantSetting> entityBuilder)
        {
            entityBuilder.ToTable("TenantSetting", "dbo");
            entityBuilder.HasKey(tenantSetting => tenantSetting.Id);

            entityBuilder
                .Property(tenantSetting => tenantSetting.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(tenantSetting => tenantSetting.TenantId)
                .HasColumnName("TenantId");

            entityBuilder
                .Property(tenantSetting => tenantSetting.CultureId)
                .HasColumnName("CultureId");

            entityBuilder
                .Property(tenantSetting => tenantSetting.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(tenantSetting => tenantSetting.ModifiedDate)
                .HasColumnName("ModifiedDate");
        }
    }
}
