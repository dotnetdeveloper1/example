import { PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { FieldType } from "../../../../api/models/InvoiceFields/FieldType";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import {
    IInvoiceDataAnnotation,
    IInvoiceFields,
    IInvoiceLineItemAnnotation,
    ILayoutItem,
    LineItemsFieldTypes
} from "../state";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";
import { getLineItemFieldNameFromLineItemFieldType } from "./helpers/getLineItemFieldNameFromLineItemFieldType";
import { getTextOrAnnotationValue } from "./helpers/getTextOrAnnotationValue";
import { tryAssignSelectedLayoutItems } from "./helpers/tryAssignSelectedLayoutItems";

const fieldsTypesToUseAnnotationValue: Array<string> = [
    LineItemsFieldTypes.price,
    LineItemsFieldTypes.lineTotal,
    LineItemsFieldTypes.quantity
];

export function assignLayoutLineItemsToInvoiceFieldReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<{ fieldType: LineItemsFieldTypes; orderNumber: number }>
): void {
    const { fieldType, orderNumber } = action.payload;

    const fieldName: string = getLineItemFieldNameFromLineItemFieldType(fieldType);
    const selectedDocumentLayoutItems = _(state.documentView.pages)
        .flatMap((page) => page.pageLayoutItems || [])
        .filter((layoutItem) => layoutItem.selected)
        .value();
    const selectedPlainTextValue = state.documentView.selectedPlainTextValue;
    const selectedAnnotationValue = state.documentView.selectedAnnotationValue;
    const invoiceFields = state.invoiceFields;
    const invoiceDataAnnotation = state.invoiceDataAnnotation;
    const invoiceLineItemAnnotation = invoiceDataAnnotation?.invoiceLineItemAnnotation;

    if (
        selectedDocumentLayoutItems &&
        selectedDocumentLayoutItems.length > 0 &&
        _.isString(fieldName) &&
        selectedPlainTextValue &&
        selectedAnnotationValue &&
        invoiceFields &&
        invoiceDataAnnotation &&
        invoiceLineItemAnnotation
    ) {
        tryAddNewInvoiceLineItemAnnotation(invoiceLineItemAnnotation, orderNumber);

        removeExistingAssignments(invoiceDataAnnotation, invoiceFields, selectedDocumentLayoutItems);

        const filteredLayoutItemsGroup = getTextOrAnnotationValue(
            selectedDocumentLayoutItems,
            fieldsTypesToUseAnnotationValue.indexOf(fieldType) > -1 ? FieldType.Decimal : FieldType.String
        );
        updateLineItemFieldValue(filteredLayoutItemsGroup.groupValue, fieldName, invoiceFields, orderNumber);

        const existingLineItemAnnotation = invoiceLineItemAnnotation.find(
            (annotation) => annotation.orderNumber === orderNumber
        );

        if (existingLineItemAnnotation) {
            if (!existingLineItemAnnotation.lineItemAnnotations) {
                existingLineItemAnnotation.lineItemAnnotations = [];
            }

            if (
                !tryAssignSelectedLayoutItems(
                    existingLineItemAnnotation.lineItemAnnotations,
                    fieldType,
                    filteredLayoutItemsGroup.groupValue,
                    filteredLayoutItemsGroup.layoutItemsIds
                )
            ) {
                existingLineItemAnnotation.lineItemAnnotations = [
                    ...existingLineItemAnnotation.lineItemAnnotations,
                    {
                        userCreated: true,
                        fieldType: fieldType,
                        fieldValue: filteredLayoutItemsGroup.groupValue,
                        documentLayoutItemIds: [...filteredLayoutItemsGroup.layoutItemsIds],
                        selected: false
                    }
                ];
            }
        }
    }

    updateDocumentLayoutItemsAssignment(state);
}

function tryAddNewInvoiceLineItemAnnotation(
    lineItemAnnotations: IInvoiceLineItemAnnotation[],
    orderNumber: number
): boolean {
    const existingLineItemAnnotation = lineItemAnnotations.find((annotation) => annotation.orderNumber === orderNumber);
    if (existingLineItemAnnotation) {
        return false;
    }
    lineItemAnnotations.push({
        orderNumber: orderNumber,
        lineItemAnnotations: []
    });

    return true;
}

function removeExistingAssignments(
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItems: ILayoutItem[]
): void {
    removeLineItemsFieldsAssignments(invoiceDataAnnotation, invoiceFields, selectedDocumentLayoutItems);

    removeInvoiceFieldsAssignments(invoiceDataAnnotation, invoiceFields, selectedDocumentLayoutItems);
}

function removeLineItemsFieldsAssignments(
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItems: ILayoutItem[]
): void {
    const lineItemAnnotations = invoiceDataAnnotation.invoiceLineItemAnnotation;
    if (!lineItemAnnotations) {
        return;
    }

    lineItemAnnotations.forEach((lineAnnotation) => {
        const existingLineItemsAssignments = lineAnnotation.lineItemAnnotations.filter((annotation) =>
            annotation.documentLayoutItemIds.find(
                (layoutId) => selectedDocumentLayoutItems.find((layoutItem) => layoutId === layoutItem.id) !== undefined
            )
        );

        if (existingLineItemsAssignments) {
            existingLineItemsAssignments.forEach((assignment) => {
                assignment.userCreated = true;
                assignment.fieldValue = "";
                assignment.documentLayoutItemIds = [];
                const fieldName = getLineItemFieldNameFromLineItemFieldType(assignment.fieldType);

                invoiceFields?.lineItems.forEach((lineItem) => {
                    if (lineItem.orderNumber === lineAnnotation.orderNumber) {
                        (lineItem as any)[fieldName!] = "";
                    }
                });
            });
        }
    });
}

function removeInvoiceFieldsAssignments(
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItems: ILayoutItem[]
): void {
    const invoiceFieldsAnnotation = invoiceDataAnnotation.invoiceFieldsAnnotation;
    if (!invoiceFieldsAnnotation) {
        return;
    }

    const existingInvoiceAssignments = invoiceFieldsAnnotation.filter((annotation) =>
        annotation.documentLayoutItemIds.find(
            (layoutId) => selectedDocumentLayoutItems.find((layoutItem) => layoutId === layoutItem.id) !== undefined
        )
    );

    if (!existingInvoiceAssignments) {
        return;
    }

    existingInvoiceAssignments.forEach((assignment) => {
        assignment.userCreated = true;
        assignment.fieldValue = "";
        assignment.documentLayoutItemIds = [];
        const invoiceField = invoiceFields.invoiceFields.find((field) => field.fieldId === assignment.fieldId);
        if (invoiceField) {
            invoiceField.value = "";
        }
    });
}

function updateLineItemFieldValue(
    fieldValue: string,
    fieldName: string,
    invoiceFields: IInvoiceFields,
    orderNumber: number
): void {
    invoiceFields?.lineItems.forEach((lineItem) => {
        if (lineItem.orderNumber === orderNumber) {
            (lineItem as any)[fieldName!] = fieldValue;
        }
    });
}
