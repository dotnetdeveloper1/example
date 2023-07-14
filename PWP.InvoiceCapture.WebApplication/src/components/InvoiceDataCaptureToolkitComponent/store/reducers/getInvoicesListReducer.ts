import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoice } from "./../state/IInvoice";

export function getInvoicesListReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<IInvoice[]>
): void {
    state.invoicesList = action.payload;
}
