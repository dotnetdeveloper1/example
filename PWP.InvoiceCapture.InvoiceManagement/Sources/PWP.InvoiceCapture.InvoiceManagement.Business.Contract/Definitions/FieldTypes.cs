namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Definitions
{
    public static class FieldTypes
    {
        // Vendor name is used for template/culture association
        public const int VendorName = 1;

        // The following list of items is required just to support v.0.9 mapping and Unit Tests
        public const int VendorAddress = 2;
        public const int TaxNumber = 3;
        public const int VendorPhone = 4;
        public const int VendorEmail = 5;
        public const int VendorWebsite = 6;
        public const int InvoiceDate = 7;
        public const int DueDate = 8;
        public const int PoNumber = 9;
        public const int InvoiceNumber = 10;
        public const int TaxAmount = 11;
        public const int FreightAmount = 12;
        public const int SubTotal = 13;
        public const int Total = 14;
    }
}
