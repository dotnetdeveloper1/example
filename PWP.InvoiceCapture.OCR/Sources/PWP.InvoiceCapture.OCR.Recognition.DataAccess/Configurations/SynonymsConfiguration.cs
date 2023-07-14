using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Configurations
{
    public class SynonymsConfiguration
    {
        public static void Configure(EntityTypeBuilder<LabelSynonym> entityBuilder)
        {
            entityBuilder.GetType();
            
            entityBuilder.ToTable("LabelSynonyms", "dbo");
            entityBuilder.HasKey(synonym => synonym.Id);

            entityBuilder
                .Property(synonym => synonym.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(synonym => synonym.Text)
                .HasColumnName("Text");

            entityBuilder
                .Property(synonym => synonym.UseInTemplateComparison)
                .HasColumnName("UseInTemplateComparison");

            entityBuilder
                .Property(synonym => synonym.LabelOfInterestId)
                .HasColumnName("LabelOfInterestId");

            entityBuilder
                .Property(synonym => synonym.Text)
                .HasColumnName("Text");
        }
    }
}
