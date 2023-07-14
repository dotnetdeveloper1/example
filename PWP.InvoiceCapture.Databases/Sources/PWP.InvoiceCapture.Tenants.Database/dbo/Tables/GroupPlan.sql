CREATE TABLE [dbo].[GroupPlan]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
	[PlanId] INT NOT NULL,
	[GroupId] INT NOT NULL,
	[UploadedDocumentsCount] INT NOT NULL,
	[IsRenewalCancelled] BIT NOT NULL DEFAULT 0, 
	[StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME  NOT NULL, 
	[CreatedDate] DATETIME CONSTRAINT [DF_GroupPlan_CreatedDate] DEFAULT (GETUTCDATE()) NOT NULL, 
    [ModifiedDate] DATETIME CONSTRAINT [DF_GroupPlan_ModifiedDate] DEFAULT (GETUTCDATE()) NOT NULL

	CONSTRAINT [PK_GroupPlan] PRIMARY KEY ([Id])
	CONSTRAINT [FK_GroupPlan_Group_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Group]([Id])
	CONSTRAINT [FK_GroupPlan_Plan_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Plan]([Id])
)
