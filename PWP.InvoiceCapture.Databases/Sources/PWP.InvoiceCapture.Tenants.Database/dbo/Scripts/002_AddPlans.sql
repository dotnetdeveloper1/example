SET IDENTITY_INSERT dbo.[Plan] ON

INSERT INTO dbo.[Plan] (id , [Name], [Type], [AllowedDocumentsCount], [Price], [CurrencyId] )
VALUES 
    (1, 'Company - Monthly- Plan A - 100Docs', 1, 100, 32, 1),
    (2, 'Company - Monthly- Plan B - 250Docs', 1, 250, 60, 1),
    (3, 'Company - Monthly- Plan C - 500Docs', 1, 500, 108, 1),
    (4, 'Company - Monthly- Plan D - 1000Docs', 1, 1000, 180, 1),
    (5, 'Company - Monthly- Plan E - 5000Docs', 1, 5000, 600, 1),
    (6, 'Company - Monthly- Plan F - 10000Docs', 1, 10000, 1080, 1),
    (7, 'Company - Monthly- Plan G - 20000Docs', 1, 20000, 1800, 1),
    (8, 'Company - Monthly- Plan H - 50000Docs', 1, 50000, 4200, 1),
    (9, 'Company - Monthly- Plan I - 100000Docs', 1, 100000, 7800, 1),
    (10, 'Company - Annual- Plan A - 1200Docs', 2, 1200, 320, 1),
    (11, 'Company - Annual- Plan B - 3000Docs', 2, 3000, 620, 1),
    (12, 'Company - Annual- Plan C - 6000Docs', 2, 6000, 1100, 1),
    (13, 'Company - Annual- Plan D - 12000Docs', 2, 12000, 2000, 1),
    (14, 'Company - Annual- Plan E - 60000Docs', 2, 60000, 6400, 1),
    (15, 'Company - Annual- Plan F - 120000Docs', 2, 120000, 11400, 1),
    (16, 'Company - Annual- Plan G - 240000Docs', 2, 240000, 18400, 1),
    (17, 'Company - Annual- Plan H - 600000Docs', 2, 600000, 44000, 1),
    (18, 'Company - Annual- Plan I - 1200000Docs', 2, 1200000, 84000, 1),
    (19, 'Company - Annual- Plan J - 30000Docs', 2, 30000, 4000, 1),
    (20, 'Company - Monthly- Plan J - 10Docs', 1, 10, 0, 1)

SET IDENTITY_INSERT dbo.[Plan] OFF
