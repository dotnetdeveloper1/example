﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Configurations
{
    internal class InvoiceConfiguration
    {
        public static void Configure(EntityTypeBuilder<Invoice> entityBuilder)
        {
            entityBuilder.ToTable("Invoice", "dbo");
            entityBuilder.HasKey(invoice => invoice.Id);

            entityBuilder
                .Property(invoice => invoice.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(invoice => invoice.Name)
                .HasColumnName("Name");

            entityBuilder
                .Property(invoice => invoice.Status)
                .HasColumnName("StatusId");

            entityBuilder
                .Property(invoice => invoice.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(invoice => invoice.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                .Property(invoice => invoice.FileName)
                .HasColumnName("FileName");

            entityBuilder
                .Property(invoice => invoice.FromEmailAddress)
                .HasColumnName("FromEmailAddress");

            entityBuilder
                .Property(invoice => invoice.FileId)
                .HasColumnName("FileId");

            entityBuilder
                .Property(invoice => invoice.FileSourceType)
                .HasColumnName("FileSourceTypeId");

            entityBuilder
               .Property(invoice => invoice.ValidationMessage)
               .HasColumnName("ValidationMessage");

            entityBuilder
                .HasMany(invoice => invoice.InvoiceLines)
                .WithOne()
                .HasForeignKey(invoice => invoice.InvoiceId);

            entityBuilder
                .HasMany(invoice => invoice.InvoiceFields)
                .WithOne()
                .HasForeignKey(invoiceField => invoiceField.InvoiceId);
        }
    }
}
