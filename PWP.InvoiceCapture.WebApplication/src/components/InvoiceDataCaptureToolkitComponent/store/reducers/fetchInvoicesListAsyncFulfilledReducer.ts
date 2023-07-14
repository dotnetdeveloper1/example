import { FETCH_INVOICES_LIST_ASYNC_ACTION } from "../actions/Actions";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";

export function fetchInvoicesListAsyncFulfilledReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== FETCH_INVOICES_LIST_ASYNC_ACTION
    );
}
