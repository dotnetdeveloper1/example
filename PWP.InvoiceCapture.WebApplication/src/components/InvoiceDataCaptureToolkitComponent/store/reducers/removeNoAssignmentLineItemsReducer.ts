import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceLineItem } from "../state/IInvoiceLineItem";
import { deleteInvoiceItem } from "./helpers/deleteInvoiceItem";
import { isEmptyLineItem } from "./helpers/isEmptyLineItem";

export function removeNoAssignmentLineItemsReducer(state: IInvoiceDataCaptureToolkitState): void {
    if (state && state.invoiceFields) {
        const fields = state.invoiceFields;

        if (fields && fields.lineItems) {
            const maxIterationCount = 10000;
            let iteration = 0;
            let emptyLineItem: IInvoiceLineItem | undefined;

            do {
                iteration++;
                if (iteration > maxIterationCount) {
                    break;
                }

                emptyLineItem = _(fields.lineItems).find((item) => isEmptyLineItem(item));
                if (emptyLineItem) {
                    deleteInvoiceItem(emptyLineItem.orderNumber, state);
                }
            } while (emptyLineItem);
        }
    }
}
