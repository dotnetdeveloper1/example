import classNames from "classnames";
import _ from "lodash";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import {
    clearEmptyDataAnnotations,
    deleteInvoiceItems,
    removeNoAssignmentLineItems,
    switchToEditMode
} from "../../store/InvoiceDataCaptureToolkitStoreSlice";
import { lineItemAnnotationsSelector, tempLineItemsSelector, useToolkitSelector } from "../../store/selectors";
import {
    IBox,
    IInvoiceLineItemAnnotation,
    IInvoiceTable,
    ILayoutItem,
    ILineItem,
    IPoint,
    ITableCell,
    ITableDivider,
    LineItemsFieldTypes
} from "../../store/state";
import { DividerOrientation } from "../../store/state/ITableDivider";
import { COMPONENT_NAME as MULTI_SELECTION_COMPONENT_NAME } from "../MultiSelectionPaneComponent";
import { LayoutItemsTableService } from "./../../../../services/layoutItemsTableService";
import { ColumnHighlightComponent } from "./ColumnHighlightComponent/ColumnHighlightComponent";
import { useTableAssignmentComponent } from "./hooks/useTableAssignmentComponent";
import { SwitchButtonComponent } from "./SwitchButtonComponent/SwitchButtonComponent";
import { SwitchToEditModeComponent } from "./SwitchToEditModeComponent/SwitchToEditModeComponent";
import "./TableAssignmentComponent.scss";
import { TableDividerComponent } from "./TableDividerComponent/TableDividerComponent";
import { TableMenuComponent } from "./TableMenuComponent/TableMenuComponent";
import { TablePositionService } from "./tablePositionService";

export interface DraftDivider {
    orientation: DividerOrientation;
    top: number;
    left: number;
    visible: boolean;
}

interface IAssignedColumn {
    columnIndex: number;
    fieldName: string;
    tableCells: ITableCell[];
    topLeft: IPoint;
    bottomRight: IPoint;
}

interface TableAssignmentProps {
    selectedTable: IBox;
    containerScrollOffset: IPoint;
    layoutItems: ILayoutItem[];
    onAssignColumnToLineItems(
        columnsRows: ITableCell[][],
        fieldType: LineItemsFieldTypes,
        orderNumber: number,
        scaleRatio: number
    ): void;
    onTooltipWasShown(): void;
    tooltipWasShown: boolean;
    scaleRatio: number;
    initialScaleRatio: number;
    currentPageOffset: IPoint;
}

interface ITableAssignmentComponentState {
    table: IBox | undefined;
    assignedColumns: IAssignedColumn[];
    dividers: ITableDivider[];
    tableData?: IInvoiceTable;
    draftHorizontalDivider: DraftDivider;
    draftVerticalDivider: DraftDivider;
    dividerDragMode: boolean;
    editMode: boolean;
    useAutoRecognizedHeader: boolean;
}

export interface ITableMenuState {
    isOpen: boolean;
    top: number;
    left: number;
    columnIndex?: number;
}

export const COMPONENT_NAME = "TableAssignmentComponent";

export const TableAssignmentComponent: React.FunctionComponent<TableAssignmentProps> = React.memo((props) => {
    const { t } = useTranslation();

    const tableAssignmentComponentRef = useRef<HTMLDivElement>(null);
    const switchToEditModeAreaComponentRef = useRef<HTMLDivElement>(null);

    const dispatch = useDispatch();

    const assignedLineItems = useToolkitSelector(tempLineItemsSelector);
    const lineItemsAssignments = useToolkitSelector(lineItemAnnotationsSelector);

    const tablePositionService: TablePositionService = new TablePositionService(
        props.initialScaleRatio,
        props.scaleRatio,
        props.currentPageOffset,
        props.selectedTable,
        props.containerScrollOffset
    );

    const [state, setState] = useState<ITableAssignmentComponentState>({
        table: undefined,
        assignedColumns: [],
        dividers: [],
        draftHorizontalDivider: { orientation: DividerOrientation.Horizontal, left: 0, top: 0, visible: false },
        draftVerticalDivider: { orientation: DividerOrientation.Vertical, left: 0, top: 0, visible: false },
        dividerDragMode: false,
        editMode: true,
        useAutoRecognizedHeader: false
    });

    const { tableMenuState, onTableMenuClosed, handleContextMenu } = useTableAssignmentComponent(
        state.tableData,
        tableAssignmentComponentRef,
        MULTI_SELECTION_COMPONENT_NAME
    );

    const tableStyles = [COMPONENT_NAME];

    const autoAssignTableColumns = useCallback(
        (tableData: IInvoiceTable | undefined, skipManuallyAssignedColumns: boolean) => {
            if (tableData && tableData.hasHeader()) {
                // tslint:disable-next-line: prefer-for-of
                for (let columnIndex = 0; columnIndex < tableData.columnsRows.length; columnIndex++) {
                    const autoRecognizedColumnType = tableData.columnsRows[columnIndex][0].autoRecognizedColumnType;
                    if (
                        autoRecognizedColumnType !== undefined &&
                        !(
                            skipManuallyAssignedColumns &&
                            (state.assignedColumns.some((column) => column.fieldName === autoRecognizedColumnType) ||
                                state.assignedColumns.some((column) => column.columnIndex === columnIndex))
                        )
                    ) {
                        props.onAssignColumnToLineItems(
                            tableData.columnsRows.map((row) => row.slice(1)),
                            autoRecognizedColumnType,
                            columnIndex,
                            props.scaleRatio
                        );
                    }
                }
            }
        },
        [props, state.assignedColumns]
    );

    const initTableData = useCallback(() => {
        const layoutItemsTableService = new LayoutItemsTableService();
        const topLeft = { x: props.selectedTable.left, y: props.selectedTable.top };
        const bottomRight = {
            x: props.selectedTable.left + props.selectedTable.width,
            y: props.selectedTable.top + props.selectedTable.height
        };
        const selectedLayoutItems = layoutItemsTableService.getSelectedLayoutItems(
            topLeft,
            bottomRight,
            props.layoutItems
        );

        const dividers = layoutItemsTableService.getDividers(selectedLayoutItems).map((divider) => ({
            ...divider,
            height: divider.orientation === DividerOrientation.Horizontal ? 3 : props.selectedTable.height,
            width: divider.orientation === DividerOrientation.Horizontal ? props.selectedTable.width : 3
        }));

        const tableData = layoutItemsTableService.getTable(dividers, selectedLayoutItems, topLeft, bottomRight);

        if (tableData.hasHeader()) {
            autoAssignTableColumns(tableData, false);
        }

        setState((prevState) => ({
            ...prevState,
            dividers: dividers,
            selectedLayoutItems: selectedLayoutItems,
            tableData: tableData,
            useAutoRecognizedHeader: tableData.hasHeader()
        }));
    }, [
        autoAssignTableColumns,
        props.layoutItems,
        props.selectedTable.height,
        props.selectedTable.left,
        props.selectedTable.top,
        props.selectedTable.width
    ]);

    const updateTableData = useCallback(() => {
        const layoutItemsTableService = new LayoutItemsTableService();
        const topLeft = {
            x: (props.selectedTable.left * props.scaleRatio) / props.initialScaleRatio,
            y: (props.selectedTable.top * props.scaleRatio) / props.initialScaleRatio
        };
        const bottomRight = {
            x:
                (props.selectedTable.left * props.scaleRatio) / props.initialScaleRatio +
                (props.selectedTable.width * props.scaleRatio) / props.initialScaleRatio,
            y:
                (props.selectedTable.top * props.scaleRatio) / props.initialScaleRatio +
                (props.selectedTable.height * props.scaleRatio) / props.initialScaleRatio
        };

        const selectedLayoutItems = layoutItemsTableService.getSelectedLayoutItems(
            topLeft,
            bottomRight,
            props.layoutItems
        );

        const tableData = layoutItemsTableService.getTable(
            state.dividers.map((divider) => ({
                ...divider,
                left: (divider.left * props.scaleRatio) / props.initialScaleRatio,
                top: (divider.top * props.scaleRatio) / props.initialScaleRatio
            })),
            selectedLayoutItems,
            topLeft,
            bottomRight
        );
        setState((prevState) => ({
            ...prevState,
            tableData: tableData
        }));
    }, [
        props.initialScaleRatio,
        props.layoutItems,
        props.scaleRatio,
        props.selectedTable.height,
        props.selectedTable.left,
        props.selectedTable.top,
        props.selectedTable.width,
        state.dividers
    ]);

    useEffect(() => {
        const assignedColumns: IAssignedColumn[] = getAssignedColumns(
            assignedLineItems,
            lineItemsAssignments,
            state.tableData
        );

        setState((prevState) => ({
            ...prevState,
            assignedColumns: assignedColumns,
            editMode: assignedColumns.length === 0 && !tableMenuState.isOpen
        }));
    }, [
        assignedLineItems,
        lineItemsAssignments,
        state.tableData,
        state.useAutoRecognizedHeader,
        tableMenuState.isOpen
    ]);

    useEffect(() => {
        updateTableData();
    }, [updateTableData, props.scaleRatio, state.dividers]);

    useEffect(() => {
        onTableMenuClosed();
    }, [onTableMenuClosed, props.containerScrollOffset]);

    useEffect(() => {
        if (!isSelectedTableChanged(state.table, props.selectedTable)) {
            return;
        }

        initTableData();
        setState((prevState) => ({
            ...prevState,
            table: props.selectedTable,
            layoutItems: props.layoutItems
        }));
    }, [initTableData, props.layoutItems, props.selectedTable, state.table]);

    const addDivider = useCallback(
        (orientation: DividerOrientation, position: IPoint): void => {
            const newDivider = createDivider(state.dividers, orientation, position, props.selectedTable);

            if (props.selectedTable && newDivider) {
                setState((prevState) => ({
                    ...prevState,
                    dividers: [...prevState.dividers, newDivider],
                    draftHorizontalDivider: { ...prevState.draftHorizontalDivider, visible: false },
                    draftVerticalDivider: { ...prevState.draftVerticalDivider, visible: false }
                }));
            }
        },
        [props.selectedTable, state.dividers]
    );

    const onVerticalControlAreaMouseDown = useCallback(
        onDividersControlAreaMouseDown(state.editMode, tablePositionService, addDivider, DividerOrientation.Vertical),
        [state.editMode, tablePositionService, addDivider]
    );

    const onHorizontalControlAreaMouseDown = useCallback(
        onDividersControlAreaMouseDown(state.editMode, tablePositionService, addDivider, DividerOrientation.Horizontal),
        [state.editMode, tablePositionService, addDivider]
    );

    const onHorizontalControlAreaMouseMove = useCallback(
        onDividersControlAreaMouseMove(
            state.draftVerticalDivider,
            state.draftHorizontalDivider,
            DividerOrientation.Horizontal,
            state.editMode,
            tablePositionService,
            setState
        ),
        [props.currentPageOffset, props.containerScrollOffset]
    );

    const onVerticalControlAreaMouseMove = useCallback(
        onDividersControlAreaMouseMove(
            state.draftVerticalDivider,
            state.draftHorizontalDivider,
            DividerOrientation.Vertical,
            state.editMode,
            tablePositionService,
            setState
        ),
        [props.currentPageOffset, props.containerScrollOffset]
    );

    const onHorizontalControlAreaMouseLeave = useCallback(
        onDividersControlAreaMouseLeave(setState, DividerOrientation.Horizontal, state.editMode),
        [setState, state.editMode]
    );

    const onVerticalControlAreaMouseLeave = useCallback(
        onDividersControlAreaMouseLeave(setState, DividerOrientation.Vertical, state.editMode),
        [setState, state.editMode]
    );

    const onDividerDeleted = useCallback(
        (id: number): void => {
            if (state.editMode) {
                setState((prevState) => ({
                    ...prevState,
                    dividers: prevState.dividers.filter((div) => div.id !== id)
                }));
            }
            onTableMenuClosed();
        },
        [onTableMenuClosed, state.editMode]
    );

    const onDividerPositionChanged = useCallback(
        (id: number, position: IPoint): void => {
            setState((prevState) => ({
                ...prevState,
                dividers: prevState.dividers.map((div) =>
                    div.id === id ? { ...div, top: position.y, left: position.x } : div
                )
            }));
            onTableMenuClosed();
        },
        [onTableMenuClosed]
    );

    const onDividerDragModeChanged = useCallback(
        (dragging: boolean): void => {
            setState((prevState) => ({
                ...prevState,
                dividerDragMode: dragging
            }));
            onTableMenuClosed();
        },
        [onTableMenuClosed]
    );

    const getTableStyles = useCallback(() => {
        if (state.dividerDragMode || !state.editMode) {
            return { ...tablePositionService.getTable(), border: "3px solid #cccccc" };
        } else {
            return tablePositionService.getTable();
        }
    }, [state.dividerDragMode, state.editMode, tablePositionService]);

    const onAssignColumn = useCallback(
        (fieldType: LineItemsFieldTypes) => () => {
            onTableMenuClosed();
            if (!props.selectedTable || !state.tableData) {
                return;
            }
            const columnsRows = state.useAutoRecognizedHeader
                ? state.tableData.columnsRows.map((column) => column.slice(1))
                : state.tableData?.columnsRows;
            if (tableMenuState.columnIndex || tableMenuState.columnIndex === 0) {
                props.onAssignColumnToLineItems(columnsRows, fieldType, tableMenuState.columnIndex, props.scaleRatio);
            }
        },
        [onTableMenuClosed, props, state.tableData, state.useAutoRecognizedHeader, tableMenuState.columnIndex]
    );

    const onHeaderAutoRecognitionCheckedChanged = useCallback(
        (checked: boolean) => {
            if (checked) {
                // clear annotations with line items from first row
                const annotations = getLineItemAnnotationsFromFirstRow(
                    assignedLineItems,
                    lineItemsAssignments,
                    state.tableData
                );
                annotations.map((annotation) => dispatch(deleteInvoiceItems(annotation)));
                dispatch(clearEmptyDataAnnotations());

                autoAssignTableColumns(state.tableData, true);
                setState((prevState) => ({
                    ...prevState,
                    useAutoRecognizedHeader: true
                }));
            } else if (state.useAutoRecognizedHeader) {
                dispatch(switchToEditMode());
                dispatch(removeNoAssignmentLineItems());
                setState((prevState) => ({
                    ...prevState,
                    useAutoRecognizedHeader: false
                }));
            }
        },
        [
            assignedLineItems,
            autoAssignTableColumns,
            dispatch,
            lineItemsAssignments,
            state.tableData,
            state.useAutoRecognizedHeader
        ]
    );

    const tableHasInvalidDividers = useCallback(() => {
        return state.tableData !== undefined && !state.tableData.dividers.some((divider) => !divider.isValid);
    }, [state.tableData]);

    const defaultSwitchComponentHeight = 32;
    const switchComponentHeight =
        switchToEditModeAreaComponentRef?.current?.clientHeight ?? defaultSwitchComponentHeight;

    const buttonsAreaStyles = useMemo(
        () => ({
            left: props.selectedTable.left * tablePositionService.getRelativeScaleRatio(),
            top:
                props.selectedTable.top * tablePositionService.getRelativeScaleRatio() -
                tablePositionService.getScaledControlAreaOffset() -
                switchComponentHeight,
            width: props.selectedTable.width * tablePositionService.getRelativeScaleRatio()
        }),
        [
            props.selectedTable.left,
            props.selectedTable.top,
            props.selectedTable.width,
            switchComponentHeight,
            tablePositionService
        ]
    );

    return (
        <div>
            {/* <ReactTooltip
                backgroundColor={"transparent"}
                id="plus-icon"
                offset={{ top: tooltipTopShift, left: tooltipLeftShift }}
                scrollHide={true}>
                <FontAwesomeIcon icon={faPlusCircle} className={`${COMPONENT_NAME}__plus-tooltip`} />
            </ReactTooltip> */}
            {!tableMenuState.isOpen && (
                <div
                    ref={switchToEditModeAreaComponentRef}
                    style={buttonsAreaStyles}
                    className={`${COMPONENT_NAME}__button-area`}>
                    <div className={`${COMPONENT_NAME}__button-area__container`}>
                        <SwitchToEditModeComponent
                            beforeSwitchToEditMode={onTableMenuClosed}
                            askConfirmation={true}
                            isMenuStyles={false}
                        />
                        <SwitchButtonComponent
                            isVisible={true}
                            checked={state.useAutoRecognizedHeader}
                            enabled={tableHasInvalidDividers()}
                            title={t("HEADER")}
                            onCheckedChanged={onHeaderAutoRecognitionCheckedChanged}
                        />
                    </div>
                </div>
            )}
            <div
                style={tablePositionService.getDividersControlArea()}
                className={`${COMPONENT_NAME}__dividers-control-area`}>
                {state.editMode && (
                    <>
                        <div
                            data-tip={true}
                            data-for="plus-icon"
                            className={`${COMPONENT_NAME}__side-control-area`}
                            style={tablePositionService.getTopControlArea()}
                            onMouseLeave={onVerticalControlAreaMouseLeave}
                            onMouseMove={onVerticalControlAreaMouseMove}
                            onMouseDown={onVerticalControlAreaMouseDown}
                        />
                        <div
                            data-tip={true}
                            data-for="plus-icon"
                            className={`${COMPONENT_NAME}__side-control-area`}
                            style={tablePositionService.getBottomControlArea()}
                            onMouseLeave={onVerticalControlAreaMouseLeave}
                            onMouseMove={onVerticalControlAreaMouseMove}
                            onMouseDown={onVerticalControlAreaMouseDown}
                        />
                        <div
                            data-tip={true}
                            data-for="plus-icon"
                            className={`${COMPONENT_NAME}__side-control-area`}
                            style={tablePositionService.getLeftControlArea()}
                            onMouseLeave={onHorizontalControlAreaMouseLeave}
                            onMouseMove={onHorizontalControlAreaMouseMove}
                            onMouseDown={onHorizontalControlAreaMouseDown}
                        />
                        <div
                            data-tip={true}
                            data-for="plus-icon"
                            className={`${COMPONENT_NAME}__side-control-area`}
                            style={tablePositionService.getRightControlArea()}
                            onMouseLeave={onHorizontalControlAreaMouseLeave}
                            onMouseMove={onHorizontalControlAreaMouseMove}
                            onMouseDown={onHorizontalControlAreaMouseDown}
                        />
                    </>
                )}
                <div className={classNames(tableStyles)} ref={tableAssignmentComponentRef} style={getTableStyles()} />
                <div
                    className={`${COMPONENT_NAME}__draft-divider--horizontal`}
                    style={{
                        top: state.draftHorizontalDivider.top,
                        visibility: state.draftHorizontalDivider.visible && state.editMode ? "visible" : "hidden"
                    }}
                />
                <div
                    className={`${COMPONENT_NAME}__draft-divider--vertical`}
                    style={{
                        left: state.draftVerticalDivider.left,
                        visibility: state.draftVerticalDivider.visible && state.editMode ? "visible" : "hidden"
                    }}
                />
                {state.tableData &&
                    state.tableData.dividers.length > 0 &&
                    state.tableData.dividers.map((div) => (
                        <TableDividerComponent
                            key={`divider_${div.id}`}
                            divider={div}
                            tablePositionService={tablePositionService}
                            table={tablePositionService.getDividersControlArea()}
                            onDividerDeleted={onDividerDeleted}
                            onDividerPositionChanged={onDividerPositionChanged}
                            onDividerDragModeChanged={onDividerDragModeChanged}
                            onDividerContextMenu={handleContextMenu}
                            onNotifyTooltipWasShown={props.onTooltipWasShown}
                            tooltipWasShown={props.tooltipWasShown}
                            dividerDragMode={state.dividerDragMode}
                            editMode={state.editMode}
                        />
                    ))}
                {state.assignedColumns.length > 0 &&
                    state.assignedColumns.map((col) => (
                        <ColumnHighlightComponent
                            key={`column-highlight-component-${col.fieldName}`}
                            topLeft={tablePositionService.getPointRelativeToPage(col.topLeft)}
                            bottomRight={tablePositionService.getPointRelativeToPage(col.bottomRight)}
                            isAssigned={true}
                            title={col.fieldName}
                        />
                    ))}
                {tableMenuState.isOpen && state.tableData && tableMenuState.columnIndex !== undefined && (
                    <ColumnHighlightComponent
                        topLeft={tablePositionService.getPointRelativeToPage(
                            getColumnRect(
                                state.useAutoRecognizedHeader
                                    ? state.tableData.columnsRows[tableMenuState.columnIndex].slice(1)
                                    : state.tableData.columnsRows[tableMenuState.columnIndex],
                                state.tableData
                            ).topLeft
                        )}
                        bottomRight={tablePositionService.getPointRelativeToPage(
                            getColumnRect(state.tableData.columnsRows[tableMenuState.columnIndex], state.tableData)
                                .bottomRight
                        )}
                        isAssigned={false}
                        title=""
                    />
                )}
                {state.useAutoRecognizedHeader &&
                    state.tableData !== undefined &&
                    state.tableData.columnsRows.length > 0 && (
                        <div
                            className={`${COMPONENT_NAME}__auto-recognized-header`}
                            style={{
                                position: "absolute",
                                top: tablePositionService.getPointRelativeToPage({
                                    x: state.tableData.columnsRows[0][0].topLeft.x,
                                    y: state.tableData.columnsRows[0][0].topLeft.y
                                }).y,
                                left: tablePositionService.getPointRelativeToPage({
                                    x: state.tableData.columnsRows[0][0].topLeft.x,
                                    y: state.tableData.columnsRows[0][0].topLeft.y
                                }).x,
                                width:
                                    state.tableData.columnsRows[state.tableData.columnsRows.length - 1][0].bottomRight
                                        .x - state.tableData.columnsRows[0][0].topLeft.x,
                                height:
                                    state.tableData.columnsRows[0][0].bottomRight.y -
                                    state.tableData.columnsRows[0][0].topLeft.y
                            }}
                        />
                    )}
            </div>
            <TableMenuComponent
                onTableMenuClosed={onTableMenuClosed}
                onAssignColumn={onAssignColumn}
                tableMenuState={tableMenuState}
            />
        </div>
    );
});

function createDivider(
    dividers: ITableDivider[],
    orientation: DividerOrientation,
    position: IPoint,
    selectedTable: IBox
): ITableDivider | undefined {
    const nextId =
        dividers.length > 0
            ? Math.max.apply(
                  Math,
                  dividers.map((div) => div.id)
              ) + 1
            : 1;

    return {
        id: nextId,
        orientation: orientation,
        left: position.x,
        top: position.y,
        width: orientation === DividerOrientation.Horizontal ? selectedTable.width : 3,
        height: orientation === DividerOrientation.Vertical ? selectedTable.height : 3,
        isValid: true
    };
}

function onDividersControlAreaMouseLeave(
    setState: React.Dispatch<React.SetStateAction<ITableAssignmentComponentState>>,
    orientation: DividerOrientation,
    editMode: boolean
): (event: React.MouseEvent<Element, MouseEvent>) => void {
    return (event: React.MouseEvent) => {
        if (event.buttons === 0 && editMode) {
            if (orientation === DividerOrientation.Vertical) {
                setState((prevState) => ({
                    ...prevState,
                    draftVerticalDivider: {
                        ...prevState.draftVerticalDivider,
                        visible: false
                    }
                }));
            } else {
                setState((prevState) => ({
                    ...prevState,
                    draftHorizontalDivider: {
                        ...prevState.draftHorizontalDivider,
                        visible: false
                    }
                }));
            }
        }
    };
}

function onDividersControlAreaMouseMove(
    draftVerticalDivider: DraftDivider,
    draftHorizontalDivider: DraftDivider,
    orientation: DividerOrientation,
    editMode: boolean,
    tablePositionService: TablePositionService,
    setState: React.Dispatch<React.SetStateAction<ITableAssignmentComponentState>>
): (event: React.MouseEvent<Element, MouseEvent>) => void {
    return (event: React.MouseEvent) => {
        if (event.buttons === 0 && editMode) {
            if (orientation === DividerOrientation.Vertical) {
                const newPositionLeft = tablePositionService.getDraftDividerPosition({
                    x: event.clientX,
                    y: event.clientY
                }).x;
                if (Math.trunc(draftVerticalDivider.left) !== newPositionLeft) {
                    setState((prevState) => ({
                        ...prevState,
                        draftVerticalDivider: {
                            ...prevState.draftVerticalDivider,
                            visible: true,
                            left: newPositionLeft
                        }
                    }));
                }
            } else {
                const newPositionTop = tablePositionService.getDraftDividerPosition({
                    x: event.clientX,
                    y: event.clientY
                }).y;
                if (Math.trunc(draftHorizontalDivider.top) !== newPositionTop) {
                    setState((prevState) => ({
                        ...prevState,
                        draftHorizontalDivider: {
                            ...prevState.draftHorizontalDivider,
                            visible: true,
                            top: newPositionTop
                        }
                    }));
                }
            }
        }
    };
}

function onDividersControlAreaMouseDown(
    editMode: boolean,
    tablePositionService: TablePositionService,
    addDivider: (orientation: DividerOrientation, position: IPoint) => void,
    orientation: DividerOrientation
): (event: React.MouseEvent<Element, MouseEvent>) => void {
    return (event: React.MouseEvent) => {
        if (event.button === 0 && editMode) {
            const { clientX, clientY } = event;
            if (orientation === DividerOrientation.Vertical) {
                addDivider(DividerOrientation.Vertical, {
                    x: tablePositionService.getNewDividerPosition({ x: clientX, y: clientY }).x,
                    y: tablePositionService.getNewDividerPosition({ x: clientX, y: clientY }).y
                });
            } else {
                addDivider(DividerOrientation.Horizontal, {
                    x: tablePositionService.getNewDividerPosition({ x: clientX, y: clientY }).x,
                    y: tablePositionService.getNewDividerPosition({ x: clientX, y: clientY }).y
                });
            }
        }
    };
}

function isSelectedTableChanged(stateTable: IBox | undefined, propsTable: IBox): boolean {
    return (
        stateTable === undefined ||
        propsTable.left !== stateTable.left ||
        propsTable.top !== stateTable.top ||
        propsTable.width !== stateTable.width ||
        propsTable.height !== stateTable.height
    );
}

function getLineItemAnnotationsFromFirstRow(
    assignedLineItems: ILineItem[],
    lineItemsAssignments: IInvoiceLineItemAnnotation[] | undefined,
    tableData: IInvoiceTable | undefined
): IInvoiceLineItemAnnotation[] {
    if (!lineItemsAssignments || !tableData) {
        return [];
    }

    const tableAnnotations: IInvoiceLineItemAnnotation[] = _.intersectionBy(
        lineItemsAssignments,
        assignedLineItems,
        "orderNumber"
    );

    const firstRowLayoutItemsIds = _.flatten(tableData.columnsRows.map((column) => column[0].layoutItems)).map(
        (item) => item.id
    );
    return tableAnnotations.filter((annotation) =>
        annotation.lineItemAnnotations.some((lineItemAnnotation) =>
            lineItemAnnotation.documentLayoutItemIds.some((id) =>
                firstRowLayoutItemsIds.some((tableItemId) => tableItemId === id)
            )
        )
    );
}

function getAssignedColumns(
    assignedLineItems: ILineItem[],
    lineItemsAssignments: IInvoiceLineItemAnnotation[] | undefined,
    tableData: IInvoiceTable | undefined
): IAssignedColumn[] {
    if (!lineItemsAssignments || !tableData) {
        return [];
    }

    const assignedColumns: IAssignedColumn[] = [];
    const tableAnnotations = _.intersectionBy(lineItemsAssignments, assignedLineItems, "orderNumber");
    const assignedTableLayoutItems = _.flatten(tableAnnotations.map((assignment) => assignment.lineItemAnnotations));
    // tslint:disable-next-line: prefer-for-of
    for (let columnIndex = 0; columnIndex < tableData.columnsRows.length; columnIndex++) {
        const currentColumnLayoutItems = _.flatten(tableData.columnsRows[columnIndex].map((row) => row.layoutItems));
        const assignedColumnLayoutItems = assignedTableLayoutItems.filter((layoutItem) => {
            const res = layoutItem.documentLayoutItemIds.some((assignedLayoutItemId) =>
                currentColumnLayoutItems.some((columnItem) => columnItem.id === assignedLayoutItemId)
            );
            return res;
        });

        if (assignedColumnLayoutItems.length > 0) {
            assignedColumns.push({
                columnIndex: columnIndex,
                fieldName: assignedColumnLayoutItems[0].fieldType,
                tableCells: tableData.columnsRows[columnIndex],
                topLeft: { x: 0, y: 0 },
                bottomRight: { x: 0, y: 0 }
            });
        }
    }

    for (const assignedColumn of assignedColumns) {
        const { topLeft, bottomRight } = getColumnRect(assignedColumn.tableCells, tableData);
        assignedColumn.topLeft = topLeft;
        assignedColumn.bottomRight = bottomRight;
    }
    return assignedColumns;
}

// tslint:disable-next-line: typedef
function getColumnRect(tableCells: ITableCell[] | undefined, tableData: IInvoiceTable | undefined) {
    if (!tableCells || !tableData) {
        return { topLeft: { x: 0, y: 0 }, bottomRight: { x: 0, y: 0 } };
    }

    const lastColumnMaxX = Math.max.apply(
        Math,
        _.flatten(tableData.columnsRows.map((col) => col.map((cell) => cell.bottomRight.x)))
    );

    const minX = Math.min.apply(
        Math,
        tableCells.map((cell) => cell.topLeft.x)
    );
    const minY = Math.min.apply(
        Math,
        tableCells.map((cell) => cell.topLeft.y)
    );
    const maxX = Math.max.apply(
        Math,
        tableCells.map((cell) => cell.bottomRight.x)
    );
    const maxY = Math.max.apply(
        Math,
        tableCells.map((cell) => cell.bottomRight.y)
    );

    return {
        topLeft: { x: minX, y: minY },
        bottomRight: { x: lastColumnMaxX === maxX ? maxX : maxX + 3, y: maxY }
    };
}
