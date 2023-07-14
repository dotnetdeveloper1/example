CREATE TABLE [dbo].[Plan]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Type] INT NOT NULL, 
    [AllowedDocumentsCount] INT NOT NULL, 
    [Price] DECIMAL(18, 2) NOT NULL, 
    [CurrencyId] INT NOT NULL, 
    [CreatedDate] DATETIME CONSTRAINT [DF_Plan_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_Plan_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_Plan] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Plan_Currency_TenantId] FOREIGN KEY (CurrencyId) REFERENCES Currency([Id])
)
