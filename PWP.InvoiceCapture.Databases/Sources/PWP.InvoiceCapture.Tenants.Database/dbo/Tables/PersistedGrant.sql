CREATE TABLE [dbo].[PersistedGrant]
(
	[Key] NVARCHAR(200) NOT NULL,
	[Type] NVARCHAR(MAX) NULL,
	[SubjectId] NVARCHAR(MAX) NULL,
	[SessionId] NVARCHAR(MAX) NULL,
	[ClientId] NVARCHAR(MAX) NULL,
	[Description] NVARCHAR(MAX) NULL,
	[Data] NVARCHAR(MAX) NULL,
	[CreationTime] DATETIME2 NULL,
	[Expiration] DATETIME2  NULL,
	[ConsumedTime] DATETIME2  NULL,

    CONSTRAINT [PK_PersistedGrant] PRIMARY KEY ([Key])
)
