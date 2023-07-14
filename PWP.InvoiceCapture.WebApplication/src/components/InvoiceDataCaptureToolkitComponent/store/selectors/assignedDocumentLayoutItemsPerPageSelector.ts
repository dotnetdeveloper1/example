import _ from "lodash";
import { assignedDocumentLayoutItemsSelector } from ".";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDocumentPage } from "../state";

export interface IDocumentLayoutItemsPerPageLookup {
    [pageNumber: number]: string[];
}

export function assignedDocumentLayoutItemsPerPageSelector(
    state: IInvoiceDataCaptureToolkitState
): IDocumentLayoutItemsPerPageLookup {
    const assignedItemsPerPageArray = _.isArray(state.documentView.pages)
        ? _(state.documentView.pages)
              .map(selectDocumentLayoutItemsForPage(assignedDocumentLayoutItemsSelector(state)))
              .value()
        : [];

    if (assignedItemsPerPageArray && assignedItemsPerPageArray.length > 0) {
        return _.zipObject(
            _.map(assignedItemsPerPageArray, "pageNumber"),
            _.map(assignedItemsPerPageArray, "documentLayoutIds")
        );
    }

    return {};
}

function selectDocumentLayoutItemsForPage(
    assignedDocumentLayoutItems: string[]
): (page: IInvoiceDocumentPage) => { pageNumber: number; documentLayoutIds: string[] } {
    return (page) => ({
        pageNumber: page.number,
        documentLayoutIds: _.isArray(page.pageLayoutItems)
            ? _(page.pageLayoutItems)
                  .map((layoutItem) => layoutItem.id)
                  .filter((layoutItemId) => !_.isEmpty(assignedDocumentLayoutItems.find((id) => id === layoutItemId)))
                  .value()
            : []
    });
}
