import { IInvoiceDataCaptureToolkitState } from "../../InvoiceDataCaptureToolkitState";

export function deleteInvoiceItem(orderNumber: number, state: IInvoiceDataCaptureToolkitState): void {
    if (orderNumber < 0) {
        return;
    }

    if (!state.invoiceFields) {
        return;
    }

    state.invoiceFields.lineItems = state.invoiceFields.lineItems.filter((item) => item.orderNumber !== orderNumber);

    if (state.invoiceDataAnnotation) {
        state.invoiceDataAnnotation.invoiceLineItemAnnotation = state.invoiceDataAnnotation?.invoiceLineItemAnnotation.filter(
            (annotation) => annotation.orderNumber !== orderNumber
        );
    }

    state.invoiceFields.lineItems.forEach((item) => {
        if (item.orderNumber > orderNumber) {
            item.orderNumber -= 1;
        }
    });

    if (state.invoiceDataAnnotation) {
        state.invoiceDataAnnotation.invoiceLineItemAnnotation.forEach((annotation) => {
            if (annotation.orderNumber > orderNumber) {
                annotation.orderNumber -= 1;
            }
        });
    }
}
