SET IDENTITY_INSERT dbo.[FieldGroup] ON

INSERT INTO dbo.[FieldGroup] (Id, OrderNumber, DisplayName, IsProtected, IsDeleted)
VALUES 
(1, 1, 'Vendor Data', 1, 0),
(2, 2, 'Invoice Data', 1, 0),
(3, 3, 'Summary Data', 1, 0)

SET IDENTITY_INSERT dbo.[FieldGroup] OFF