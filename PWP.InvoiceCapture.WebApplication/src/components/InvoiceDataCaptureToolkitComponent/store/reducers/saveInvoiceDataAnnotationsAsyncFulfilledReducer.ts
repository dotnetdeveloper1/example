import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION } from "./../actions/Actions";

export function saveInvoiceDataAnnotationsAsyncFulfilledReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION
    );
}
