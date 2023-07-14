	UPDATE [Tenant]
    SET [DocumentUploadEmail] =  LOWER(Replace(NEWID(), '-', '') + '@emailinvoice.workplacecloud.com')
    WHERE [DocumentUploadEmail] IS NULL;
