SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([Id], [UserName], [PasswordHash], [GroupId], [IsActive], [CreatedDate], [ModifiedDate]) 
VALUES (1, N'tenantKey', N'np+VfX5PaPhaHbF92vP+AMPttIz3IkcAnVKpeipQnBBwSVvwXTPcuZQbKOY/MlhaAChllLBVv2zQwNy5Ra4chQ==', 1, 1, GETUTCDATE(), GETUTCDATE())

SET IDENTITY_INSERT [dbo].[User] OFF
