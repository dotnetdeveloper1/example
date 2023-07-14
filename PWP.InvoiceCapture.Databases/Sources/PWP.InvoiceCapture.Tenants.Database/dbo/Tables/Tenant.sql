CREATE TABLE [dbo].[Tenant]
(
    [Id] INT NOT NULL IDENTITY(1,1), 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [GroupId] INT NOT NULL,
    [DatabaseName] NVARCHAR(MAX) NOT NULL, 
    [IsActive] BIT NOT NULL, 
    [Status] INT NOT NULL, 
    [DocumentUploadEmail] NVARCHAR(MAX) NULL,
    
    [CreatedDate] DATETIME CONSTRAINT [DF_Tenant_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_Tenant_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
     
    CONSTRAINT [PK_Tenant] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tenant_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group]([Id])
)

