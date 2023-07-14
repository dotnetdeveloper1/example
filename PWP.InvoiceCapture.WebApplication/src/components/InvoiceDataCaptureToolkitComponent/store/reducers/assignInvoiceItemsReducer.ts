import { PayloadAction } from "@reduxjs/toolkit";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceLineItemDataAnnotation } from "./../state/index";
import { LineItemsFieldTypesKeyMap } from "./../state/LineItemsFieldTypes";

export function assignInvoiceItemsReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<{ orderNumber: number }>
): void {
    if (!state.invoiceFields) {
        return;
    }

    const { orderNumber } = action.payload;

    state.invoiceFields.lineItems = [
        ...state.invoiceFields.lineItems,
        {
            orderNumber: orderNumber,
            number: "",
            description: "",
            price: "" as any,
            quantity: "" as any,
            lineTotal: "" as any
        }
    ];

    const emptyLineAnnotation: IInvoiceLineItemDataAnnotation[] = [];

    LineItemsFieldTypesKeyMap.forEach((value, key, map) => {
        emptyLineAnnotation.push({
            fieldType: key,
            fieldValue: "",
            userCreated: true,
            selected: false,
            documentLayoutItemIds: []
        });
    });

    if (state.invoiceDataAnnotation) {
        state.invoiceDataAnnotation.invoiceLineItemAnnotation = [
            ...state.invoiceDataAnnotation.invoiceLineItemAnnotation,
            {
                orderNumber: orderNumber,
                lineItemAnnotations: emptyLineAnnotation
            }
        ];
    }
}
