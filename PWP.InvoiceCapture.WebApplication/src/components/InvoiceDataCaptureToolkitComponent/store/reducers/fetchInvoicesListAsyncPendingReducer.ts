import { FETCH_INVOICES_LIST_ASYNC_ACTION } from "./../actions/Actions";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";

export function fetchInvoicesListAsyncPendingReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.pendingHttpRequests = [...state.pendingHttpRequests, { type: FETCH_INVOICES_LIST_ASYNC_ACTION }];
}
