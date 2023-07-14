SET IDENTITY_INSERT [dbo].[Tenant] ON 

INSERT [dbo].[Tenant] ([Id], [Name], [DatabaseName], [IsActive], [Status], [CreatedDate], [ModifiedDate], [GroupId]) 
VALUES (1, N'Default', N'Invoices_Default', 1, 0, GETUTCDATE(), GETUTCDATE(), 1)

SET IDENTITY_INSERT [dbo].[Tenant] OFF
