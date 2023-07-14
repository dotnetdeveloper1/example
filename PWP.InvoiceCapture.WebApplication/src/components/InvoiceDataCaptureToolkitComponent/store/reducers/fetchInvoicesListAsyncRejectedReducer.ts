import { SerializedError } from "@reduxjs/toolkit";
import { PayloadAction } from "@reduxjs/toolkit";
import { FETCH_INVOICES_LIST_ASYNC_ACTION } from "../actions/Actions";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";
import { IInvoiceDataCaptureErrors } from "./../state/IInvoiceDataCaptureErrors";

export function fetchInvoicesListAsyncRejectedReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<
        IInvoiceDataCaptureErrors | undefined,
        string,
        { arg: string; requestId: string; aborted: boolean },
        SerializedError
    >
): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== FETCH_INVOICES_LIST_ASYNC_ACTION
    );
    if (action.payload) {
        state.error = { ...action.payload, confirm: false };
    } else {
        state.error = { message: "Internal Error", confirm: false };
    }
}
