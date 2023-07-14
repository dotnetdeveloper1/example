CREATE TABLE [dbo].[Field]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [GroupId] INT NOT NULL,
    [OrderNumber] INT NOT NULL,
    [IsProtected] BIT NOT NULL,
    [IsDeleted] BIT NOT NULL,
    [DisplayName] NVARCHAR(MAX) NOT NULL,
    [Type] INT NOT NULL,
    [TargetFieldType] INT NULL,
    [DefaultValue] NVARCHAR(MAX) NULL,
    [Formula] NVARCHAR(MAX) NULL,
    [IsRequired] BIT NOT NULL,
    [MinValue] DECIMAL NULL,
    [MaxValue] DECIMAL NULL,
    [MinLength] INT NULL,
    [MaxLength] INT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_Field_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_Field_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_Field] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Field_FieldGroup_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [FieldGroup]([Id])
)
