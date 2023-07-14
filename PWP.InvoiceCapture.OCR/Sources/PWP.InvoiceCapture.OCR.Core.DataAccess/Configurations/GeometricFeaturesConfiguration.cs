using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.OCR.Core.Models;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Configurations
{
    public class GeometricFeaturesConfiguration
    {
        public static void Configure(EntityTypeBuilder<GeometricFeatureCollection> entityBuilder)
        {
            entityBuilder.ToTable("GeometricFeaturesForTemplates","dbo");
            entityBuilder.HasKey(feature => feature.Id);

            entityBuilder
                .Property(feature => feature.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(feature => feature.AverageBlobHeight)
                .HasColumnName("AverageBlobHeight");

            entityBuilder
                .Property(feature => feature.ConnectedComponentCount)
                .HasColumnName("ConnectedComponentCount");

            entityBuilder
                .Property(feature => feature.ContourCount)
                .HasColumnName("ContourCount");


            entityBuilder
                .Property(feature => feature.LineCount)
                .HasColumnName("LineCount");

            entityBuilder
                .Property(feature => feature.PixelDensity)
                .HasColumnName("PixelDensity");
        }
    }
}
