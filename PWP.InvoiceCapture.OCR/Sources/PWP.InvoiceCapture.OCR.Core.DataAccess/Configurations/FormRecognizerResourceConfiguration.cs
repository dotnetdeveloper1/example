using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.OCR.Core.Models;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Configurations
{
    public class FormRecognizerResourceConfiguration
    {
        public static void Configure(EntityTypeBuilder<FormRecognizerResource> entityBuilder)
        {
            entityBuilder.ToTable("FormRecognizers", "dbo");
            entityBuilder.HasKey(recognizer => recognizer.Id);

            entityBuilder
                .Property(recognizer => recognizer.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(recognizer => recognizer.IsActive)
                .HasColumnName("IsActive");

            entityBuilder
               .Property(recognizer => recognizer.CreatedDate)
               .HasColumnName("CreatedDate");

            entityBuilder
               .Property(recognizer => recognizer.ModifiedDate)
               .HasColumnName("ModifiedDate");
        }
    }
}
