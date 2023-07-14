import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function confirmErrorReducer(state: IInvoiceDataCaptureToolkitState): void {
    if (state.error) {
        state.error.confirm = true;
    }
}
