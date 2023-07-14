CREATE TABLE [dbo].[LabelKeywords]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [LabelOfInterestId] INT NOT NULL, 
    [Text] NVARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_LabelKeywords_LabelsOfInterest] FOREIGN KEY ([LabelOfInterestId]) REFERENCES [LabelsOfInterest]([Id])

)
