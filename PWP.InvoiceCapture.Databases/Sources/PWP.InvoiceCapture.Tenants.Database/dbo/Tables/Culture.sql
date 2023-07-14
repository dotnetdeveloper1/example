CREATE TABLE [dbo].[Culture]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(MAX) NOT NULL,
    [EnglishName] NVARCHAR(MAX) NOT NULL,
    [NativeName] NVARCHAR(MAX) NULL

    CONSTRAINT [PK_Culture] PRIMARY KEY ([Id])
)
