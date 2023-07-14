CREATE TABLE [dbo].[FormRecognizers]
(
    [Id] INT NOT NULL,
    [IsActive] BIT NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_FormRecognizers_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_FormRecognizers_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_FormRecognizers] PRIMARY KEY ([Id])
)
