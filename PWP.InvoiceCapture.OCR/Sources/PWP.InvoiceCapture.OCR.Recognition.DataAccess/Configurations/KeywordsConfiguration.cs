using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Configurations
{
    public class KeywordsConfiguration
    {
        public static void Configure(EntityTypeBuilder<LabelKeyWord> entityBuilder)
        {
            entityBuilder.GetType();
            entityBuilder.ToTable("LabelKeywords", "dbo");
            entityBuilder.HasKey(keyword => keyword.Id);
            entityBuilder
                .Property(keyword => keyword.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entityBuilder
                .Property(keyword => keyword.Text)
                .HasColumnName("Text");

            entityBuilder
                .Property(keyword => keyword.LabelOfInterestId)
                .HasColumnName("LabelOfInterestId");

            entityBuilder
                .Property(keyword => keyword.Text)
                .HasColumnName("Text");
        }
    }
}
