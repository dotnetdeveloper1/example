import { PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceLineItemAnnotation } from "../state";
import { updateDocumentLayoutItemsFocus } from "./common/updateDocumentLayoutItemsFocus";

export function selectInvoiceFieldDataAnnotationReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<string>
): void {
    if (state.invoiceDataAnnotation && _.isArray(state.invoiceDataAnnotation.invoiceFieldsAnnotation)) {
        deselectOtherOrdersLineItems(state.invoiceDataAnnotation.invoiceLineItemAnnotation);

        const hasSelectedFieldAnnotation =
            state.invoiceDataAnnotation.invoiceFieldsAnnotation.find(
                (annotation) => action.payload === annotation.fieldId
            ) !== undefined;

        const selectedFieldValue =
            state.invoiceFields && state.invoiceFields.invoiceFields
                ? state.invoiceFields.invoiceFields.find((field) => field.fieldId === action.payload)?.value
                : undefined;

        if (!hasSelectedFieldAnnotation) {
            state.invoiceDataAnnotation.invoiceFieldsAnnotation = [
                ...state.invoiceDataAnnotation.invoiceFieldsAnnotation,
                {
                    userCreated: true,
                    fieldId: action.payload,
                    fieldValue: selectedFieldValue || "",
                    documentLayoutItemIds: [],
                    selected: true
                }
            ];
        }

        state.invoiceDataAnnotation.invoiceFieldsAnnotation.forEach((annotation) => {
            annotation.selected = action.payload === annotation.fieldId;
        });
    }

    updateDocumentLayoutItemsFocus(state);
}

function deselectOtherOrdersLineItems(invoiceLineItemAnnotations: IInvoiceLineItemAnnotation[]): void {
    if (!invoiceLineItemAnnotations) {
        return;
    }
    const lineItemAnnotations = _(invoiceLineItemAnnotations)
        .flatMap((annotation) => annotation.lineItemAnnotations)
        .value();
    lineItemAnnotations.forEach((annotation) => {
        annotation.selected = false;
    });
}
