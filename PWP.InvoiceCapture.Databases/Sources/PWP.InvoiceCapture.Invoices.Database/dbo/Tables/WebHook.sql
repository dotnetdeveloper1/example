CREATE TABLE [dbo].[WebHook]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [TriggerType] INT NOT NULL,
    [URL] NVARCHAR(4000) NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_WebHook_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_WebHook_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_WebHook] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_WebHook_TriggerType_URL] UNIQUE([TriggerType], [URL])
)
