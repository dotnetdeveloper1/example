using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.DataAccess.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Repositories
{
    public class LabelOfInterestRepository : ILabelOfInterestRepository
    {
        public LabelOfInterestRepository(IRecognitionDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));
            this.contextFactory = contextFactory;
        }
        public async Task<IEnumerable<LabelOfInterest>> GetAllAsync(CancellationToken cancellationToken)
        {
            using (var context = contextFactory.Create())
            {
                return await context.LabelsOfInterest
                    .Include(label => label.Keywords)
                    .Include(label => label.Synonyms)
                    .ToListAsync(cancellationToken);
            }
        }

        private readonly IRecognitionDatabaseContextFactory contextFactory;
    }
}
