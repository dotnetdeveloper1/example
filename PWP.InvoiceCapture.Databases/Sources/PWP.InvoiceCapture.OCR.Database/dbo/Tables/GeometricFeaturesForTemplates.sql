CREATE TABLE [dbo].[GeometricFeaturesForTemplates]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [InvoiceTemplateId] INT NULL, 
    [PixelDensity] FLOAT NULL, 
    [ContourCount] INT NULL, 
    [LineCount] INT NULL, 
    [ConnectedComponentCount] INT NULL, 
    [AverageBlobHeight] FLOAT NULL
)
