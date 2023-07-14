import { Invoice } from "./Invoice";
import { InvoiceDataAnnotation } from "./InvoiceDataAnnotation";

export interface InvoiceProcessingResult {
    id: number;
    invoiceId: number;
    createdDate: Date;
    modifiedDate: Date;
    templateId?: number;
    processingTypeId: number;
    dataAnnotationFileId: string;
    invoice: Invoice;
    dataAnnotation: InvoiceDataAnnotation;
    cultureName?: string;
    vendorName?: string;
}
