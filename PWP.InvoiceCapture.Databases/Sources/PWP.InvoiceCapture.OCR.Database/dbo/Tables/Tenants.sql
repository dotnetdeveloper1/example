CREATE TABLE [dbo].[Tenants] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [TenantId] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id])
);

