using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;

namespace PWP.InvoiceCapture.Identity.DataAccess.Configurations
{
    internal class SqlDatabaseConfiguration
    {
        public static void Configure(EntityTypeBuilder<SqlDatabase> entityBuilder, bool isSqlServer)
        {
            if (isSqlServer)
            {
                entityBuilder.ToView("databases", "sys");
                entityBuilder.HasNoKey();
            }
            else
            {
                entityBuilder.HasKey(e => e.Name);
            }

            entityBuilder
               .Property(database => database.Name)
               .HasColumnName("name");

            entityBuilder
                .Property(database => database.State)
                .HasColumnName("state");

            entityBuilder
                .Property(database => database.StateDescription)
                .HasColumnName("state_desc");
        }
    }
}
