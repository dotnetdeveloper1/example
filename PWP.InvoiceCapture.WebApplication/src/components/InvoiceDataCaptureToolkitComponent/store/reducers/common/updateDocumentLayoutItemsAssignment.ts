import { IInvoiceDataCaptureToolkitState } from "../../InvoiceDataCaptureToolkitState";
import { assignedDocumentLayoutItemsPerPageSelector } from "../../selectors";

export function updateDocumentLayoutItemsAssignment(state: IInvoiceDataCaptureToolkitState): void {
    if (state.documentView.pages && state.documentView.pages.length > 0) {
        const assignedDocumentLayoutItemsPerPage = assignedDocumentLayoutItemsPerPageSelector(state);

        state.documentView.pages.forEach((page) => {
            if (page.pageLayoutItems && page.pageLayoutItems.length > 0) {
                const assignedPageLayoutItems = assignedDocumentLayoutItemsPerPage[page.number];

                page.pageLayoutItems.forEach((layoutItem) => {
                    layoutItem.assigned =
                        assignedPageLayoutItems &&
                        assignedPageLayoutItems.length > 0 &&
                        assignedPageLayoutItems.find((id) => layoutItem.id === id) !== undefined;
                });
            }
        });
    }
}
