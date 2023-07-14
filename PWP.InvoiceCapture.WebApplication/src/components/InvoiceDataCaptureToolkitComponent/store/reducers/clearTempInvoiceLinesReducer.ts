import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function clearTempInvoiceLinesReducer(state: IInvoiceDataCaptureToolkitState): void {
    if (state && state.invoiceFields) {
        state.invoiceFields.tableTemporaryLineItems = [];
    }
}
