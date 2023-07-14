import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { updateDocumentLayoutItemsFocus } from "./common/updateDocumentLayoutItemsFocus";

export function clearInvoiceFieldDataAnnotationSelectionReducer(state: IInvoiceDataCaptureToolkitState): void {
    if (state.invoiceDataAnnotation) {
        if (_.isArray(state.invoiceDataAnnotation.invoiceFieldsAnnotation)) {
            state.invoiceDataAnnotation.invoiceFieldsAnnotation.forEach((annotation) => {
                annotation.selected = false;
            });
        }
        if (_.isArray(state.invoiceDataAnnotation.invoiceLineItemAnnotation)) {
            state.invoiceDataAnnotation.invoiceLineItemAnnotation.forEach((annotation) => {
                if (_.isArray(annotation.lineItemAnnotations)) {
                    annotation.lineItemAnnotations.forEach((lineAnnotation) => (lineAnnotation.selected = false));
                }
            });
        }
    }

    updateDocumentLayoutItemsFocus(state);
}
