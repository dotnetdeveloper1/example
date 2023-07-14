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
    internal class LabelExtractorService : ILabelExtractorService
    {
        public LabelExtractorService(ILabelOfInterestRepository labelsRepository, ILineService lineService, ILabelOfInterestService labelService)
        {   
            this.lineService = lineService;
            this.labelService = labelService;
            this.labelsRepository = labelsRepository;
        }

        public async Task<Dictionary<LabelOfInterest, List<WordGroup>>> ExtractLabelsAsync(OCRElements ocrElements,CancellationToken cancellationToken)
        {
            var lines = ocrElements.Lines.ToList();
            var candidateList = new Dictionary<LabelOfInterest, List<WordGroup>>();
            var labels = await labelsRepository.GetAllAsync(cancellationToken);
            
            // Create word groupings (Merge close and relevant words into a single cluster)
            // Only horizontal groupings are possible
            CreateWordGroupings(lines, labels.ToList());
            
            for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
            {
                var line = lines[lineIndex];
                var words = line.ProcessedWords;
                for (var index = 0; index < words.Count; index++)
                {
                    var currentWord = words[index];
                    AnalyzeWordForLabelAndMatchValue(currentWord, labels, candidateList, words, lines, index, ocrElements.Words.ToList());
                }
            }

            return candidateList;
        }

        public async Task<Dictionary<LabelOfInterest, List<WordGroup>>> ExtractLabelsFromPairsAsync(OCRElements ocrElements, CancellationToken cancellationToken)
        {
            var foundPairs = new Dictionary<LabelOfInterest, List<WordGroup>>();
            var labels = await labelsRepository.GetAllAsync(cancellationToken);
            // If a line only contains 2 processed words it might be a key value pair
            // In this case we disregard the distance between the words and add them as candidates
            foreach (var line in ocrElements.Lines)
            {
                if (line.ProcessedWords.Count == 2)   
                {
                    foreach (var label in labels)
                    {
                        if (labelService.DoesWordConform(label, line.ProcessedWords[0].Text)
                            && IsWordASuitableValueForLabel(line.ProcessedWords[1], label))
                        {
                            AddKeyToCandidateList(foundPairs, label);
                            var wordGroup = new WordGroup(line.ProcessedWords[1]);
                            foundPairs[label].Add(wordGroup);
                        }
                    }
                }
            }
            return foundPairs;
        }

        public async Task<Dictionary<LabelOfInterest, List<WordGroup>>> ExtractLabelsFromNGramsAsync(OCRElements ocrElements, CancellationToken cancellationToken)
        {
            var foundLabels = new Dictionary<LabelOfInterest, List<WordGroup>>();
            var allLabels = await labelsRepository.GetAllAsync(cancellationToken);
            foreach (var line in ocrElements.Lines)
            {
                var ngrams = lineService.CreateNgramsOfLine(line.Words);
                foreach (var ngram in ngrams)
                {
                    foreach (var label in allLabels)
                    {
                        // This checks if next word is suitable
                        // for composite words it will not find the value
                        // will be better to replace with a check on ngrams starting with next word
                        if (labelService.DoesWordConform(label, ngram.Text)
                            && IsWordASuitableValueForLabel(ngram.NextWord, label))
                        {
                            AddKeyToCandidateList(foundLabels, label);
                            var wordGroup = new WordGroup(ngram.NextWord);
                            foundLabels[label].Add(wordGroup);
                        }
                    }
                }
            }

            return foundLabels;
        }

        private WordDefinition GetNextWordInLine(List<WordDefinition> words, int index)
        {
            if (words.Count <= index + 1)
            {
                return null;
            }

            var nextWord = words[index + 1];
            var nextWordText = nextWord.Text.Trim();

            // 1. Check if next word is a label or a price value (if it's a label, don't match them, value is probably down below)    
            if (dollarSignMarkers.Contains(nextWordText) && words.Count > index + 2)
            {
                nextWord = words[index + 2];
            }

            return nextWord;
        }

        private bool IsWordASuitableValueForLabel(WordDefinition word, LabelOfInterest label)
        {
            if (word == null)
            {
                return false;
            }

            var isWordSuitable = 
                !word.IsLabel &&                               // if word is a label it can't be a value
                (word.DataType == label.ExpectedType ||
                label.ExpectedType == DataType.Undefined) &&
                word.ValueProbability > candidateMatchingProbabilityThreshold;

            return isWordSuitable;
        }

        private WordDefinition GetCandidateFromFollowingLines(WordDefinition word, List<Line> lines, LabelOfInterest label)
        {
            var nextLine = word.LineNo + 1;
            for (var i = 0; i < maxLineSearchCount; i++)
            {
                if (lines.Count <= nextLine)
                {
                    break;
                }

                var match = lineService.GetIntersectingWordOfLine(word, lines[nextLine]);
                if (IsWordASuitableValueForLabel(match, label))
                {
                    return match;
                }

                nextLine++;
            }

            return null;
        }

        private void AddKeyToCandidateList(Dictionary<LabelOfInterest, List<WordGroup>> candidateList, LabelOfInterest key)
        {
            if (!candidateList.Keys.Contains(key))
            {
                candidateList.Add(key, new List<WordGroup>());
            }
        }

        private List<WordDefinition> GetWordsByIds(List<WordDefinition> words, List<int> ids) => words.Where(word => ids.Contains(word.Id)).ToList();

        private void AnalyzeWordForLabelAndMatchValue(WordDefinition currentWord, IEnumerable<LabelOfInterest> labels,
            Dictionary<LabelOfInterest, List<WordGroup>> candidateList, List<WordDefinition> wordsInThisLine, List<Line> lines, int index,List<WordDefinition> allWords)
        {
            foreach (var label in labels)
            {
                if (labelService.DoesWordConform(label, currentWord.Text))
                {
                    AddKeyToCandidateList(candidateList, label);
                    // check if next word in this line makes sense, otherwise check the word on the next line
                    var nextWord = GetNextWordInLine(wordsInThisLine, index);
                    if (IsWordASuitableValueForLabel(nextWord, label))
                    {
                        var individualWords = GetWordsByIds(allWords, nextWord.IndividualWordIds);
                        var wordGroup = new WordGroup(individualWords);
                        candidateList[label].Add(wordGroup);
                    }
                    else
                    {
                        // Check further lines for a value suitable for this label
                        var match = GetCandidateFromFollowingLines(currentWord, lines, label);
                        if (match != null)
                        {
                            var individualWords = GetWordsByIds(allWords, match.IndividualWordIds);
                            var wordGroup = new WordGroup(individualWords);
                            candidateList[label].Add(wordGroup);
                        }
                    }
                }
            }
        }

        private void CalculateWordSpacingsInLine(Line line, List<LabelOfInterest> labels)
        {
            for (var i = 1; i < line.Words.Count; i++)
            {
                var currentWord = line.Words[i];
                var previousWord = line.Words[i - 1];

                if (labels.Any(label => labelService.DoesWordConform(label, currentWord.Text)))
                {
                    currentWord.IsLabel = true;
                }

                var space = Math.Abs(currentWord.DocumentLevelNormalizedLeft - previousWord.DocumentLevelNormalizedRight);
                line.Spaces.Add(space);
            }
        }

        private bool IsWordSpacingClose(float space, float minSpace) => space <= (minSpace * wordBreakThreshold) && space < absoluteBreakDistance;
        
        private void ExpandWordIntoNext(WordDefinition lastWord, WordDefinition currentWord)
        {
            lastWord.Text += " " + currentWord.Text;
            lastWord.DocumentLevelNormalizedRight = currentWord.DocumentLevelNormalizedRight;
            lastWord.DocumentLevelNormalizedBottom = currentWord.DocumentLevelNormalizedBottom;
            lastWord.DataType = lastWord.Text.GetDataType();
            lastWord.IndividualWordIds.Add(currentWord.Id);
        }

        private void AssignLabelFlags(List<LabelOfInterest>  labels, WordDefinition previousWord, WordDefinition currentWord,float space, float minSpace, Line line, int i)
        {
            var compoundLabel = false;
            if (labels.Any(label => labelService.DoesWordConform(label, previousWord.Text + " " + currentWord.Text)))
            {
                compoundLabel = true;
            }

            if (IsWordSpacingClose(space, minSpace) && (!previousWord.IsLabel || compoundLabel))
            {
                var lastWord = line.ProcessedWords.Last();
                ExpandWordIntoNext(lastWord, currentWord);

                if (compoundLabel && line.Words.Count > (i + 1))
                {
                    currentWord.IsLabel = true;
                    previousWord.IsLabel = true;
                    var nextWordInLine = line.Words[i + 1];

                    if (labels.Any(label => labelService.DoesWordConform(label, lastWord.Text + " " + nextWordInLine.Text)))
                    {
                        ExpandWordIntoNext(lastWord, nextWordInLine);
                        nextWordInLine.IsLabel = true;
                    }
                }
            }
            else
            {
                line.ProcessedWords.Add((WordDefinition)currentWord.Clone());
                line.ProcessedWords.Last().IndividualWordIds.Add(currentWord.Id);
            }
        }

        private void CreateWordGroupings(List<Line> lines, List<LabelOfInterest> labels)
        {
            foreach (var line in lines)
            {
                line.Spaces = new SortedSet<float>();
                line.ProcessedWords = new List<WordDefinition>();

                // Merge close words
                // dont merge labels with other words unless they form a new label when merged
                if (line.Words.Count > 1)
                {
                    if (labels.Any(label => labelService.DoesWordConform(label, line.Words[0].Text)))
                    {
                        line.Words[0].IsLabel = true;
                    }

                    // Calculate average space between words in line
                    CalculateWordSpacingsInLine(line,labels);
                    var minSpace = line.Spaces.Min;
                    line.ProcessedWords.Add((WordDefinition)line.Words[0].Clone());
                    line.ProcessedWords[0].IndividualWordIds.Add(line.Words[0].Id);
                    for (var i = 1; i < line.Words.Count; i++)
                    {
                        var currentWord = line.Words[i];
                        var previousWord = line.Words[i - 1];
                        var space = (currentWord.DocumentLevelNormalizedLeft - previousWord.DocumentLevelNormalizedRight);
                        AssignLabelFlags(labels, previousWord, currentWord, space, minSpace, line,i);
                    }
                }
            }
        }
        
        private readonly ILineService lineService;
        private readonly ILabelOfInterestService labelService;
        private readonly ILabelOfInterestRepository labelsRepository;
        private const float wordBreakThreshold = 2.2f;
        private const float absoluteBreakDistance = 0.05f;
        private const float candidateMatchingProbabilityThreshold = 0.15f;
        private const int maxLineSearchCount = 4;
        private static readonly string[] dollarSignMarkers = new string[] { "USD", "US", "$" };
    }
}
