CREATE TABLE [dbo].[User]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [UserName] NVARCHAR(2048) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [GroupId] INT NOT NULL, 
    [IsActive] BIT NOT NULL, 
    [CreatedDate] DATETIME CONSTRAINT [DF_User_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_User_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_User] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_User_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group]([Id]),
    CONSTRAINT [UQ_User_UserName] UNIQUE ([UserName])
)
