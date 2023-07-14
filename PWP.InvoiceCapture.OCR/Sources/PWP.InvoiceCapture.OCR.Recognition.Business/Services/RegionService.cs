using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class RegionService : IRegionService
    {
        public bool DoesWordBelong(WordDefinition word,Region region)
        {
            if (!region.Initialized)
            {
                throw new Exception("Region is not initialized");
            }

            var xDistanceLeft = Math.Abs(word.DocumentLevelNormalizedLeft - region.Left);
            var xDistanceRight = Math.Abs(word.DocumentLevelNormalizedRight - region.Right);
            var yDistance = Math.Abs(word.DocumentLevelNormalizedTop - region.Bottom);

            return (xDistanceLeft  < leftDistanceThreshold   && yDistance < topDistanceThreshold) || 
                   (xDistanceRight < righttDistanceThreshold && yDistance < topDistanceThreshold);
        }

        private const float leftDistanceThreshold = 0.005f;
        private const float topDistanceThreshold = 0.025f;
        private const float righttDistanceThreshold = 0.005f;
    }
}
