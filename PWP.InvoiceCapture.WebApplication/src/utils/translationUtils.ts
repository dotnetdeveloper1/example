import { InvoiceStatus } from "../components/InvoiceDataCaptureToolkitComponent/store/state/InvoiceStatus";

export class TranslationUtils {
    public static getInvoiceStatusLocalizationKeyByEnum = (status: InvoiceStatus): string => {
        switch (status) {
            case InvoiceStatus.Completed:
                return "INVOICE_STATUS->COMPLETED";
            case InvoiceStatus.Error:
                return "INVOICE_STATUS->ERROR";
            case InvoiceStatus.InProgress:
                return "INVOICE_STATUS->IN_PROGRESS";
            case InvoiceStatus.LimitExceeded:
                return "INVOICE_STATUS->LIMIT_EXCEEDED";
            case InvoiceStatus.NotStarted:
                return "INVOICE_STATUS->NOT_STARTED";
            case InvoiceStatus.PendingReview:
                return "INVOICE_STATUS->PENDING_REVIEW";
            case InvoiceStatus.Queued:
                return "INVOICE_STATUS->QUEUED";
            default:
                return "INVOICE_STATUS->UNDEFINED";
        }
    };

    public static getInvoiceStatusLocalizationKeyByString = (status: string): string => {
        switch (status) {
            case "Completed":
                return "INVOICE_STATUS->COMPLETED";
            case "Error":
                return "INVOICE_STATUS->ERROR";
            case "InProgress":
                return "INVOICE_STATUS->IN_PROGRESS";
            case "LimitExceeded":
                return "INVOICE_STATUS->LIMIT_EXCEEDED";
            case "NotStarted":
                return "INVOICE_STATUS->NOT_STARTED";
            case "PendingReview":
                return "INVOICE_STATUS->PENDING_REVIEW";
            case "Queued":
                return "INVOICE_STATUS->QUEUED";
            default:
                return "INVOICE_STATUS->UNDEFINED";
        }
    };

    public static getInvoiceFieldTypeLocalizationKeyByString = (fieldType: string): string => {
        switch (fieldType) {
            case "Vendor Name":
                return "INVOICE_FIELDS_TITLE->VENDOR_NAME";
            case "Vendor Address":
                return "INVOICE_FIELDS_TITLE->VENDOR_ADDRESS";
            case "Tax ID":
                return "INVOICE_FIELDS_TITLE->TAX_ID";
            case "Phone Number":
                return "INVOICE_FIELDS_TITLE->PHONE_NUMBER";
            case "Email":
                return "INVOICE_FIELDS_TITLE->EMAIL";
            case "Website":
                return "INVOICE_FIELDS_TITLE->WEBSITE";
            case "Invoice Date":
                return "INVOICE_FIELDS_TITLE->INVOICE_DATE";
            case "Due Date":
                return "INVOICE_FIELDS_TITLE->DUE_DATE";
            case "PO Number":
                return "INVOICE_FIELDS_TITLE->PO_NUMBER";
            case "Invoice Number":
                return "INVOICE_FIELDS_TITLE->INVOICE_NUMBER";
            case "Tax Amount":
                return "INVOICE_FIELDS_TITLE->TAX_AMOUNT";
            case "Freight Total":
                return "INVOICE_FIELDS_TITLE->FREIGHT_TOTAL";
            case "Subtotal":
                return "INVOICE_FIELDS_TITLE->SUBTOTAL";
            case "Total":
                return "INVOICE_FIELDS_TITLE->TOTAL";
            default:
                return "";
        }
    };

    public static getItemsFieldTypeLocalizationKeyByString = (fieldType: string): string => {
        switch (fieldType) {
            case "Number #":
                return "INVOICE_LINES_TITLE->NUMBER";
            case "Description":
                return "INVOICE_LINES_TITLE->DESCRIPTION";
            case "Qty":
                return "INVOICE_LINES_TITLE->QUANTITY";
            case "Unit Price":
                return "INVOICE_LINES_TITLE->UNIT_PRICE";
            case "Line Total":
                return "INVOICE_LINES_TITLE->LINE_TOTAL";
            default:
                return "";
        }
    };
}
