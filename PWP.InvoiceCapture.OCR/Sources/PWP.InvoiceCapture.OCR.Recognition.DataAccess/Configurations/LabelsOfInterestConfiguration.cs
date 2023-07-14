using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Configurations
{
    public class LabelsOfInterestConfiguration
    {
        public static void Configure(EntityTypeBuilder<LabelOfInterest> entityBuilder)
        {
            entityBuilder.ToTable("LabelsOfInterest", "dbo");
            entityBuilder.HasKey(label => label.Id);

            entityBuilder
                .Property(label => label.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(label => label.Text)
                .HasColumnName("Text");

            entityBuilder
                .Property(label => label.ExpectedType)
                .HasColumnName("ExpectedType");

            entityBuilder
                .Property(label => label.UseAbsoluteComparison)
                .HasColumnName("UseAbsoluteComparison");

            entityBuilder
                .HasMany(label => label.Keywords)
                .WithOne()
                .HasForeignKey(word => word.LabelOfInterestId);

            entityBuilder
                .HasMany(label => label.Synonyms)
                .WithOne()
                .HasForeignKey(word => word.LabelOfInterestId);

            entityBuilder.Ignore(label => label.MockedErrors);
            entityBuilder.Ignore(label => label.FallBackLabels);
        }
    }
}
