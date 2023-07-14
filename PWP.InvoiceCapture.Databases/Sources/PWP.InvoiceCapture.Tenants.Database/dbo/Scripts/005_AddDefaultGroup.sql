SET IDENTITY_INSERT [dbo].[Group] ON 

INSERT [dbo].[Group] ([Id], [Name], [ParentGroupId], [CreatedDate], [ModifiedDate]) 
VALUES (1, N'DefaultGroup', NULL, GETUTCDATE(), GETUTCDATE())

SET IDENTITY_INSERT [dbo].[Group] OFF
