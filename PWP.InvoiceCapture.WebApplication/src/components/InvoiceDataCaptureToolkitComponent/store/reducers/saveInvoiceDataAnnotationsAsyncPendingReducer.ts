import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION } from "./../actions/Actions";

export function saveInvoiceDataAnnotationsAsyncPendingReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.pendingHttpRequests = [...state.pendingHttpRequests, { type: SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION }];
}
