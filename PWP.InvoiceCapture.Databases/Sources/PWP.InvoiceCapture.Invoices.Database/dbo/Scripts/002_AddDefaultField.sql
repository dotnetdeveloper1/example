SET IDENTITY_INSERT dbo.[Field] ON

INSERT INTO dbo.[Field]
(Id, GroupId, OrderNumber, DisplayName, DefaultValue, [Type], TargetFieldType, IsRequired, IsDeleted, IsProtected, MinValue, MaxValue, MinLength, [MaxLength])
VALUES 
(1, 1, 1, 'Vendor Name',     NULL, 0, 7,    1, 0, 1, NULL, NULL, NULL , 250),
(2, 1, 2, 'Vendor Address',  NULL, 0, 8,    0, 0, 1, NULL, NULL, NULL , 250),
(3, 1, 3, 'Tax ID',          NULL, 0, NULL, 0, 0, 1, NULL, NULL, NULL , 250),
(4, 1, 4, 'Phone Number',    NULL, 0, NULL, 0, 0, 1, NULL, NULL, NULL , 250),
(5, 1, 5, 'Email',           NULL, 0, NULL, 0, 0, 1, NULL, NULL, NULL , 250),
(6, 1, 6, 'Website',         NULL, 0, NULL, 0, 0, 1, NULL, NULL, NULL , 250),

(7, 2, 1, 'Invoice Date',    NULL, 2, 5,    1, 0, 1, NULL, NULL, NULL , NULL),
(8, 2, 2, 'Due Date',        NULL, 2, 6,    0, 0, 1, NULL, NULL, NULL , NULL),
(9, 2, 3, 'PO Number',       NULL, 0, 3,    0, 0, 1, NULL, NULL, NULL , 250),
(10, 2, 4, 'Invoice Number', NULL, 0, 4,    1, 0, 1, NULL, NULL, NULL , 250),

(11, 3, 1, 'Tax Amount',     NULL, 1, 17,   0, 0, 1, NULL, NULL, NULL , NULL),
(12, 3, 2, 'Freight Total',  NULL, 1, NULL, 0, 0, 1, NULL, NULL, NULL , NULL),
(13, 3, 3, 'Subtotal',       NULL, 1, 16,   1, 0, 1, NULL, NULL, NULL , NULL),
(14, 3, 4, 'Total',          NULL, 1, 18,   1, 0, 1, NULL, NULL, NULL , NULL)


SET IDENTITY_INSERT dbo.[Field] OFF