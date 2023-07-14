import { PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceFieldDataAnnotation } from "../state";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";

export function changeInvoiceFieldDataAnnotationReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<IInvoiceFieldDataAnnotation>
): void {
    if (state.invoiceDataAnnotation && _.isArray(state.invoiceDataAnnotation.invoiceFieldsAnnotation)) {
        const oldInvoiceFieldAnnotation = _(state.invoiceDataAnnotation.invoiceFieldsAnnotation).find(
            (annotation) => action.payload.fieldId === annotation.fieldId
        );
        if (oldInvoiceFieldAnnotation) {
            oldInvoiceFieldAnnotation.fieldId = action.payload.fieldId;
            oldInvoiceFieldAnnotation.fieldValue = action.payload.fieldValue;
            oldInvoiceFieldAnnotation.userCreated = true;
            oldInvoiceFieldAnnotation.documentLayoutItemIds = action.payload.documentLayoutItemIds;
            oldInvoiceFieldAnnotation.selected = action.payload.selected;
        } else {
            state.invoiceDataAnnotation.invoiceFieldsAnnotation.push(action.payload);
        }
    }

    updateDocumentLayoutItemsAssignment(state);
}
