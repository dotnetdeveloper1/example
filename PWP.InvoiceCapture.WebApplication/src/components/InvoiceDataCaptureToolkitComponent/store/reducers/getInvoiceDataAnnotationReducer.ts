import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDataAnnotation } from "../state";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";

export function getInvoiceDataAnnotationReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<IInvoiceDataAnnotation>
): void {
    state.invoiceDataAnnotation = action.payload;

    updateDocumentLayoutItemsAssignment(state);
}
