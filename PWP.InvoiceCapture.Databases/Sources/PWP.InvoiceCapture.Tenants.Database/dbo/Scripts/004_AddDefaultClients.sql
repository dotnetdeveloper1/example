SET IDENTITY_INSERT [dbo].[Client] ON

INSERT [dbo].[Client] ([Id], [ClientId], [SecretHash], [IsActive], [CreatedDate], [ModifiedDate]) 
VALUES (1, N'defaultClient', N'XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=', 1, GETUTCDATE(), GETUTCDATE())

INSERT [dbo].[Client] ([Id], [ClientId], [SecretHash], [IsActive], [CreatedDate], [ModifiedDate]) 
VALUES (2, N'webApplication', N'XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=', 1, GETUTCDATE(), GETUTCDATE())

SET IDENTITY_INSERT [dbo].[Client] OFF
