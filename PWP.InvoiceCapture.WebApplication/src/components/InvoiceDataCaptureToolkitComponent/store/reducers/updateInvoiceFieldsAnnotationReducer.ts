import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { ILineItem, LineItemsFieldTypesKeyMap } from "./../state/index";

export function updateInvoiceFieldsAnnotationReducer(state: IInvoiceDataCaptureToolkitState): void {
    if (state.invoiceDataAnnotation && _.isArray(state.invoiceDataAnnotation.invoiceFieldsAnnotation)) {
        const invoiceFieldsAnnotation = state.invoiceDataAnnotation.invoiceFieldsAnnotation;
        const invoiceLinesFieldsAnnotation = state.invoiceDataAnnotation.invoiceLineItemAnnotation;
        const invoiceFields = state.invoiceFields;

        if (invoiceFields && invoiceFields.invoiceFields) {
            invoiceFieldsAnnotation.forEach((annotation) => {
                const invoiceField = invoiceFields.invoiceFields.find((field) => field.fieldId === annotation.fieldId);

                if (invoiceField && annotation.fieldValue !== invoiceField.value) {
                    annotation.fieldValue = invoiceField.value || "";
                    annotation.userCreated = true;
                }
            });
        }

        if (invoiceLinesFieldsAnnotation && invoiceFields && invoiceFields.lineItems) {
            invoiceLinesFieldsAnnotation.forEach((annotations) => {
                annotations.lineItemAnnotations.forEach((annotation) => {
                    const lineItemFields = invoiceFields.lineItems.filter(
                        (lineItem) => lineItem.orderNumber === annotations.orderNumber
                    )[0];
                    const lineFieldType: keyof ILineItem | undefined = LineItemsFieldTypesKeyMap.get(
                        annotation.fieldType
                    );
                    if (
                        lineItemFields &&
                        lineFieldType &&
                        annotation.fieldValue !== (lineItemFields as any)[lineFieldType]
                    ) {
                        annotation.fieldValue = (lineItemFields as any)[lineFieldType].toString();
                        annotation.userCreated = true;
                    }
                });
            });
        }
    }
}
