SET IDENTITY_INSERT dbo.[Pack] ON

INSERT INTO dbo.[Pack] (id, [Name], [AllowedDocumentsCount], [Price], [CurrencyId] )
VALUES 
    (1, 'Company Booster Pack - Evaluation', 300, 0, 1),
    (2, '1200 Docs BoosterPack - Companyspace', 1200, 396, 1),
    (3, '3000 Docs BoosterPack - Companyspace', 3000, 744, 1),
    (4, '6000 Docs BoosterPack - Companyspace', 6000, 1320, 1),
    (5, '12000 Docs BoosterPack - Companyspace', 12000, 2400, 1)

SET IDENTITY_INSERT dbo.[Pack] OFF
