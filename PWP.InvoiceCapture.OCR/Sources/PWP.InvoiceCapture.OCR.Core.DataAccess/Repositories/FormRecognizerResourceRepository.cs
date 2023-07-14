using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.OCR.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories
{
    public class FormRecognizerResourceRepository : IFormRecognizerResourceRepository
    {
        public FormRecognizerResourceRepository(IDatabaseContextFactory contextFactory) 
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<FormRecognizerResource> GetActiveAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.FormRecognizers.FirstOrDefaultAsync(recognizer => recognizer.IsActive, cancellationToken);
            }
        }

        public async Task DisableAsync(int id, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(id, nameof(id));

            using (var context = contextFactory.Create())
            {
                var formRecognizer = new FormRecognizerResource
                {
                    Id = id,
                    IsActive = false,
                    ModifiedDate = DateTime.UtcNow
                };

                context.Entry(formRecognizer).State = EntityState.Modified;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private readonly IDatabaseContextFactory contextFactory;
    }
}
