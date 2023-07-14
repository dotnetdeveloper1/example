CREATE TABLE [dbo].[GroupPack]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
	[PackId] INT NOT NULL,
	[GroupId] INT NOT NULL,
	[UploadedDocumentsCount] INT NOT NULL,
	[CreatedDate] DATETIME CONSTRAINT [DF_GroupPack_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_GroupPack_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL

	CONSTRAINT [PK_GroupPack] PRIMARY KEY ([Id])
	CONSTRAINT [FK_GroupPack_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group]([Id])
	CONSTRAINT [FK_GroupPack_Pack_PackId] FOREIGN KEY ([PackId]) REFERENCES [Pack]([Id])
)
