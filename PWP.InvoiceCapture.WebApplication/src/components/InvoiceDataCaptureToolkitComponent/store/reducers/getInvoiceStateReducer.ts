import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { InvoiceStatus } from "./../state";

export function getInvoiceStateReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<InvoiceStatus>
): void {
    state.invoiceStatus = action.payload;
}
