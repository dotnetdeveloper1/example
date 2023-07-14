CREATE TABLE [dbo].[InvoiceTemplateCultureSetting]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[CultureName] NVARCHAR(50) NOT NULL,
	[TemplateId] NVARCHAR(MAX) NOT NULL,
	[CreatedDate] DATETIME CONSTRAINT [DF_InvoiceTemplateCultureSetting_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_InvoiceTemplateCultureSetting_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
	
	CONSTRAINT [PK_InvoiceTemplateCultureSetting] PRIMARY KEY ([Id])
)
