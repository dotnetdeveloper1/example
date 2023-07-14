using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class GroupsConfiguration
    {
        public static void Configure(EntityTypeBuilder<Group> entityBuilder)
        {
            entityBuilder.ToTable("Group", "dbo");
            entityBuilder.HasKey(group => group.Id);

            entityBuilder
                .Property(group => group.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(group => group.Name)
                .HasColumnName("Name");

            entityBuilder
                .Property(group => group.ParentGroupId)
                .HasColumnName("ParentGroupId");

            entityBuilder
                .Property(group => group.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(group => group.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                .HasMany(group => group.Tenants)
                .WithOne()
                .HasForeignKey(tenant => tenant.GroupId);
        }
    }
}
