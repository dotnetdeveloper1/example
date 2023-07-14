import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { ISelectedLineItemsFieldTypes } from "../state/ISelectedLineItemsFieldTypes";

export function lineItemsFieldsInFocusSelector(state: IInvoiceDataCaptureToolkitState): ISelectedLineItemsFieldTypes[] {
    let selectedDoumentLineItemsFieldTypes: ISelectedLineItemsFieldTypes[] = [];

    if (
        state.documentView.selectedDocumentLayoutItemIds &&
        state.documentView.selectedDocumentLayoutItemIds.length > 0 &&
        state.invoiceDataAnnotation &&
        state.invoiceDataAnnotation.invoiceFieldsAnnotation.length > 0
    ) {
        const selectedDocumentLayoutItemSet = new Set(state.documentView.selectedDocumentLayoutItemIds);

        const lineItemAnnotations = state.invoiceDataAnnotation.invoiceLineItemAnnotation;

        lineItemAnnotations.forEach((annotation) => {
            const fieldAnnotationsInFocus = annotation.lineItemAnnotations.filter((lineItemAnnotation) =>
                lineItemAnnotation.documentLayoutItemIds.find((id) => selectedDocumentLayoutItemSet.has(id))
            );

            if (fieldAnnotationsInFocus && fieldAnnotationsInFocus.length > 0) {
                const selectedLineItemsFieldTypes: ISelectedLineItemsFieldTypes = {
                    lineItemsFieldTypes: [],
                    orderNumber: annotation.orderNumber
                };

                fieldAnnotationsInFocus.forEach((dataAnnotation) => {
                    selectedLineItemsFieldTypes.lineItemsFieldTypes = [
                        ...selectedLineItemsFieldTypes.lineItemsFieldTypes,
                        dataAnnotation.fieldType
                    ];
                });

                selectedDoumentLineItemsFieldTypes = [
                    ...selectedDoumentLineItemsFieldTypes,
                    selectedLineItemsFieldTypes
                ];
            }
        });
    }

    return selectedDoumentLineItemsFieldTypes;
}
