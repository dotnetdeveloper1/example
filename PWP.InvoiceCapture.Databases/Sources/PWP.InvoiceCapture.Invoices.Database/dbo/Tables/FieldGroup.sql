CREATE TABLE [dbo].[FieldGroup]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [DisplayName] NVARCHAR(MAX) NOT NULL,
    [OrderNumber] INT NOT NULL,
    [IsProtected] BIT NOT NULL,
    [IsDeleted] BIT NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_FieldGroup_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_FieldGroup_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    
    CONSTRAINT [PK_FieldGroup] PRIMARY KEY ([Id])
)
