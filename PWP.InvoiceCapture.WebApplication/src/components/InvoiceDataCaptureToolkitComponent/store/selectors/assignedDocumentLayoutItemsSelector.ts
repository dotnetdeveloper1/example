import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDataAnnotation } from "../state";

export function assignedDocumentLayoutItemsSelector(state: IInvoiceDataCaptureToolkitState): string[] {
    return state.invoiceDataAnnotation && _.isArray(state.invoiceDataAnnotation.invoiceFieldsAnnotation)
        ? selectAssignedDocumentLayoutItems(state.invoiceDataAnnotation)
        : [];
}

function selectAssignedDocumentLayoutItems(invoiceDataAnnotation: IInvoiceDataAnnotation): string[] {
    const invoiceFieldsLayoutItemIds = _(invoiceDataAnnotation.invoiceFieldsAnnotation)
        .flatMap((fieldAnnotation) => fieldAnnotation.documentLayoutItemIds)
        .sortedUniq()
        .value();

    if (_.isArray(invoiceDataAnnotation.invoiceLineItemAnnotation)) {
        const invoiceFieldsLayoutLineItems = _(invoiceDataAnnotation.invoiceLineItemAnnotation)
            .flatMap((fieldAnnotation) => fieldAnnotation.lineItemAnnotations)
            .value();

        if (_.isArray(invoiceFieldsLayoutLineItems)) {
            const invoiceFieldsLayoutLineItemIds = _(invoiceFieldsLayoutLineItems)
                .flatMap((fieldAnnotation) => fieldAnnotation.documentLayoutItemIds)
                .sortedUniq()
                .value();

            return invoiceFieldsLayoutLineItemIds.concat(invoiceFieldsLayoutItemIds);
        }
    }

    return invoiceFieldsLayoutItemIds;
}
