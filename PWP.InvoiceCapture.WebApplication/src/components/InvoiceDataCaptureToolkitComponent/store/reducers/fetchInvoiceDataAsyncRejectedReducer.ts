import { PayloadAction, SerializedError } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDataCaptureErrors } from "../state";
import { FETCH_INVOICE_DATA_ASYNC_ACTION } from "./../actions/Actions";

export function fetchInvoiceDataAsyncRejectedReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<
        IInvoiceDataCaptureErrors | undefined,
        string,
        { arg: number; requestId: string; aborted: boolean },
        SerializedError
    >
): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== FETCH_INVOICE_DATA_ASYNC_ACTION
    );
    if (action.payload) {
        state.error = { ...action.payload, confirm: false };
    } else {
        state.error = { message: "Internal Error", confirm: false };
    }
}
