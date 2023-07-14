import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function getCultureReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<string | undefined>
): void {
    state.cultureName = action.payload;
}
