CREATE TABLE [dbo].[InvoiceField]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [InvoiceId] INT NOT NULL,
    [FieldId] INT NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_InvoiceField_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_InvoiceField_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_InvoiceField] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InvoiceField_Invoice_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoice]([Id]),
    CONSTRAINT [FK_InvoiceField_Field_FieldId] FOREIGN KEY ([FieldId]) REFERENCES [Field]([Id])
)
