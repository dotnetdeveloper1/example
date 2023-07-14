import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceFields } from "../state";

export function getInvoiceFieldsReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<IInvoiceFields>
): void {
    state.invoiceFields = action.payload;
}
