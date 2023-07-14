import { PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { defaultDocumentViewState, IInvoiceDocumentPage } from "../state";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";

export function getInvoicePagesReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<IInvoiceDocumentPage[]>
): void {
    if (action.payload) {
        state.documentView = { ...defaultDocumentViewState };
        state.documentView.pages = action.payload;

        if (state.documentView.pages && state.documentView.pages.length > 0) {
            const firstPageNumber =
                _(state.documentView.pages)
                    .map((page) => page.number)
                    .min() || 0;

            state.documentView.pageCount =
                _(state.documentView.pages)
                    .map((page) => page.number)
                    .max() || 0;

            state.documentView.currentPageNumber = firstPageNumber;

            const firstPage = state.documentView.pages.find((page) => page.number === firstPageNumber);

            if (firstPage) {
                firstPage.current = true;
            }
        }

        updateDocumentLayoutItemsAssignment(state);
    }
}
