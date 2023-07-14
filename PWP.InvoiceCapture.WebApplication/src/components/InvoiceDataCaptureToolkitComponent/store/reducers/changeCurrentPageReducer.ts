import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function changeCurrentPageReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<{ pageNumber: number; autoScroll: boolean }>
): void {
    if (
        state.documentView.pages &&
        state.documentView.pages.length > 0 &&
        state.documentView.pages.find((page) => page.number === action.payload.pageNumber)
    ) {
        state.documentView.pages.forEach((page) => {
            if (action.payload.pageNumber === page.number) {
                page.current = true;
                page.autoScroll = action.payload.autoScroll;
                state.documentView.currentPageNumber = action.payload.pageNumber;
            } else {
                page.current = false;
                page.autoScroll = undefined;
            }
        });
    }
}
