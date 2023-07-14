import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION } from "./../actions/Actions";

export function submitInvoiceDataAnnotationsAsyncFulfilledReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION
    );
}
