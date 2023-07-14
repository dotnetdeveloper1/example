import { LineItemsFieldTypes } from "./LineItemsFieldTypes";

export interface IInvoiceLineItemDataAnnotation {
    fieldType: LineItemsFieldTypes;
    fieldValue: string;
    userCreated: boolean;
    documentLayoutItemIds: string[];
    selected: boolean;
}
