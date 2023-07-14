using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TroubleShootingApp.Contracts;
using TroubleShootingApp.Options;

namespace TroubleShootingApp.DataAccess
{
    public class FieldsRepository : RepositoryBase<Field>, IFieldsRepository
    {
        public FieldsRepository(IInvoicesDatabaseNameProvider invoicesDatabaseNameProvider, IOptions<InvoiceDbOptions> options)
        {
            Guard.IsNotNull(invoicesDatabaseNameProvider, nameof(invoicesDatabaseNameProvider));
            Guard.IsNotNull(options, nameof(options));
            Guard.IsNotNullOrEmpty(options.Value.ConnectionString, nameof(options.Value.ConnectionString));
            this.invoicesDatabaseNameProvider = invoicesDatabaseNameProvider;
            this.cnStringTemplate = options.Value.ConnectionString;
        }

        public async Task<List<Field>> GetNotDeletedAsync(string tenantId)
        {
            string sql = "SELECT * FROM [dbo].[Field] WHERE [IsDeleted] = 0";
            string dbName = invoicesDatabaseNameProvider.Get(tenantId);
            string connectionString = string.Format(cnStringTemplate, dbName);
            return await base.GetAllAsync(sql, connectionString, (sqlObject) =>
            { 
                return new Field 
                { 
                    Id = sqlObject.Id, 
                    GroupId = sqlObject.GroupId,
                    OrderNumber = sqlObject.OrderNumber,
                    IsProtected = sqlObject.IsProtected,
                    DisplayName = Convert.IsDBNull(sqlObject.DisplayName) ? null : sqlObject.DisplayName,
                    IsDeleted = sqlObject.IsDeleted,
                    Type = (FieldType)sqlObject.Type,
                    DefaultValue = Convert.IsDBNull(sqlObject.DefaultValue) ? null : sqlObject.DefaultValue,
                    IsRequired = sqlObject.IsRequired,
                    MinValue = Convert.IsDBNull(sqlObject.MinValue) ? null : sqlObject.MinValue,
                    MaxValue = Convert.IsDBNull(sqlObject.MaxValue) ? null : sqlObject.MaxValue,
                    MinLength = Convert.IsDBNull(sqlObject.MinLength) ? null : sqlObject.MinLength,
                    MaxLength = Convert.IsDBNull(sqlObject.MaxLength) ? null : sqlObject.MaxLength,
                    CreatedDate = sqlObject.CreatedDate,
                    ModifiedDate = sqlObject.ModifiedDate,
                    TargetFieldType = Convert.IsDBNull(sqlObject.TargetFieldType) ? null : (TargetFieldType)sqlObject.TargetFieldType
                }; 
            });
        }

        private readonly IInvoicesDatabaseNameProvider invoicesDatabaseNameProvider;
        private readonly string cnStringTemplate;
    }
}
