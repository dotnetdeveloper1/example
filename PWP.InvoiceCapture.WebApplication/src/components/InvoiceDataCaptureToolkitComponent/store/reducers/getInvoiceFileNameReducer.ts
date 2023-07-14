import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function getInvoiceFileNameReducer(state: IInvoiceDataCaptureToolkitState, action: PayloadAction<string>): void {
    state.invoiceFileName = action.payload;
}
