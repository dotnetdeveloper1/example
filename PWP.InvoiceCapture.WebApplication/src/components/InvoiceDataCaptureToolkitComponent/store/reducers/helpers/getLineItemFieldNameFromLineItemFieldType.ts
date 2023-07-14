import { LineItemsFieldTypes } from "../../state";
import { IInvoiceLineItem } from "../../state/IInvoiceLineItem";

export function getLineItemFieldNameFromLineItemFieldType(lineItemsFieldTypes: LineItemsFieldTypes): string {
    switch (lineItemsFieldTypes) {
        case LineItemsFieldTypes.description:
            return getPropertyName<IInvoiceLineItem>((p) => p.description);
        case LineItemsFieldTypes.number:
            return getPropertyName<IInvoiceLineItem>((p) => p.number);
        case LineItemsFieldTypes.quantity:
            return getPropertyName<IInvoiceLineItem>((p) => p.quantity);
        case LineItemsFieldTypes.lineTotal:
            return getPropertyName<IInvoiceLineItem>((p) => p.lineTotal);
        case LineItemsFieldTypes.price:
            return getPropertyName<IInvoiceLineItem>((p) => p.price);
        default:
            return "";
    }
}

const getPropertyName = <T>(property: (object: T) => void) => {
    const chaine = property.toString();
    const arr = chaine.match(/[\s\S]*{[\s\S]*\.([^.; ]*)[ ;\n]*}/);
    if (arr) {
        return arr[1];
    }
    return "";
};
