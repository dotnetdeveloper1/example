import { PayloadAction } from "@reduxjs/toolkit";
import { FieldType } from "../../../../api/models/InvoiceFields/FieldType";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import {
    IInvoiceDataAnnotation,
    IInvoiceFields,
    IInvoiceLineItemAnnotation,
    IInvoiceLineItemDataAnnotation,
    LineItemsFieldTypes
} from "../state";
import { IInvoiceLineItem } from "../state/IInvoiceLineItem";
import { ILayoutItem, ITableCell } from "../state/index";
import { updateDocumentLayoutItemsAssignment } from "./common/updateDocumentLayoutItemsAssignment";
import { updateDocumentLayoutItemsFocus } from "./common/updateDocumentLayoutItemsFocus";
import { getLineItemFieldNameFromLineItemFieldType } from "./helpers/getLineItemFieldNameFromLineItemFieldType";
import { getTextOrAnnotationValue } from "./helpers/getTextOrAnnotationValue";

const fieldsTypesToUseAnnotationValue: Array<string> = [
    LineItemsFieldTypes.price,
    LineItemsFieldTypes.lineTotal,
    LineItemsFieldTypes.quantity
];

export function assignTableColumnToInvoiceLineItemsFieldReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<{
        columnsRows: ITableCell[][];
        columnNumber: number;
        fieldType: LineItemsFieldTypes;
    }>
): void {
    const { columnsRows, columnNumber, fieldType } = action.payload;

    const invoiceFields = state.invoiceFields;
    const invoiceDataAnnotation = state.invoiceDataAnnotation;
    if (invoiceFields && invoiceDataAnnotation && invoiceDataAnnotation.invoiceLineItemAnnotation) {
        const layoutItemsInRows: ILayoutItem[][] = getColumnLayoutItemsRows(columnsRows, columnNumber);

        const isUnassignment = isUnassignmentAction(invoiceFields, fieldType);

        const layoutItemsIds = layoutItemsInRows.flatMap((cellLayoutItems) => cellLayoutItems.map((item) => item.id));
        const isAnotherColumnReassignment = !isSameColumnAssignment(invoiceDataAnnotation, fieldType, layoutItemsIds);

        layoutItemsInRows.forEach((cellLayoutItems) => {
            const rowLayoutItemsIds = cellLayoutItems.map((item) => item.id);
            removeInvoiceFieldsAssignments(invoiceDataAnnotation!, invoiceFields, rowLayoutItemsIds);
            removeLineItemsFieldsAssignments(fieldType, invoiceDataAnnotation!, invoiceFields, rowLayoutItemsIds);
        });

        updateLineItems(isUnassignment, invoiceFields, invoiceDataAnnotation, layoutItemsInRows, fieldType);

        if (isUnassignment && isAnotherColumnReassignment) {
            updateLineItems(false, invoiceFields, invoiceDataAnnotation, layoutItemsInRows, fieldType);
        }

        updateDocumentLayoutItemsAssignment(state);
        updateDocumentLayoutItemsFocus(state);
    }
}

function getColumnLayoutItemsRows(tableColumnsRows: ITableCell[][], selectedColumn: number): ILayoutItem[][] {
    if (!tableColumnsRows || selectedColumn < 0 || tableColumnsRows.length <= selectedColumn) {
        return [];
    }

    return tableColumnsRows[selectedColumn].map((cell) => cell.layoutItems);
}

function updateLineItems(
    isUnassignment: boolean,
    invoiceFields: IInvoiceFields,
    dataAnnotation: IInvoiceDataAnnotation,
    layoutItemsRows: ILayoutItem[][],
    lineItemFieldType: LineItemsFieldTypes
): boolean {
    if (!dataAnnotation || !layoutItemsRows) {
        return false;
    }

    const createNewLineItems =
        !invoiceFields.tableTemporaryLineItems || invoiceFields.tableTemporaryLineItems.length === 0;

    if (createNewLineItems) {
        tryAddNewLineItems(invoiceFields, layoutItemsRows, lineItemFieldType, dataAnnotation.invoiceLineItemAnnotation);
    } else {
        tryUpdateLineItems(
            isUnassignment,
            invoiceFields,
            layoutItemsRows,
            lineItemFieldType,
            dataAnnotation.invoiceLineItemAnnotation
        );
    }

    return true;
}

function tryAddNewLineItems(
    invoiceFields: IInvoiceFields,
    layoutItemsRows: ILayoutItem[][],
    lineItemFieldType: LineItemsFieldTypes,
    invoiceLineItemAnnotations: IInvoiceLineItemAnnotation[]
): void {
    let orderNumber = invoiceFields.lineItems.length;
    layoutItemsRows.forEach((cellLayoutItems) => {
        orderNumber++;

        const fieldName = getLineItemFieldNameFromLineItemFieldType(lineItemFieldType);

        const layoutItemsGroup = getTextOrAnnotationValue(
            cellLayoutItems,
            fieldsTypesToUseAnnotationValue.indexOf(lineItemFieldType) > -1 ? FieldType.Decimal : FieldType.String
        );

        const fieldValue = layoutItemsGroup.groupValue;

        const newLineItem: IInvoiceLineItem = {
            orderNumber: orderNumber,
            number: "",
            description: "",
            price: "" as any,
            quantity: "" as any,
            lineTotal: "" as any
        };
        (newLineItem as any)[fieldName!] = fieldValue;
        invoiceFields.lineItems = [...invoiceFields.lineItems, newLineItem];
        invoiceFields.tableTemporaryLineItems = [...invoiceFields.tableTemporaryLineItems, newLineItem];

        createDataAnnotations(
            layoutItemsGroup.layoutItemsIds,
            fieldValue,
            orderNumber,
            lineItemFieldType,
            invoiceLineItemAnnotations
        );
    });
}

function createDataAnnotations(
    cellLayoutItems: string[],
    fieldValue: string,
    orderNumber: number,
    lineItemFieldType: LineItemsFieldTypes,
    invoiceLineItemAnnotations: IInvoiceLineItemAnnotation[]
): void {
    invoiceLineItemAnnotations.push({
        orderNumber: orderNumber,
        lineItemAnnotations:
            cellLayoutItems.length > 0
                ? [getLineItemDataAnnotation(cellLayoutItems, lineItemFieldType, fieldValue)]
                : []
    });
}

function tryUpdateLineItems(
    isUnassignment: boolean,
    invoiceFields: IInvoiceFields,
    layoutItemsRows: ILayoutItem[][],
    lineItemFieldType: LineItemsFieldTypes,
    invoiceLineItemAnnotations: IInvoiceLineItemAnnotation[]
): void {
    let index = 0;
    layoutItemsRows.forEach((cellLayoutItems) => {
        const tempLineItem = invoiceFields.tableTemporaryLineItems[index];
        if (tempLineItem) {
            const lineItem = invoiceFields.lineItems.find((item) => item.orderNumber === tempLineItem.orderNumber);
            const fieldName = getLineItemFieldNameFromLineItemFieldType(lineItemFieldType);

            const layoutItemsGroup = getTextOrAnnotationValue(
                cellLayoutItems,
                fieldsTypesToUseAnnotationValue.indexOf(lineItemFieldType) > -1 ? FieldType.Decimal : FieldType.String
            );

            if (lineItem) {
                const fieldValue = isUnassignment ? "" : layoutItemsGroup.groupValue;

                (tempLineItem as any)[fieldName!] = fieldValue;
                (lineItem as any)[fieldName!] = fieldValue;

                if (!isUnassignment) {
                    const lineItemAnnotation = invoiceLineItemAnnotations.find(
                        (item) => item.orderNumber === lineItem.orderNumber
                    );
                    updateDataAnnotations(
                        layoutItemsGroup.layoutItemsIds,
                        fieldValue,
                        lineItemFieldType,
                        lineItemAnnotation
                    );
                }
            }
        }
        index++;
    });
}

function isUnassignmentAction(invoiceFields: IInvoiceFields, lineItemfieldType: LineItemsFieldTypes): boolean {
    if (invoiceFields.tableTemporaryLineItems && invoiceFields.tableTemporaryLineItems.length > 0) {
        return invoiceFields.tableTemporaryLineItems.some((tempLineItem) => {
            const fieldName = getLineItemFieldNameFromLineItemFieldType(lineItemfieldType);
            return (tempLineItem as any)[fieldName!] !== "";
        });
    }
    return true;
}

function updateDataAnnotations(
    cellLayoutItems: string[],
    fieldValue: string,
    lineItemFieldType: LineItemsFieldTypes,
    lineItemAnnotation: IInvoiceLineItemAnnotation | undefined
): void {
    if (lineItemAnnotation) {
        const lineItemDataAnnotation = getLineItemDataAnnotation(cellLayoutItems, lineItemFieldType, fieldValue);
        lineItemAnnotation.lineItemAnnotations = lineItemAnnotation.lineItemAnnotations.filter(
            (annotation) => annotation.fieldType !== lineItemFieldType
        );
        lineItemAnnotation.lineItemAnnotations = [...lineItemAnnotation.lineItemAnnotations, lineItemDataAnnotation];
    }
}

function getLineItemDataAnnotation(
    cellLayoutItems: string[],
    lineItemFieldType: LineItemsFieldTypes,
    fieldValue: string
): IInvoiceLineItemDataAnnotation {
    return {
        fieldType: lineItemFieldType,
        documentLayoutItemIds: cellLayoutItems,
        fieldValue: fieldValue,
        selected: false,
        userCreated: true
    };
}

function isSameColumnAssignment(
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    fieldType: LineItemsFieldTypes,
    selectedDocumentLayoutItemIds: string[]
): boolean {
    const lineItemAnnotations = invoiceDataAnnotation.invoiceLineItemAnnotation;
    if (!lineItemAnnotations) {
        return false;
    }

    return lineItemAnnotations.some((annotation) => {
        return annotation.lineItemAnnotations.some((lineItemAnnotation) => {
            return (
                lineItemAnnotation.fieldType === fieldType &&
                lineItemAnnotation.documentLayoutItemIds.some(
                    (layoutId) => selectedDocumentLayoutItemIds.find((id) => layoutId === id) !== undefined
                )
            );
        });
    });
}

function removeLineItemsFieldsAssignments(
    fieldType: LineItemsFieldTypes,
    invoiceDataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItemIds: string[]
): void {
    const lineItemAnnotations = invoiceDataAnnotation.invoiceLineItemAnnotation;
    if (!lineItemAnnotations) {
        return;
    }

    lineItemAnnotations.forEach((lineAnnotation) => {
        const isSelectedAnnotation = invoiceFields.tableTemporaryLineItems.some(
            (lineItem) => lineItem.orderNumber === lineAnnotation.orderNumber
        );

        const existingLineItemsAssignmentsFieldType = isSelectedAnnotation
            ? lineAnnotation.lineItemAnnotations.filter((annotation) => annotation.fieldType === fieldType)
            : [];

        const existingLineItemsAssignmentsInSelectedColumn = lineAnnotation.lineItemAnnotations.filter((annotation) =>
            annotation.documentLayoutItemIds.find(
                (layoutId) => selectedDocumentLayoutItemIds.find((id) => layoutId === id) !== undefined
            )
        );

        const existingLineItemsAssignments = [
            ...existingLineItemsAssignmentsFieldType,
            ...existingLineItemsAssignmentsInSelectedColumn
        ];

        existingLineItemsAssignments.forEach((assignment) => {
            assignment.userCreated = true;
            assignment.fieldValue = "";
            assignment.documentLayoutItemIds = [];

            const fieldName = getLineItemFieldNameFromLineItemFieldType(assignment.fieldType);

            invoiceFields?.lineItems.forEach((lineItem) => {
                if (lineAnnotation.orderNumber === lineItem.orderNumber) {
                    (lineItem as any)[fieldName!] = "";
                }
            });

            invoiceFields?.tableTemporaryLineItems.forEach((lineItem) => {
                if (lineAnnotation.orderNumber === lineItem.orderNumber) {
                    (lineItem as any)[fieldName!] = "";
                }
            });
        });
    });
}

function removeInvoiceFieldsAssignments(
    dataAnnotation: IInvoiceDataAnnotation,
    invoiceFields: IInvoiceFields,
    selectedDocumentLayoutItemIds: string[]
): void {
    const invoiceFieldsAnnotation = dataAnnotation.invoiceFieldsAnnotation;
    if (!invoiceFieldsAnnotation) {
        return;
    }
    const existingInvoiceAssignments = invoiceFieldsAnnotation.filter((annotation) =>
        annotation.documentLayoutItemIds.find(
            (layoutId) => selectedDocumentLayoutItemIds.find((id) => layoutId === id) !== undefined
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
