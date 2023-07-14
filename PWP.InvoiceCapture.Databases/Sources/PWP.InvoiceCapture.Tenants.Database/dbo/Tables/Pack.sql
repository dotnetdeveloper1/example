CREATE TABLE [dbo].[Pack]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [AllowedDocumentsCount] INT NOT NULL, 
    [Price] DECIMAL(18, 2) NOT NULL, 
    [CurrencyId] INT NOT NULL, 
    [CreatedDate] DATETIME CONSTRAINT [DF_Pack_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_Pack_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_Pack] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Pack_Currency_TenantId] FOREIGN KEY (CurrencyId) REFERENCES Currency([Id])
)
