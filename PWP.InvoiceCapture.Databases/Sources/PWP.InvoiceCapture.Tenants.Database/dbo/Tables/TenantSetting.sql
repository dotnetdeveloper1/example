CREATE TABLE [dbo].[TenantSetting]
(
    [Id] INT NOT NULL IDENTITY(1,1), 
    [TenantId] INT NOT NULL, 
    [CultureId] INT NOT NULL,
    [CreatedDate] DATETIME CONSTRAINT [DF_TenantSetting_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_TenantSetting_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL, 

    CONSTRAINT [PK_TenantSetting] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_TenantSetting_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenant]([Id]),
    CONSTRAINT [FK_TenantSetting_Culture_CultureId] FOREIGN KEY ([CultureId]) REFERENCES [Culture]([Id])
)

