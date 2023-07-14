import { InvoiceField } from "./../InvoiceFields/InvoiceField";
import { Contact } from "./Contact";
import { InvoiceLineItem } from "./InvoiceLineItem";

export interface Invoice {
    id: number;
    name: string;
    status: number;
    createdDate: Date;
    modifiedDate: Date;
    invoiceNumber: string;
    invoiceDate: Date;
    dueDate: Date;
    poNumber: string;
    taxNumber: string;
    taxAmount: number;
    freightAmount: number;
    subTotal: number;
    total: number;
    currencyId: number;
    fileName: string;
    fileId: string;
    fileSourceTypeId: number;
    contacts: Contact[];
    invoiceFields: InvoiceField[];
    invoiceLines: InvoiceLineItem[];
}
