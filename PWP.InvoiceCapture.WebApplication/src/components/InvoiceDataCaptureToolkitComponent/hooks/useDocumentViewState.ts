import { useCallback } from "react";
import { useDispatch } from "react-redux";
import { ReactPropType } from "../../../helperTypes";
import {
    assignSelectedLayoutItemsToInvoiceField,
    assignSelectedLayoutLineItemsToInvoiceField,
    assignTableColumnToInvoiceLineItemsField,
    changeCurrentPage,
    selectDocumentLayoutItems
} from "../store/InvoiceDataCaptureToolkitStoreSlice";
import { documentViewSelector, useToolkitSelector } from "../store/selectors";
import { IDocumentView } from "../store/state";
import { Toolkit } from "../Toolkit";
import { tableSelectionModeSelector } from "./../store/selectors/index";

interface IDocumentViewHookResult {
    documentViewState: IDocumentView;
    onDocumentViewPageChange: ReactPropType<typeof Toolkit, "onPageChange">;
    onLayoutItemsSelect: ReactPropType<typeof Toolkit, "onLayoutItemsSelect">;
    onAssignItemsToInvoiceField: ReactPropType<typeof Toolkit, "onAssignItems">;
    onAssignLineItemsToInvoiceField: ReactPropType<typeof Toolkit, "onAssignLineItems">;
    onAssignTableColumnToInvoiceLineItemsField: ReactPropType<typeof Toolkit, "onAssignColumnToLineItems">;
    tableSelectionMode: boolean;
}

export function useDocumentViewState(): IDocumentViewHookResult {
    const dispatch = useDispatch();

    const documentViewState = useToolkitSelector(documentViewSelector);
    const tableSelectionMode = useToolkitSelector(tableSelectionModeSelector);

    const onDocumentViewPageChange = useCallback(
        (pageNumber: number, autoScroll: boolean): void => {
            dispatch(changeCurrentPage({ pageNumber: pageNumber, autoScroll: autoScroll }));
        },
        [dispatch]
    );

    const onLayoutItemsSelect = useCallback(
        (layoutItemIds: string[]): void => {
            dispatch(selectDocumentLayoutItems(layoutItemIds));
        },
        [dispatch]
    );

    const onAssignItemsToInvoiceField = useCallback(
        (fieldId: string) => {
            dispatch(assignSelectedLayoutItemsToInvoiceField(fieldId));
            dispatch(selectDocumentLayoutItems([]));
        },
        [dispatch]
    );

    const onAssignLineItemsToInvoiceField = useCallback(
        (lineFieldType, orderNumber): void => {
            dispatch(
                assignSelectedLayoutLineItemsToInvoiceField({ fieldType: lineFieldType, orderNumber: orderNumber })
            );
            dispatch(selectDocumentLayoutItems([]));
        },
        [dispatch]
    );

    const onAssignTableColumnToInvoiceLineItemsField = useCallback(
        (columnsRows, lineFieldType, columnNumber): void => {
            dispatch(
                assignTableColumnToInvoiceLineItemsField({
                    columnsRows: columnsRows,
                    fieldType: lineFieldType,
                    columnNumber: columnNumber
                })
            );
        },
        [dispatch]
    );

    return {
        documentViewState: documentViewState,
        onDocumentViewPageChange: onDocumentViewPageChange,
        onLayoutItemsSelect: onLayoutItemsSelect,
        onAssignItemsToInvoiceField: onAssignItemsToInvoiceField,
        onAssignLineItemsToInvoiceField: onAssignLineItemsToInvoiceField,
        onAssignTableColumnToInvoiceLineItemsField: onAssignTableColumnToInvoiceLineItemsField,
        tableSelectionMode: tableSelectionMode
    };
}
