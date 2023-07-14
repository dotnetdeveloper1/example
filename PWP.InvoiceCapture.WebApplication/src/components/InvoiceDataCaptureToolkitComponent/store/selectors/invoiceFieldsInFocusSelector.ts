import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";

export function invoiceFieldsInFocusSelector(state: IInvoiceDataCaptureToolkitState): string[] {
    if (
        state.documentView.selectedDocumentLayoutItemIds &&
        state.documentView.selectedDocumentLayoutItemIds.length > 0 &&
        state.invoiceDataAnnotation &&
        state.invoiceDataAnnotation.invoiceFieldsAnnotation.length > 0
    ) {
        const selectedDocumentLayoutItemSet = new Set(state.documentView.selectedDocumentLayoutItemIds);

        const fieldAnnotationsInFocus = state.invoiceDataAnnotation.invoiceFieldsAnnotation.filter((annotation) =>
            annotation.documentLayoutItemIds.find((id) => selectedDocumentLayoutItemSet.has(id))
        );

        if (fieldAnnotationsInFocus && fieldAnnotationsInFocus.length > 0) {
            return fieldAnnotationsInFocus.map((annotation) => annotation.fieldId);
        }
    }

    return [];
}
