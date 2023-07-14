import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";

export function tableSelectionReducer(state: IInvoiceDataCaptureToolkitState, action: PayloadAction<boolean>): void {
    state.tableSelectionMode = action.payload;
}
