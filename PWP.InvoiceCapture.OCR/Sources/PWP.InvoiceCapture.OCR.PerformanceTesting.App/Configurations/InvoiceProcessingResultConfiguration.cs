using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Configurations
{
    internal class InvoiceProcessingResultConfiguration
    {
        public static void Configure(EntityTypeBuilder<InvoiceProcessingResult> entityBuilder)
        {
            entityBuilder.ToTable("InvoiceProcessingResult", "dbo");
            entityBuilder.HasKey(processingResult => processingResult.Id);

            entityBuilder
                .Property(processingResult => processingResult.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
               .Property(processingResult => processingResult.InvoiceId)
               .HasColumnName("InvoiceId");

            entityBuilder
               .Property(processingResult => processingResult.ProcessingType)
               .HasColumnName("ProcessingTypeId");

            entityBuilder
                .Property(processingResult => processingResult.TemplateId)
                .HasColumnName("TemplateId");

            entityBuilder
                .Property(processingResult => processingResult.DataAnnotationFileId)
                .HasColumnName("DataAnnotationFileId");

            entityBuilder
                .Property(processingResult => processingResult.InitialDataAnnotationFileId)
                .HasColumnName("InitialDataAnnotationFileId");

            entityBuilder
                .Property(processingResult => processingResult.CreatedDate)
                .HasColumnName("CreatedDate");

            entityBuilder
                .Property(processingResult => processingResult.ModifiedDate)
                .HasColumnName("ModifiedDate");

            entityBuilder
                .HasOne(processingResult => processingResult.Invoice)
                .WithMany()
                .HasForeignKey(processingResult => processingResult.InvoiceId);

            entityBuilder
                .Ignore(processingResult => processingResult.DataAnnotation);
        }
    }
}
