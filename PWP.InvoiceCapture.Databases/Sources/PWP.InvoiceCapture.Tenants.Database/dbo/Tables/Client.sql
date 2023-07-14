CREATE TABLE [dbo].[Client]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [ClientId] NVARCHAR(2048) NOT NULL,
    [SecretHash] NVARCHAR(MAX) NOT NULL,
    [IsActive] BIT NOT NULL, 
    [CreatedDate] DATETIME CONSTRAINT [DF_Client_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_Client_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_Client] PRIMARY KEY ([Id]), 
    CONSTRAINT [UQ_Client_ClientId] UNIQUE ([ClientId])
)
