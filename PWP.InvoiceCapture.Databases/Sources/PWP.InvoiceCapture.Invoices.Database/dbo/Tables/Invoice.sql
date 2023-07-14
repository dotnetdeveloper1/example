CREATE TABLE [dbo].[Invoice]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(MAX) NOT NULL,
    [StatusId] INT NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_Invoice_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_Invoice_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [FileName] NVARCHAR(MAX) NOT NULL, 
    [FileId] NVARCHAR(MAX) NOT NULL, 
    [FileSourceTypeId] INT NOT NULL, 
    [InvoiceState] INT NOT NULL,
    [ValidationMessage] NVARCHAR(MAX) NULL,
    [FromEmailAddress] NVARCHAR(MAX) NULL,

    CONSTRAINT [PK_Invoice] PRIMARY KEY ([Id])
)
