CREATE TABLE [dbo].[LabelSynonyms]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [LabelOfInterestId] INT NOT NULL, 
    [Text] NVARCHAR(100) NOT NULL, 
    [UseInTemplateComparison] BIT NOT NULL,

    CONSTRAINT [FK_LabelSynonyms_LabelsOfInterest] FOREIGN KEY ([LabelOfInterestId]) REFERENCES [LabelsOfInterest]([Id])
)
