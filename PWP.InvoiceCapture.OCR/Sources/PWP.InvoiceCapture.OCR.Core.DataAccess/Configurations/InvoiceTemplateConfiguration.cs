using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using PWP.InvoiceCapture.OCR.Core.Models;
using System.Collections.Generic;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Configurations
{
    public class InvoiceTemplateConfiguration
    {
        public static void Configure(EntityTypeBuilder<InvoiceTemplate> entityBuilder)
        {
            entityBuilder.ToTable("InvoiceTemplates", "dbo");
            entityBuilder.HasKey(template => template.Id);

            entityBuilder
                .Property(template => template.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(template => template.FormRecognizerModelId)
                .HasColumnName("FormRecognizerModelId");

            entityBuilder
                .Property(template => template.FormRecognizerId)
                .HasColumnName("FormRecognizerId");

            entityBuilder
                .Property(template => template.TrainingBlobUri)
                .HasColumnName("TrainingBlobUri");

            entityBuilder
                .Property(template => template.TrainingFileCount)
                .HasColumnName("TrainingFileCount");

            entityBuilder
                .HasOne(template => template.GeometricFeatures)
                .WithOne(feature=>feature.InvoiceTemplate)
                .HasForeignKey<GeometricFeatureCollection>(feature => feature.InvoiceTemplateId);

            entityBuilder.Property(template => template.KeyWordCoordinates)
                .HasConversion(
                    keyWordCoordinates => JsonConvert.SerializeObject(keyWordCoordinates, Formatting.None),
                    keyWordCoordinates => JsonConvert.DeserializeObject<Dictionary<string, Coordinate>>(keyWordCoordinates)
                );
        }
    }
}
