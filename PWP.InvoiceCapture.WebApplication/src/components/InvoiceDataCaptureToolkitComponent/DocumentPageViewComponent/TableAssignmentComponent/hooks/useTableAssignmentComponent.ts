import { useCallback, useEffect, useMemo, useState } from "react";
import { LayoutItemsTableService } from "../../../../../services/layoutItemsTableService";
import { BrowserIdentifier } from "../../../../../utils/browserIdentifier";
import { isOutOfComponent } from "../../../store/reducers/helpers/isOutOfComponent";
import { IInvoiceTable } from "../../../store/state/IInvoiceTable";
import { ITableMenuState } from "../TableAssignmentComponent";

interface ITableAssignmentComponentHookResult {
    tableMenuState: ITableMenuState;
    onTableMenuClosed: () => void;
    handleContextMenu: (event: React.MouseEvent<Element, MouseEvent> | MouseEvent) => void;
}

export function useTableAssignmentComponent(
    tableData: IInvoiceTable | undefined,
    tableRef: React.RefObject<HTMLDivElement>,
    multiselectionComponentName: string
): ITableAssignmentComponentHookResult {
    const [tableMenuState, setTableMenuState] = useState<ITableMenuState>(getDefaultTableMenuState());

    const isDisabled = useMemo(
        () => tableData === undefined || tableData.dividers.some((divider) => !divider.isValid),
        [tableData]
    );

    const getTableMenuLeftCoordinate = useCallback(
        (tableMenuLeft: number) => {
            const multiselectionElements = document.getElementsByClassName(multiselectionComponentName);
            if (!multiselectionElements || !multiselectionElements[0]) {
                return tableMenuLeft;
            }

            const selectionPane = multiselectionElements[0] as HTMLDivElement;
            const multiSelectionLeft = selectionPane ? selectionPane.getBoundingClientRect().left : 0;

            return tableMenuLeft - multiSelectionLeft;
        },
        [multiselectionComponentName]
    );

    const getColumnIndex = useCallback(
        (tableMenuLeft: number): number | undefined => {
            if (!tableData) {
                return undefined;
            }
            const layoutItemsTableService = new LayoutItemsTableService();
            return layoutItemsTableService.getColumnNumber(
                { x: getTableMenuLeftCoordinate(tableMenuLeft), y: 0 },
                tableData?.columnsRows
            );
        },
        [getTableMenuLeftCoordinate, tableData]
    );

    const openMenu = useCallback(
        (event: MouseEvent) => {
            setTableMenuState({
                isOpen: true,
                top: event.clientY,
                left: event.clientX,
                columnIndex: getColumnIndex(event.clientX)
            });
        },
        [getColumnIndex]
    );

    const closeMenu = useCallback(() => {
        setTableMenuState(getDefaultTableMenuState());
    }, [setTableMenuState]);

    const handleContextMenu = useCallback(
        (event: React.MouseEvent<Element, MouseEvent> | MouseEvent) => {
            if (isDisabled) {
                event.preventDefault();
                return;
            }
            // In safari, it seems like the focus is given to the context menu when right-clicking, so the context menu receives the mouseup event rather than the target element.
            if (BrowserIdentifier.IsSafari()) {
                event.preventDefault();
            }
            const mouseEvent = event as MouseEvent;
            if (!mouseEvent || isOutOfComponent(tableRef?.current, mouseEvent)) {
                closeMenu();
            } else {
                event.preventDefault();
                openMenu(mouseEvent);
            }
        },
        [closeMenu, isDisabled, openMenu, tableRef]
    );

    useEffect(() => {
        tableRef?.current?.addEventListener("contextmenu", handleContextMenu);
        return () => {
            tableRef?.current?.removeEventListener("contextmenu", handleContextMenu);
        };
    }, [handleContextMenu, tableRef]);

    return { tableMenuState: tableMenuState, onTableMenuClosed: closeMenu, handleContextMenu: handleContextMenu };

    function getDefaultTableMenuState(): ITableMenuState {
        return {
            isOpen: false,
            left: 0,
            top: 0,
            columnIndex: undefined
        };
    }
}
