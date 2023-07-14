CREATE TABLE [dbo].[InvoicePage]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [InvoiceId] INT NOT NULL,
    [Number] INT NOT NULL, 
    [ImageFileId] NVARCHAR(MAX) NOT NULL, 
    [Width] INT NOT NULL, 
    [Height] INT NOT NULL, 

    CONSTRAINT [PK_InvoicePage] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_InvoicePage_Invoice_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoice]([Id])
)
