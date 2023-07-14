﻿SET IDENTITY_INSERT dbo.[LabelSynonyms] ON

INSERT INTO dbo.[LabelSynonyms] ([Id], [LabelOfInterestId], [Text], [UseInTemplateComparison])
VALUES 
    (1, 1, 'Invoice Number', 1),
    (2, 1, 'Invoice No.', 1),
    (3, 1, 'Invoice No#', 1),
    (4, 1, 'Invoice Id', 1),
    (5, 1, 'Invoice #:', 1),
    (6, 1, 'Invoice No', 1),
    (7, 1, 'Invoice #', 1),
    (8, 1, 'INV #', 1),
    (9, 1, 'Tax Invoice', 1),
    (10, 2, 'Account Number', 1),
    (11, 2, 'Account No.', 1),
    (12, 2, 'Account No', 1),
    (13, 2, 'Account #:', 1),
    (14, 2, 'Account #', 1),
    (15, 2, 'Acc No.', 1),
    (16, 2, 'Acc #', 1),
    (17, 2, 'Acct #', 1),
    (18, 2, 'Account Code', 1),
    (19, 2, 'Acct Code', 1),
    (20, 2, 'Acc Code', 1),
    (21, 2, 'Account', 0),
    (22, 3, 'Invoice Date', 1),
    (23, 3, 'Date Of Invoice', 1),
    (24, 3, 'Billing Date', 1),
    (25, 3, 'Date Of Issue', 1),
    (26, 3, 'INV. DATE', 1),
    (27, 3, 'nvc date', 1),
    (28, 3, 'invc date', 1),
    (29, 3, 'order date', 1),
    (30, 3, 'date', 1),
    (31, 3, 'statement date', 1),
    (32, 5, 'Shipped Date', 0),
    (33, 5, 'Shipping Date', 0),
    (34, 5, 'Ship Date', 0),
    (35, 6, 'due date', 1),
    (36, 6, 'Time Period', 1),
    (37, 6, 'Auto Pay', 1),
    (38, 6, 'payment due', 0),
    (39, 6, 'date due', 1),
    (40, 6, 'due on', 0),
    (41, 6, 'due', 0),
    (42, 7, 'Invoice Address', 0),
    (43, 7, 'Bill to', 1),
    (44, 7, 'To:', 1),
    (45, 7, 'Invoice To', 1),
    (46, 8, 'Phone number', 0),
    (47, 8, 'Phone No', 0),
    (48, 9, 'Mail', 0),
    (49, 10, 'TAX ID', 1),
    (50, 10, 'Tax Registration Number', 1),
    (51, 10, 'Registration number', 1),
    (52, 10, 'Tax Registration #', 1),
    (53, 10, 'Tax Registration', 1),
    (54, 10, 'Registration No', 1),
    (55, 10, 'Registration #', 1),
    (56, 10, 'Federal Tax ID', 1),
    (57, 10, 'Federal ID#', 1),
    (58, 10, 'Federal ID', 1),
    (59, 10, 'Fed ID', 1),
    (60, 10, 'Tax Number', 1),
    (61, 10, 'HST number', 1),
    (62, 10, 'Federal No', 1),
    (63, 10, 'Tax No', 1),
    (64, 11, 'Already Paid', 0),
    (65, 11, 'Paid Amount', 0),
    (66, 12, 'Tax Amount', 0),
    (67, 12, 'Total Taxes', 0),
    (68, 12, 'Total Tax', 0),
    (69, 12, 'Sales Tax', 0),
    (70, 12, 'GST/HST', 0),
    (71, 12, 'Taxes', 0),
    (72, 12, 'HST', 0),
    (73, 12, 'GST', 0),
    (74, 12, 'Tax', 0),
    (75, 13, 'Balance Due', 0),
    (76, 15, 'Freight', 0),
    (77, 15, 'Freight Amount', 0),
    (78, 15, 'Freight Header', 0),
    (79, 17, 'Please Pay This Amount', 0),
    (80, 17, 'Payment Amount Due', 0),
    (81, 17, 'Total Amount Due', 0),
    (82, 17, 'Amount Due (USD)', 0),
    (83, 17, 'Amount Payable', 0),
    (84, 17, 'Invoice Total', 0),
    (85, 17, 'Total Invoice', 0),
    (86, 17, 'Total Amount', 0),
    (87, 17, 'Grand Total', 0),
    (88, 17, 'Amount Due', 0),
    (89, 17, 'Total Due', 0),
    (90, 17, 'Total No', 0),
    (91, 18, 'Sub Total Amount', 0),
    (92, 18, 'ltem(s) Subtotal', 0),
    (93, 18, 'Invoice SubTotal', 0),
    (94, 18, 'Net Invoice', 0),
    (95, 18, 'Subtotal', 0),
    (96, 18, 'Sub Total', 0),
    (97, 18, 'SubTotal', 0),
    (98, 19, 'Customer Number', 1),
    (99, 19, 'Customer Num.', 1),
    (100, 19, 'Customer Id', 1),
    (101, 19, 'Customer No', 1),
    (102, 19, 'Customer #', 1),
    (103, 19, 'Cust Id', 1),
    (104, 19, 'Cust#', 1),
    (105, 20, 'Terms', 1),
    (106, 20, 'Payment Terms', 1),
    (107, 21, 'CUSTOMER PURCHASE ORDER NUMBER', 1),
    (108, 21, 'purchase order number', 1),
    (109, 21, 'Purchase Order', 1),
    (110, 21, 'Customer PO No', 1),
    (111, 21, 'Customer PO No.', 1),
    (112, 21, 'Customer PO #:', 1),
    (113, 21, 'Customer PO #', 1),
    (114, 21, 'Order Number', 1),
    (115, 21, 'Cust Order#', 1),
    (116, 21, 'P.O. Number', 1),
    (117, 21, 'PO Number', 1),
    (118, 21, 'P.O. No.', 1),
    (119, 21, 'P.O. No', 1),
    (120, 21, 'Order #', 1),
    (121, 21, 'Order#', 1),
    (122, 21, 'P.O. #', 1),
    (123, 21, 'PO No.', 1),
    (124, 21, 'PO No', 1),
    (125, 21, 'PO #', 1),
    (126, 21, 'PO Ref', 1),
    (127, 25, 'P.O. Box', 0),
    (128, 25, 'PO Box', 0) 

SET IDENTITY_INSERT dbo.[LabelSynonyms] OFF
