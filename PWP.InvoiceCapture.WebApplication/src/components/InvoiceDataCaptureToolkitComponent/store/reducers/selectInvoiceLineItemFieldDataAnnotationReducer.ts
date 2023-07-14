import { PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { LineItemsFieldTypes } from "../state/LineItemsFieldTypes";
import { updateDocumentLayoutItemsFocus } from "./common/updateDocumentLayoutItemsFocus";
import { getLineItemFieldNameFromLineItemFieldType } from "./helpers/getLineItemFieldNameFromLineItemFieldType";

export function selectInvoiceLineItemFieldDataAnnotationReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<{ fieldType: LineItemsFieldTypes; orderNumber: number }>
): void {
    const { fieldType, orderNumber } = action.payload;

    deselectOtherOrdersLineItems(state, orderNumber);

    selectLineItemFieldAnnotation(state, fieldType, orderNumber);

    updateDocumentLayoutItemsFocus(state);
}

function selectLineItemFieldAnnotation(
    state: IInvoiceDataCaptureToolkitState,
    fieldType: LineItemsFieldTypes,
    orderNumber: number
): void {
    if (!state || !state.invoiceDataAnnotation || !_.isArray(state.invoiceDataAnnotation.invoiceLineItemAnnotation)) {
        return;
    }

    const lineItemAnnotation = state.invoiceDataAnnotation.invoiceLineItemAnnotation.find(
        (annotation) => annotation.orderNumber === orderNumber
    );
    if (!lineItemAnnotation || !_.isArray(lineItemAnnotation.lineItemAnnotations)) {
        return;
    }

    const hasSelectedFieldAnnotation =
        lineItemAnnotation.lineItemAnnotations.find((annotation) => fieldType === annotation.fieldType) !== undefined;

    lineItemAnnotation.lineItemAnnotations.forEach((annotation) => {
        annotation.selected = fieldType === annotation.fieldType;
    });

    const fieldName = getLineItemFieldNameFromLineItemFieldType(fieldType);

    if (!hasSelectedFieldAnnotation && fieldName) {
        const lineItem = state.invoiceFields?.lineItems?.find(
            (invoiceLineItem) => invoiceLineItem.orderNumber === orderNumber
        );
        const lineItemValue = (lineItem as any)[fieldName!];

        lineItemAnnotation.lineItemAnnotations = [
            ...lineItemAnnotation.lineItemAnnotations,
            {
                userCreated: true,
                fieldType: fieldType,
                fieldValue: lineItemValue ? lineItemValue.toString() : "",
                documentLayoutItemIds: [],
                selected: true
            }
        ];
    }
}

function deselectOtherOrdersLineItems(state: IInvoiceDataCaptureToolkitState, orderNumber: number): void {
    if (!state || !state.invoiceDataAnnotation || !_.isArray(state.invoiceDataAnnotation.invoiceLineItemAnnotation)) {
        return;
    }

    const invoiceLineItemAnnotations = state.invoiceDataAnnotation.invoiceLineItemAnnotation;

    const otherOrdersLineItemAnnotations = _(invoiceLineItemAnnotations)
        .filter((annotation) => annotation.orderNumber !== orderNumber)
        .flatMap((annotation) => annotation.lineItemAnnotations)
        .value();
    otherOrdersLineItemAnnotations.forEach((annotation) => {
        annotation.selected = false;
    });
}
