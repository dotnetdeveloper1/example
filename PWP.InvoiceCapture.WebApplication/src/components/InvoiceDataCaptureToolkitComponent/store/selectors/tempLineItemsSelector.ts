import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { ILineItem } from "../state";

export function tempLineItemsSelector(state: IInvoiceDataCaptureToolkitState): ILineItem[] {
    if (state.invoiceFields && state.invoiceFields.tableTemporaryLineItems) {
        return state.invoiceFields.tableTemporaryLineItems;
    }

    return [];
}
