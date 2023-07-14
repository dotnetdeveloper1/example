import { POST_INVOICE_FILE_ASYNC_ACTION } from "./../actions/Actions";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";

export function postInvoiceFileAsyncFulfilledReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== POST_INVOICE_FILE_ASYNC_ACTION
    );
}
