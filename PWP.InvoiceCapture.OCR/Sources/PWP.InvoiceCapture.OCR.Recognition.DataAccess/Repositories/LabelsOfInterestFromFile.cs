using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.DataAccess.Repositories
{
    internal class LabelsOfInterestFromFile : ILabelOfInterestRepository
    {
        public LabelsOfInterestFromFile(ISerializationService serializationService)
        {
            Guard.IsNotNull(serializationService, nameof(serializationService));
            this.serializationService = serializationService;
        }

        public Task<IEnumerable<LabelOfInterest>> GetAllAsync(CancellationToken cancellationToken)
        {
            if (labels != null)
            {
                return Task.FromResult(labels);
            }

            var root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(root, pathToLabelsFile);
            var labelsContent = File.ReadAllText(path);
            labels = serializationService.Deserialize<List<LabelOfInterest>>(labelsContent);

            return Task.FromResult(labels);
        }

        private IEnumerable<LabelOfInterest> labels = null;
        private readonly ISerializationService serializationService;
        private readonly string pathToLabelsFile = @"Files\labels.json";
    }
}
