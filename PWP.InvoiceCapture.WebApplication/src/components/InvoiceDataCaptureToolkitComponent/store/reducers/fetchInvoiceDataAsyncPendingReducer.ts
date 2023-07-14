import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { defaultDocumentViewState } from "../state";
import { FETCH_INVOICE_DATA_ASYNC_ACTION } from "./../actions/Actions";

export function fetchInvoiceDataAsyncPendingReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.invoiceFields = undefined;
    state.error = undefined;
    state.invoiceDataAnnotation = undefined;
    state.cleanProcessingResults = undefined;
    state.documentView = { ...defaultDocumentViewState };
    state.pendingHttpRequests = [...state.pendingHttpRequests, { type: FETCH_INVOICE_DATA_ASYNC_ACTION }];
}
