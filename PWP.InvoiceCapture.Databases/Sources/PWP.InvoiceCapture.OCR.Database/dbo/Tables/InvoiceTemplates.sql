CREATE TABLE [dbo].[InvoiceTemplates] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [TenantId] NVARCHAR(MAX) NOT NULL,
    [FormRecognizerModelId] NVARCHAR (100) NULL, 
    [FormRecognizerId] INT NULL,
    [TrainingFileCount] INT NOT NULL DEFAULT 0, 
    [TrainingBlobUri] NVARCHAR(255) NULL, 
    [KeywordCoordinates] NVARCHAR(MAX) NULL, 

    CONSTRAINT [PK_InvoiceTemplates] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InvoiceTemplates_FormRecognizers] FOREIGN KEY ([FormRecognizerId]) REFERENCES [FormRecognizers]([Id])
);

