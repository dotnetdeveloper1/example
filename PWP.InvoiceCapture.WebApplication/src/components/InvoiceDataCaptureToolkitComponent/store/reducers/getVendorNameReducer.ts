import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function getVendorNameReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<string | undefined>
): void {
    state.vendorName = action.payload;
}
