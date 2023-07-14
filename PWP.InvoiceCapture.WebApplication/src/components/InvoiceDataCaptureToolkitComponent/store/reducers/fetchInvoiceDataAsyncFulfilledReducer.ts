import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { FETCH_INVOICE_DATA_ASYNC_ACTION } from "./../actions/Actions";
import { syncInvoiceFieldsAnnotationWithInvoiceFields } from "./common/syncInvoiceFieldsAnnotationWithInvoiceFields";

export function fetchInvoiceDataAsyncFulfilledReducer(state: IInvoiceDataCaptureToolkitState): void {
    syncInvoiceFieldsAnnotationWithInvoiceFields(state);
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== FETCH_INVOICE_DATA_ASYNC_ACTION
    );
}
