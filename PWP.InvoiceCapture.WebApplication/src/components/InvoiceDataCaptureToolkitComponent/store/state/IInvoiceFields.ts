import { FieldType } from "./../../../../api/models/InvoiceFields/FieldType";
import { IInvoiceLineItem } from "./IInvoiceLineItem";
export interface IInvoiceField {
    groupId: string;
    groupOrder: number;
    fieldOrder: number;
    groupName: string;
    fieldId: string;
    fieldName: string;
    fieldType: FieldType;
    isDeleted: boolean;
    defaultValue: string | undefined;
    customValidationFormula: string | undefined;
    isRequired: boolean;
    minValue: number | undefined;
    maxValue: number | undefined;
    minLength: number | undefined;
    maxLength: number | undefined;
    value: string | undefined;
}

export interface IInvoiceFields {
    invoiceFields: IInvoiceField[];
    lineItems: IInvoiceLineItem[];
    tableTemporaryLineItems: IInvoiceLineItem[];
}
