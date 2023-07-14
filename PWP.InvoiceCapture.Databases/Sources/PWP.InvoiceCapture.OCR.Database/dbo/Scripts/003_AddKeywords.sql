SET IDENTITY_INSERT dbo.[LabelKeywords] ON

INSERT INTO dbo.[LabelKeywords] (Id , [LabelOfInterestId],[Text] )
VALUES 
    (1,3,'Invoice' ),
    (2,3,'Date' ),
    (3,6,'due' ),
    (4,15,'Freight' )

SET IDENTITY_INSERT dbo.[LabelKeywords] OFF
