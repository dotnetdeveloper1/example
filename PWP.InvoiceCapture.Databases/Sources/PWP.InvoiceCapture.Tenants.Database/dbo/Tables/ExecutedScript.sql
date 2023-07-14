CREATE TABLE [dbo].[ExecutedScript]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
    [Name] NVARCHAR(MAX) NULL, 
    [Content] NVARCHAR(MAX) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_ExecutedScript_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [CreatedDate] DATETIME CONSTRAINT [DF_ExecutedScript_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL,

    CONSTRAINT [PK_ExecutedScript] PRIMARY KEY ([Id])
)
