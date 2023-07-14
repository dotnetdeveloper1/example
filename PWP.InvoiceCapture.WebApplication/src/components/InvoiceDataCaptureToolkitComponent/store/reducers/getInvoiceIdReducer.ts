import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function getInvoiceIdReducer(state: IInvoiceDataCaptureToolkitState, action: PayloadAction<number>): void {
    state.invoiceId = action.payload;
}
