CREATE TABLE [dbo].[InvoiceLine]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [InvoiceId] INT NOT NULL,
    [OrderNumber] INT NOT NULL,
    [Number] NVARCHAR(MAX) NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [Quantity] DECIMAL(18,2) NULL,
    [Price] DECIMAL(18,2) NULL, 
    [Total] DECIMAL(18,2) NOT NULL, 
    [CreatedDate] DATETIME CONSTRAINT [DF_InvoiceLine_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_InvoiceLine_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_InvoiceLine] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_InvoiceLine_Invoice_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoice]([Id]),
    CONSTRAINT [UQ_InvoiceLine_InvoiceId_OrderNumber] UNIQUE ([InvoiceId], [OrderNumber])
)
