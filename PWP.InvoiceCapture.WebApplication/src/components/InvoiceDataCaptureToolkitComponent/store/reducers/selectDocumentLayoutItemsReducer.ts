import { PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IDocumentView } from "../state";
import { sortLayoutItemsById } from "./helpers/sortLayoutItems";

export function selectDocumentLayoutItemsReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<string[]>
): void {
    const documentLayoutItemIds = action.payload;
    if (documentLayoutItemIds && state.documentView.pages && state.documentView.pages.length > 0) {
        const nextSelectedLayoutItemIds: string[] = [];

        state.documentView.pages.forEach((page) => {
            if (page.pageLayoutItems && page.pageLayoutItems.length > 0) {
                page.pageLayoutItems.forEach((layoutItem) => {
                    layoutItem.selected = documentLayoutItemIds.find((id) => layoutItem.id === id) !== undefined;

                    if (layoutItem.selected) {
                        nextSelectedLayoutItemIds.push(layoutItem.id);
                    }
                });
            }
        });

        state.documentView.selectedDocumentLayoutItemIds = nextSelectedLayoutItemIds;

        if (state.documentView.selectedDocumentLayoutItemIds.length > 0) {
            state.documentView.selectedPlainTextValue = selectedLayoutItemsPlainText(state.documentView);
            state.documentView.selectedAnnotationValue = selectedDocumentLayoutItemsAnnotationValue(state.documentView);
        } else {
            state.documentView.selectedPlainTextValue = undefined;
            state.documentView.selectedAnnotationValue = undefined;
        }
    }
}

function selectedLayoutItemsPlainText(state: IDocumentView): string {
    if (state.pages) {
        const selectedLayoutItems = _(state.pages)
            .flatMap((page) => page.pageLayoutItems || [])
            .filter((layoutItem) => layoutItem.selected)
            .value();

        const sortedLayoutItems = sortLayoutItemsById(selectedLayoutItems);

        return sortedLayoutItems.map((layoutItem) => layoutItem.value).join(" ");
    } else {
        return "";
    }
}

function selectedDocumentLayoutItemsAnnotationValue(state: IDocumentView): string {
    if (!state.pages) {
        return "";
    }

    const layoutItems = _(state.pages)
        .flatMap((page) => page.pageLayoutItems || [])
        .filter((layoutItem) => layoutItem.selected)
        .value();

    return layoutItems.map((layoutItem) => layoutItem.value).join(" ");
}
