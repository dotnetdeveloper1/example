import { Field } from "./Field";

export interface InvoiceField {
    id: number;
    invoiceId: number;
    field: Field;
    value: string | undefined;
    createdDate: Date;
    modifiedDate: Date;
}
