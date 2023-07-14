CREATE TABLE [dbo].[InvoiceProcessingResult]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [InvoiceId] INT NOT NULL, 
    [ProcessingTypeId] INT NOT NULL, 
    [TemplateId] NVARCHAR(MAX) NOT NULL,
    [TrainingFileCount] INT CONSTRAINT [DF_InvoiceProcessingResult_TrainingFileCount] DEFAULT (0) NOT NULL,
    [InitialDataAnnotationFileId] NVARCHAR(MAX) NOT NULL, 
    [DataAnnotationFileId] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_InvoiceProcessingResult_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_InvoiceProcessingResult_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_InvoiceProcessingResult] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_InvoiceProcessingResult_Invoice_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoice]([Id])
)
