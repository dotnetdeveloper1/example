import { PayloadAction } from "@reduxjs/toolkit";
import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDataAnnotation, IInvoiceFieldDataAnnotation } from "../state";
import { IInvoiceFields } from "../state/index";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";
import { getLineItemFieldNameFromLineItemFieldType } from "./helpers/getLineItemFieldNameFromLineItemFieldType";
import { getTextOrAnnotationValue } from "./helpers/getTextOrAnnotationValue";

export function assignLayoutItemsToInvoiceFieldReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<string>
): void {
    const fieldId = action.payload;

    const selectedDocumentLayoutItems = _(state.documentView.pages)
        .flatMap((page) => page.pageLayoutItems || [])
        .filter((layoutItem) => layoutItem.selected)
        .value();
    const selectedPlainTextValue = state.documentView.selectedPlainTextValue;
    const selectedAnnotationValue = state.documentView.selectedAnnotationValue;
    const invoiceField =
        state.invoiceFields && state.invoiceFields.invoiceFields
            ? state.invoiceFields?.invoiceFields.find((field) => field.fieldId === fieldId)
            : undefined;
    const invoiceDataAnnotation = state.invoiceDataAnnotation;
    const invoiceFieldsAnnotation = invoiceDataAnnotation && invoiceDataAnnotation.invoiceFieldsAnnotation;

    if (
        selectedDocumentLayoutItems &&
        selectedDocumentLayoutItems.length > 0 &&
        selectedPlainTextValue &&
        selectedAnnotationValue &&
        state.invoiceFields &&
        invoiceField &&
        invoiceDataAnnotation &&
        invoiceFieldsAnnotation
    ) {
        invoiceField.value = getTextOrAnnotationValue(selectedDocumentLayoutItems, invoiceField.fieldType).groupValue;

        removeExistingAssignments(
            invoiceDataAnnotation,
            state.invoiceFields,
            selectedDocumentLayoutItems.map((layoutItem) => layoutItem.id),
            fieldId
        );

        const filteredLayoutItemsGroup = getTextOrAnnotationValue(selectedDocumentLayoutItems, invoiceField.fieldType);

        if (
            !tryAssignSelectedLayoutItems(
                invoiceFieldsAnnotation,
                fieldId,
                filteredLayoutItemsGroup.groupValue,
                filteredLayoutItemsGroup.layoutItemsIds
            )
        ) {
            invoiceDataAnnotation.invoiceFieldsAnnotation = [
                ...invoiceDataAnnotation.invoiceFieldsAnnotation,
                {
                    userCreated: true,
                    fieldId: fieldId,
                    fieldValue: filteredLayoutItemsGroup.groupValue,
                    documentLayoutItemIds: [...filteredLayoutItemsGroup.layoutItemsIds],
                    selected: false
                }
            ];
        }
    }

    updateDocumentLayoutItemsAssignment(state);
}

function tryAssignSelectedLayoutItems(
    invoiceFieldsAnnotation: IInvoiceFieldDataAnnotation[],
    fieldId: string,
    selectedAnnotationValue: string,
    selectedDocumentLayoutItemsIds: string[]
): boolean {
    const fieldDataAnnotations = invoiceFieldsAnnotation.filter((annotation) => annotation.fieldId === fieldId);

    if (fieldDataAnnotations && fieldDataAnnotations.length > 0) {
        fieldDataAnnotations.forEach((assignment) => {
            assignment.userCreated = true;
            assignment.fieldValue = selectedAnnotationValue;
            assignment.documentLayoutItemIds = [...selectedDocumentLayoutItemsIds];
        });

        return true;
    }

    return false;
}

function removeExistingAssignments(
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItemIds: string[],
    fieldIdToAssign: string
): void {
    removeInvoiceFieldsAssignments(
        invoiceDataAnnotation,
        invoiceFields,
        selectedDocumentLayoutItemIds,
        fieldIdToAssign
    );
    removeLineItemsFieldsAssignments(invoiceDataAnnotation, invoiceFields, selectedDocumentLayoutItemIds);
}

// TODO: move to common helper
function removeInvoiceFieldsAssignments(
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItemIds: string[],
    fieldIdToAssign: string
): void {
    const invoiceFieldsAnnotation = invoiceDataAnnotation.invoiceFieldsAnnotation;
    if (!invoiceFieldsAnnotation) {
        return;
    }

    const existingAssignments = invoiceFieldsAnnotation.filter((annotation) =>
        annotation.documentLayoutItemIds.find(
            (layoutId) => selectedDocumentLayoutItemIds.find((id) => layoutId === id) !== undefined
        )
    );

    if (!existingAssignments) {
        return;
    }

    existingAssignments.forEach((assignment) => {
        assignment.userCreated = true;
        assignment.fieldValue = "";
        assignment.documentLayoutItemIds = [];
        if (fieldIdToAssign !== assignment.fieldId) {
            const invoiceField = invoiceFields.invoiceFields.find((field) => field.fieldId === assignment.fieldId);
            if (invoiceField !== undefined) {
                invoiceField.value = "";
            }
        }
    });
}

function removeLineItemsFieldsAssignments(
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItemIds: string[]
): void {
    const invoiceLineItemAnnotation = invoiceDataAnnotation.invoiceLineItemAnnotation;
    if (!invoiceLineItemAnnotation) {
        return;
    }

    const invoiceFieldsLayoutLineItems = _(invoiceDataAnnotation.invoiceLineItemAnnotation)
        .flatMap((fieldAnnotation) => fieldAnnotation.lineItemAnnotations)
        .value();

    if (!invoiceFieldsLayoutLineItems) {
        return;
    }

    invoiceDataAnnotation.invoiceLineItemAnnotation.forEach((lineItemAnnotation) => {
        const existingLineItemsAssignments = lineItemAnnotation.lineItemAnnotations.filter((annotation) =>
            annotation.documentLayoutItemIds.find(
                (layoutId) => selectedDocumentLayoutItemIds.find((id) => layoutId === id) !== undefined
            )
        );

        if (!existingLineItemsAssignments) {
            return;
        }

        existingLineItemsAssignments.forEach((assignment) => {
            assignment.userCreated = true;
            assignment.fieldValue = "";
            assignment.documentLayoutItemIds = [];
            const fieldName = getLineItemFieldNameFromLineItemFieldType(assignment.fieldType);
            cleanLineItems(fieldName, invoiceFields, lineItemAnnotation.orderNumber);
        });
    });
}

function cleanLineItems(fieldName: string, invoiceFields: IInvoiceFields, orderNumber: number): void {
    invoiceFields?.lineItems.forEach((lineItem) => {
        if (lineItem.orderNumber === orderNumber) {
            (lineItem as any)[fieldName!] = "";
        }
    });
}
