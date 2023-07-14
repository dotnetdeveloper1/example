import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function getTrainingsCountReducer(state: IInvoiceDataCaptureToolkitState, action: PayloadAction<number>): void {
    state.trainingsCount = action.payload;
}
