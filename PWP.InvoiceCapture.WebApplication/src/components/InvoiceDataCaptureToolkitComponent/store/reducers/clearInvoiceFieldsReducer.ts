import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";
import { updateDocumentLayoutItemsFocus } from "./common/updateDocumentLayoutItemsFocus";

export function clearInvoiceFieldsReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.invoiceFields = {
        invoiceFields: state.invoiceFields
            ? state.invoiceFields?.invoiceFields.map((field) => ({
                  ...field,
                  value: ""
              }))
            : [],
        lineItems: [],
        tableTemporaryLineItems: []
    };
    if (state.invoiceDataAnnotation && _.isArray(state.invoiceDataAnnotation.invoiceFieldsAnnotation)) {
        state.invoiceDataAnnotation.invoiceFieldsAnnotation = [];
        state.invoiceDataAnnotation.invoiceLineItemAnnotation = [];
    }
    updateDocumentLayoutItemsFocus(state);
    updateDocumentLayoutItemsAssignment(state);
}
