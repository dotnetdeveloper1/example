import { IInvoiceDataCaptureToolkitState } from "../../InvoiceDataCaptureToolkitState";
import { LineItemsFieldTypes } from "../../state";

export function syncInvoiceFieldsAnnotationWithInvoiceFields(state: IInvoiceDataCaptureToolkitState): void {
    if (
        state.invoiceDataAnnotation &&
        state.invoiceDataAnnotation.invoiceFieldsAnnotation &&
        state.invoiceDataAnnotation.invoiceFieldsAnnotation.length > 0 &&
        state.invoiceFields
    ) {
        const invoiceFieldsAnnotation = state.invoiceDataAnnotation.invoiceFieldsAnnotation;
        const invoiceLinesAnnotation = state.invoiceDataAnnotation.invoiceLineItemAnnotation;
        const invoiceFields = state.invoiceFields;

        invoiceFieldsAnnotation.forEach((annotation) => {
            const invoiceField = invoiceFields.invoiceFields.find((field) => field.fieldId === annotation.fieldId);

            if (invoiceField) {
                invoiceField.value = annotation.fieldValue;
            }
        });

        invoiceLinesAnnotation.forEach((annotation) => {
            const description = annotation.lineItemAnnotations.filter(
                (line) => line.fieldType === LineItemsFieldTypes.description
            );
            const itemNumber = annotation.lineItemAnnotations.filter(
                (line) => line.fieldType === LineItemsFieldTypes.number
            );
            const quantity = annotation.lineItemAnnotations.filter(
                (line) => line.fieldType === LineItemsFieldTypes.quantity
            );
            const total = annotation.lineItemAnnotations.filter(
                (line) => line.fieldType === LineItemsFieldTypes.lineTotal
            );
            const price = annotation.lineItemAnnotations.filter((line) => line.fieldType === LineItemsFieldTypes.price);
            invoiceFields.lineItems = [
                ...invoiceFields.lineItems,
                {
                    description: description && description[0] ? description[0].fieldValue : "",
                    number: itemNumber && itemNumber[0] ? itemNumber[0].fieldValue : "",
                    orderNumber: annotation.orderNumber,
                    quantity: quantity && quantity[0] ? (quantity[0].fieldValue as any) : "",
                    price: price && price[0] ? (price[0].fieldValue as any) : "",
                    lineTotal: total && total[0] ? (total[0].fieldValue as any) : ""
                }
            ];
        });
    }
}
