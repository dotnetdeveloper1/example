import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function toggleCompareBoxVisibilityReducer(state: IInvoiceDataCaptureToolkitState): void {
    state.documentView.compareBoxesVisible = !state.documentView.compareBoxesVisible;
}
