import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";

export function clearEmptyDataAnnotationsReducer(state: IInvoiceDataCaptureToolkitState): void {
    if (state.invoiceDataAnnotation && _.isArray(state.invoiceDataAnnotation.invoiceFieldsAnnotation)) {
        state.invoiceDataAnnotation.invoiceFieldsAnnotation = state.invoiceDataAnnotation.invoiceFieldsAnnotation.filter(
            (annotation) => annotation.fieldValue !== ""
        );

        state.invoiceDataAnnotation.invoiceLineItemAnnotation.forEach((annotations) => {
            annotations.lineItemAnnotations = annotations.lineItemAnnotations.filter(
                (currentAnnotation) => currentAnnotation.fieldValue !== ""
            );
        });

        updateDocumentLayoutItemsAssignment(state);
    }
}
