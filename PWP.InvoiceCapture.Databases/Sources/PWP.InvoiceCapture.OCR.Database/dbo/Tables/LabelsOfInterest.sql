CREATE TABLE [dbo].[LabelsOfInterest]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[Text] NVARCHAR(100) NOT NULL, 
    [ExpectedType] INT NOT NULL, 
    [UseAbsoluteComparison] BIT NOT NULL
)
