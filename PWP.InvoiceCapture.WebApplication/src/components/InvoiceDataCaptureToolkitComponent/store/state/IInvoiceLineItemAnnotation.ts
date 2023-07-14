import { IInvoiceLineItemDataAnnotation } from "./IInvoiceLineItemDataAnnotation";

export interface IInvoiceLineItemAnnotation {
    orderNumber: number;
    lineItemAnnotations: IInvoiceLineItemDataAnnotation[];
}
