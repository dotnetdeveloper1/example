import { IInvoiceLineItem } from "../../state/IInvoiceLineItem";

export function isEmptyLineItem(lineItem: IInvoiceLineItem): boolean {
    return (
        !lineItem ||
        ((!lineItem.description || lineItem.description === "") &&
            (!lineItem.number || lineItem.number === "") &&
            !lineItem.lineTotal &&
            !lineItem.quantity &&
            !lineItem.price)
    );
}
