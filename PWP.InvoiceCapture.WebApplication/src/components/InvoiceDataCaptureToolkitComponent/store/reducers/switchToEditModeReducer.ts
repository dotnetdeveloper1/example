import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDataAnnotation, IInvoiceFields } from "../state";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";
import { updateDocumentLayoutItemsFocus } from "./common/updateDocumentLayoutItemsFocus";
import { deleteInvoiceItem } from "./helpers/deleteInvoiceItem";

export function switchToEditModeReducer(state: IInvoiceDataCaptureToolkitState): void {
    if (state && state.invoiceFields) {
        let tempLineItems = _(state.invoiceFields.tableTemporaryLineItems)
            .sortBy((item) => item.orderNumber)
            .value();

        const maxIterationCount = 10000;
        let iteration = 0;

        while (tempLineItems && tempLineItems.length > 0) {
            iteration++;
            if (iteration > maxIterationCount) {
                break;
            }

            const tempLineItem = tempLineItems[tempLineItems.length - 1];
            deleteInvoiceItem(tempLineItem.orderNumber, state);

            tempLineItems = tempLineItems.filter((tempItem) => tempItem.orderNumber !== tempLineItem.orderNumber);
        }

        state.invoiceFields.tableTemporaryLineItems = state.invoiceFields.tableTemporaryLineItems.filter(
            (stateTempItem) => tempLineItems.some((tempItem) => tempItem.orderNumber === stateTempItem.orderNumber)
        );

        removeLineItemsFieldsAssignments(state.invoiceDataAnnotation, state.invoiceFields);

        updateDocumentLayoutItemsAssignment(state);
        updateDocumentLayoutItemsFocus(state);
    }
}

function removeLineItemsFieldsAssignments(
    invoiceDataAnnotation: IInvoiceDataAnnotation | undefined,
    invoiceFields: IInvoiceFields
): void {
    if (invoiceFields && invoiceDataAnnotation) {
        const lineItemAnnotations = invoiceDataAnnotation?.invoiceLineItemAnnotation;
        invoiceDataAnnotation.invoiceLineItemAnnotation = lineItemAnnotations.filter((annotation) =>
            invoiceFields.lineItems.some((item) => item.orderNumber === annotation.orderNumber)
        );
    }
}
