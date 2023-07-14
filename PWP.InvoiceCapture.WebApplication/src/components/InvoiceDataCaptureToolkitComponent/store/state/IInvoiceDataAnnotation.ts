import { IInvoiceFieldDataAnnotation } from "./IInvoiceFieldDataAnnotation";
import { IInvoiceLineItemAnnotation } from "./IInvoiceLineItemAnnotation";

export interface IInvoiceDataAnnotation {
    plainDocumentText?: string;
    invoiceFieldsAnnotation: IInvoiceFieldDataAnnotation[];
    invoiceLineItemAnnotation: IInvoiceLineItemAnnotation[];
}
