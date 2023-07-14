UPDATE LabelSynonyms
SET UseInTemplateComparison = 0
WHERE Id IN (
    21, -- Account 
    38, -- Payment due
    40) -- Due on