import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION } from "./../actions/Actions";

export function submitInvoiceDataAnnotationsAsyncPendingReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.pendingHttpRequests = [...state.pendingHttpRequests, { type: SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION }];
}
