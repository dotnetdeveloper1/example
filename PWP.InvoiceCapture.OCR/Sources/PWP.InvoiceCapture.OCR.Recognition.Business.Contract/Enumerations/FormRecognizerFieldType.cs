namespace PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Enumerations
{
    //Form Recognizer Documentation: https://docs.microsoft.com/en-us/azure/cognitive-services/form-recognizer/concept-invoices
    public enum FormRecognizerFieldType
    {
        Unknown = 0,
        CustomerName = 1,
        CustomerId = 2,
        PurchaseOrder = 3,
        InvoiceId = 4,
        InvoiceDate = 5,
        DueDate = 6,
        VendorName = 7,
        VendorAddress = 8,
        VendorAddressRecipient = 9,
        CustomerAddress = 10,
        CustomerAddressRecipien = 11,
        BillingAddress = 12,
        BillingAddressRecipient = 13,
        ShippingAddress = 14,
        ShippingAddressRecipient = 15,
        SubTotal = 16,
        TotalTax = 17,
        InvoiceTotal = 18,
        AmountDue = 19,
        ServiceAddress = 20,
        ServiceAddressRecipient = 21,
        RemittanceAddress = 22,
        RemittanceAddressRecipient = 23,
        ServiceStartDate = 24,
        ServiceEndDate = 25,
        PreviousUnpaidBalance = 26
    }
}
