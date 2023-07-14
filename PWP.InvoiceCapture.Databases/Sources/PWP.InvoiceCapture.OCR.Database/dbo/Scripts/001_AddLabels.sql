SET IDENTITY_INSERT dbo.[LabelsOfInterest] ON

INSERT INTO dbo.[LabelsOfInterest] (Id , [Text], [ExpectedType], [UseAbsoluteComparison])
VALUES 
    (1, 'invoiceNumber', 4, 1),
    (2, 'Account Number', 1, 1),
    (3, 'invoiceDate'   , 3, 0),
    (4, 'Order Date', 3, 0),
    (5, 'Shipping Date', 3, 0),
    (6, 'dueDate', 3, 0),
    (7, 'vendorAddress', 1, 0),
    (8, 'vendorPhone', 1, 0),
    (9, 'vendorEmail', 1, 0),
    (10, 'taxNumber', 1, 0),
    (11, 'Amount Paid', 2, 0),
    (12, 'taxAmount', 2, 0),
    (13, 'Balance', 2, 0),
    (14, 'Description', 1, 0),
    (15, 'freightAmount', 2, 0),
    (16, 'Ordered By', 1, 0),
    (17, 'total', 2, 0),
    (18, 'subTotal', 2, 0),
    (19, 'Customer', 1, 0),
    (20, 'Terms', 1, 0),
    (21, 'poNumber', 4, 1),
    (22, 'Authorized By', 1, 0),
    (23, 'vendorName', 1, 0),
    (24, 'vendorWebsite', 1, 0),
    (25, 'poBox', 1, 0)
SET IDENTITY_INSERT dbo.[LabelsOfInterest] OFF
