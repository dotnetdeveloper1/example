CREATE TABLE [dbo].[FormulaField]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [ResultFieldId] INT NOT NULL,
    [OperandFieldId] INT NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_FormulaField_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_FormulaField_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_FormulaField] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FormulaField_Field_ResultFieldId] FOREIGN KEY ([ResultFieldId]) REFERENCES [Field]([Id]),
    CONSTRAINT [FK_FormulaField_Field_OperandFieldId] FOREIGN KEY ([OperandFieldId]) REFERENCES [Field]([Id])
)
