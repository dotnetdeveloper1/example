using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.Recognition.Business.Services
{
    internal class PostOcrTemplateMatchingService : IPostOcrTemplateMatchingService
    {
        public PostOcrTemplateMatchingService(ILabelOfInterestRepository labelOfInterestRepository, ILineService lineService) 
        {
            Guard.IsNotNull(labelOfInterestRepository, nameof(labelOfInterestRepository));
            Guard.IsNotNull(lineService, nameof(lineService));

            this.labelOfInterestRepository = labelOfInterestRepository;
            this.lineService = lineService;
        }

        public async Task<Dictionary<string, Coordinate>> GetKeyWordCoordinatesAsync(OCRElements ocrElements, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(ocrElements, nameof(ocrElements));

            var keywords = new Dictionary<string, Coordinate>();

            await SetKeywordsAsync(keywords, ocrElements, cancellationToken);

            if (keywords.Count < minKeywordsCount || IsDistanceBetweenKeywordsNotEnough(keywords))
            {
                // Assuming the biggest word in the invoice is a logo or company name
                SetBiggestWordKeywords(keywords, ocrElements);
            }

            return keywords;
        }

        public InvoiceTemplate GetMatchingTemplate(Dictionary<string, Coordinate> keyWordCoordinates, List<InvoiceTemplate> invoiceTemplates)
        {
            Guard.IsNotNull(keyWordCoordinates, nameof(keyWordCoordinates));
            Guard.IsNotNull(invoiceTemplates, nameof(invoiceTemplates));

            if (keyWordCoordinates.Count < minKeywordsCount)
            {
                return null;
            }

            foreach (var template in invoiceTemplates)
            {
                if (AreSameTemplates(keyWordCoordinates, template.KeyWordCoordinates))
                {
                    return template;
                }
            }

            return null;
        }

        public int MinKeywordsCount => minKeywordsCount;

        private async Task SetKeywordsAsync(Dictionary<string, Coordinate> keywords, OCRElements ocrElements, CancellationToken cancellationToken)
        {
            var keywordsGroupsPerLabel = await GetKeywordsPerLabelAsync(cancellationToken);
            var lines = GetHalfFirstPageLines(ocrElements.Lines);

            foreach (var line in lines)
            {
                var ngrams = lineService.CreateNgramsOfLine(line.Words);

                foreach (var keywordsGroup in keywordsGroupsPerLabel)
                {
                    foreach (var keyword in keywordsGroup)
                    {
                        var key = GetKey(keyword);

                        if (keywords.ContainsKey(key))
                        {
                            continue;
                        }

                        var matchedNGram = ngrams
                            .OrderBy(ngram => ngram.Words.Count)
                            .FirstOrDefault(ngram => ngram.Text.ToLower().RemoveWhiteSpaces().Contains(key));

                        if (matchedNGram != null)
                        {
                            keywords.Add(key, CreateCoordinate(matchedNGram.Words.First()));
                            break;
                        }
                    }
                }
            }
        }

        private void SetBiggestWordKeywords(Dictionary<string, Coordinate> keywords, OCRElements ocrElements)
        {
            var lines = GetHalfFirstPageLines(ocrElements.Lines);

            var biggestWord = lines
                .SelectMany(line => line.Words)
                .OrderByDescending(word => word.Height)
                .ThenByDescending(word => word.Width)
                .FirstOrDefault();

            if (biggestWord == null)
            {
                return;
            }

            var key = GetKey(biggestWord.Text);

            if (keywords.ContainsKey(key))
            {
                return;
            }

            keywords.Add(key, CreateCoordinate(biggestWord));
        }

        private string GetKey(string keyword)
        {
            return keyword.ToLower().RemoveWhiteSpaces();
        }

        private bool AllKeysFound(Dictionary<string, Coordinate> origin, Dictionary<string, Coordinate> target) => 
            origin != null && 
            target != null && 
            origin.Keys.OrderBy(key => key).SequenceEqual(target.Keys.OrderBy(key => key));

        private bool AreSameTemplates(Dictionary<string, Coordinate> template1, Dictionary<string, Coordinate> template2)
        {
            if (template1.Count < minKeywordsCount)
            {
                return false;
            }

            if (!AllKeysFound(template1, template2))
            {
                return false;
            }

            double firstXDistance = 0;
            double firstYDistance = 0;
            
            var isFirstDistanceSet = false;
            var zoomRatio = GetZoomRatio(template1, template2);

            foreach (var key in template1.Keys)
            {
                var xDistance = Math.Abs(template2[key].Left - (template1[key].Left * zoomRatio));
                var yDistance = Math.Abs(template2[key].Top - (template1[key].Top * zoomRatio));

                if (isFirstDistanceSet)
                {
                    if (Math.Abs(xDistance - firstXDistance) > displacementThresholdInPixels || Math.Abs(yDistance - firstYDistance) > displacementThresholdInPixels)
                    {
                        return false;
                    }
                }
                else
                {
                    firstXDistance = xDistance;
                    firstYDistance = yDistance;
                    isFirstDistanceSet = true;
                }
            }

            return true;
        }

        private double GetZoomRatio(Dictionary<string, Coordinate> template1, Dictionary<string, Coordinate> template2)
        {
            var keys = template1.Keys.ToList();
            var results = new List<TemplateKeywordComparisonResult>();

            for (var firstIndex = 0; firstIndex < keys.Count - 1; firstIndex++)
            {
                var firstKey = keys[firstIndex];

                // Calculate diagonal length of the same word (TopLeft - BottomRight)
                results.Add(GetKeywordDiagonalLenght(template1[firstKey], template2[firstKey]));

                for (var secondIndex = firstIndex + 1; secondIndex < keys.Count; secondIndex++)
                {
                    var secondKey = keys[secondIndex];

                    // Calculate lenght of all available by 4 points diagonals: 

                    results.Add(GetTwoKeywordsDiagonalLength(template1[firstKey], template1[secondKey], template2[firstKey], template2[secondKey]));
                    results.Add(GetTwoKeywordsDiagonalLength(template1[secondKey], template1[firstKey], template2[secondKey], template2[firstKey]));
                }
            }

            // Calculate diagonal length of the last element as well
            var lastKey = keys.Last();
            results.Add(GetKeywordDiagonalLenght(template1[lastKey], template2[lastKey]));

            var maxDistanceResult = results
                .OrderByDescending(result => result.Template1Distance)
                .First();

            return maxDistanceResult.Template2Distance / maxDistanceResult.Template1Distance;
        }

        private bool IsDistanceBetweenKeywordsNotEnough(Dictionary<string, Coordinate> keywords) 
        {
            var keys = keywords.Keys.ToList();
            var results = new List<double>();

            for (var firstIndex = 0; firstIndex < keys.Count - 1; firstIndex++)
            {
                var firstKey = keys[firstIndex];

                // Calculate diagonal length of the same word (TopLeft - BottomRight)
                results.Add(GetDistance(keywords[firstKey].Left, keywords[firstKey].Top, keywords[firstKey].Right, keywords[firstKey].Bottom));

                for (var secondIndex = firstIndex + 1; secondIndex < keys.Count; secondIndex++)
                {
                    var secondKey = keys[secondIndex];

                    // Calculate lenght of all available by 4 points diagonals: 

                    results.Add(GetDistance(keywords[firstKey].Left, keywords[firstKey].Top, keywords[secondKey].Right, keywords[secondKey].Bottom));
                    results.Add(GetDistance(keywords[secondKey].Left, keywords[secondKey].Top, keywords[firstKey].Right, keywords[firstKey].Bottom));
                }
            }

            // Calculate diagonal length of the last element as well
            var lastKey = keys.Last();
            results.Add(GetDistance(keywords[lastKey].Left, keywords[lastKey].Top, keywords[lastKey].Right, keywords[lastKey].Bottom));

            var maxDistance = results
                .OrderByDescending(result => result)
                .First();

            return maxDistance < minDistanceBetweenKeywordsInPixels;
        }

        private TemplateKeywordComparisonResult GetKeywordDiagonalLenght(Coordinate template1Coordinate, Coordinate template2Coordinate)
        {
            return new TemplateKeywordComparisonResult
            {
                Template1Distance = GetDistance(template1Coordinate.Left, template1Coordinate.Top, template1Coordinate.Right, template1Coordinate.Bottom),
                Template2Distance = GetDistance(template2Coordinate.Left, template2Coordinate.Top, template2Coordinate.Right, template2Coordinate.Bottom)
            };
        }

        private TemplateKeywordComparisonResult GetTwoKeywordsDiagonalLength(Coordinate template1Key1, Coordinate template1Key2, Coordinate template2Key1, Coordinate template2Key2)
        {
            return new TemplateKeywordComparisonResult 
            {
                Template1Distance = GetDistance(template1Key1.Left, template1Key1.Top, template1Key2.Right, template1Key2.Bottom),
                Template2Distance = GetDistance(template2Key1.Left, template2Key1.Top, template2Key2.Right, template2Key2.Bottom)
            };
        }

        private double GetDistance(float firstX, float firstY, float secondX, float secondY) 
        {
            var width = firstX - secondX;
            var height = firstY - secondY;

            return Math.Sqrt(width * width + height * height);
        }

        private async Task<List<List<string>>> GetKeywordsPerLabelAsync(CancellationToken cancellationToken)
        {
            var labels = await labelOfInterestRepository.GetAllAsync(cancellationToken);

            return labels
                .Where(label => label.Synonyms != null)
                .Select(label => label.Synonyms
                    .Where(synonym => synonym.UseInTemplateComparison)
                    .Select(synonym => synonym.Text)
                    .OrderByDescending(text => text.Length)
                    .ToList())
                .Where(synonymList => synonymList.Count > 0)
                .ToList();
        }

        private Coordinate CreateCoordinate(WordDefinition word)
        {
            return new Coordinate
            {
                Left = word.PageLevelPixelsLeft,
                Top = word.PageLevelPixelsTop,
                Right = word.PageLevelPixelsRight,
                Bottom = word.PageLevelPixelsBottom
            };
        }

        private List<Line> GetHalfFirstPageLines(IEnumerable<Line> lines) 
        {
            return lines
                .Where(line => line.PageNumber == firstPageIndex && line.PageLevelNormalizedBottom <= halfPageNormalizedCoordinate)
                .OrderBy(line => line.DocumentLevelNormalizedTop)
                .ToList();
        }

        private const int minKeywordsCount = 3;
        private const int firstPageIndex = 1;
        private const float halfPageNormalizedCoordinate = 0.5f;
        private const float displacementThresholdInPixels = 10f;
        private const float minDistanceBetweenKeywordsInPixels = 250f;

        private readonly ILabelOfInterestRepository labelOfInterestRepository;
        private readonly ILineService lineService;
    }
}
