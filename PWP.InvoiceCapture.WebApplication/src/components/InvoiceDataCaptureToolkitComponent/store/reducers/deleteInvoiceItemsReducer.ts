import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { deleteInvoiceItem } from "./helpers/deleteInvoiceItem";

export function deleteInvoiceItemsReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<{ orderNumber: number }>
): void {
    deleteInvoiceItem(action.payload.orderNumber, state);
}
