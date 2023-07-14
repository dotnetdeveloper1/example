import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../../InvoiceDataCaptureToolkitState";

export function updateDocumentLayoutItemsFocus(state: IInvoiceDataCaptureToolkitState): void {
    if (
        state.documentView.pages &&
        state.documentView.pages.length > 0 &&
        state.invoiceDataAnnotation &&
        state.invoiceDataAnnotation.invoiceFieldsAnnotation
    ) {
        const isInvoiceFieldSelected =
            state.invoiceDataAnnotation.invoiceFieldsAnnotation.find((annotation) => annotation.selected) !== undefined;

        const layoutItemIdsOfSelectedInvoiceFields = isInvoiceFieldSelected
            ? _(state.invoiceDataAnnotation.invoiceFieldsAnnotation)
                  .filter((annotation) => annotation.selected)
                  .flatMap((annotation) => annotation.documentLayoutItemIds)
                  .value()
            : [];

        const lineItemAnnotations = _(state.invoiceDataAnnotation.invoiceLineItemAnnotation)
            .flatMap((fieldAnnotation) => fieldAnnotation.lineItemAnnotations)
            .value();

        const isLineItemsFieldSelected = lineItemAnnotations.find((annotation) => annotation.selected) !== undefined;

        const layoutItemIdsOfSelectedLineItemFields = isLineItemsFieldSelected
            ? _(lineItemAnnotations)
                  .filter((annotation) => annotation.selected)
                  .flatMap((annotation) => annotation.documentLayoutItemIds)
                  .value()
            : [];

        const selectedFields = [...layoutItemIdsOfSelectedLineItemFields, ...layoutItemIdsOfSelectedInvoiceFields];

        state.documentView.pages.forEach((page) => {
            if (page.pageLayoutItems && page.pageLayoutItems.length > 0) {
                page.pageLayoutItems.forEach((layoutItem) => {
                    layoutItem.inFocus =
                        isInvoiceFieldSelected || isLineItemsFieldSelected
                            ? selectedFields.find((id) => layoutItem.id === id) !== undefined
                            : undefined;
                });
            }
        });
    }
}
