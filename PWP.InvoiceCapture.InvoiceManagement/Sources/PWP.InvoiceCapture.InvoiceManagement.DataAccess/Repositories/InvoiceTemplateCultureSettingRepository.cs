using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories
{
    internal class InvoiceTemplateCultureSettingRepository : IInvoiceTemplateCultureSettingRepository
    {
        public InvoiceTemplateCultureSettingRepository(IDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task CreateAsync(InvoiceTemplateCultureSetting invoiceTemplateCultureSetting, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceTemplateCultureSetting, nameof(invoiceTemplateCultureSetting));
            
            var currentDate = DateTime.UtcNow;

            invoiceTemplateCultureSetting.CreatedDate = currentDate;
            invoiceTemplateCultureSetting.ModifiedDate = currentDate;
           
            using (var context = contextFactory.Create())
            {
                context.Entry(invoiceTemplateCultureSetting).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateAsync(InvoiceTemplateCultureSetting invoiceTemplateCultureSetting, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(invoiceTemplateCultureSetting, nameof(invoiceTemplateCultureSetting));

            using (var context = contextFactory.Create())
            {
                invoiceTemplateCultureSetting.ModifiedDate = DateTime.UtcNow;

                context.Entry(invoiceTemplateCultureSetting).State = EntityState.Modified;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<InvoiceTemplateCultureSetting> GetByTemplateIdAsync(string templateId, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(templateId, nameof(templateId));

            using (var context = contextFactory.Create())
            {
                return await context.InvoiceTemplateCultureSettings.FirstOrDefaultAsync(
                   invoiceTemplateCultureSetting => invoiceTemplateCultureSetting.TemplateId == templateId);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
