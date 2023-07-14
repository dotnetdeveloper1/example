import _ from "lodash";
import { Column, Point, Row } from "../api/models/InvoiceManagement/InvoiceDataAnnotation";
import {
    DividerOrientation,
    IInvoiceTable,
    ILayoutItem,
    ITableCell,
    ITableDivider
} from "./../components/InvoiceDataCaptureToolkitComponent/store/state";
import { DividersService } from "./dividersService";
import { InvoiceTableBuilder } from "./invoiceTableBuilder";
import { LayoutItemsService } from "./layoutItemsService";

export class LayoutItemsTableService {
    private readonly maxIterationsCount = 10000;
    private readonly layoutItemsService: LayoutItemsService;
    private readonly dividersService: DividersService;

    constructor() {
        this.layoutItemsService = new LayoutItemsService(); // TODO: use DI
        this.dividersService = new DividersService(); // TODO: use DI
    }

    public getSelectedLayoutItems(topLeft: Point, bottomRight: Point, layoutItems: ILayoutItem[]): ILayoutItem[] {
        const validLayoutItems = this.layoutItemsService.getValidLayoutItems(layoutItems);

        return this.layoutItemsService.getSelectedLayoutItems(topLeft, bottomRight, validLayoutItems);
    }

    public getTable(
        dividers: ITableDivider[],
        items: ILayoutItem[],
        topLeft: Point,
        bottomRight: Point
    ): IInvoiceTable {
        const validatedDividers = this.dividersService.validateDividers(items, dividers);

        return new InvoiceTableBuilder()
            .dividers(validatedDividers)
            .borders(topLeft, bottomRight)
            .layoutItems(items)
            .build();
    }

    public moveDividers(dividers: ITableDivider[], items: ILayoutItem[]): ITableDivider[] {
        return this.dividersService.moveDividers(items, dividers);
    }

    public getDividers(layoutItems: ILayoutItem[]): ITableDivider[] {
        const validLayoutItems = this.layoutItemsService.getValidLayoutItems(layoutItems);

        const columns = this.getColumns(validLayoutItems);

        this.mergeColumns(validLayoutItems, columns);

        const columnsRows = this.getColumnRows(validLayoutItems, columns);

        const rows = this.getRows(validLayoutItems, columnsRows);

        const columnDividers = this.dividersService.calculateColumnDividers(columns);
        const validColumnDividers = columnDividers.filter((divider) =>
            this.dividerPositionIsValid(validLayoutItems, divider)
        );

        const rowDividers = this.dividersService.calculateRowDividers(rows, columnDividers.length, validLayoutItems);
        const validRowDividers = rowDividers.filter((divider) =>
            this.dividerPositionIsValid(validLayoutItems, divider)
        );

        return [...validColumnDividers, ...validRowDividers];
    }

    public getColumnNumber(coordinate: Point, columnsRows: ITableCell[][]): number | undefined {
        if (!coordinate || !columnsRows) {
            return undefined;
        }

        const columnIndex = columnsRows.findIndex((column) =>
            column.find((row) => row.topLeft.x < coordinate.x && coordinate.x < row.bottomRight.x)
        );

        return columnIndex >= 0 ? columnIndex : undefined;
    }

    public getColumnLayoutItems(coordinate: Point, columnsRows: ITableCell[][]): ILayoutItem[] {
        if (!coordinate || !columnsRows) {
            return [];
        }

        const columnIndex = columnsRows.findIndex((column) =>
            column.find((row) => row.topLeft.x < coordinate.x && coordinate.x < row.bottomRight.x)
        );

        if (columnIndex < 0) {
            return [];
        }

        const items = _(columnsRows[columnIndex])
            .flatMap((column) => column.layoutItems)
            .value();

        return items;
    }

    private getColumns(layoutItems: ILayoutItem[]): Column[] {
        if (!layoutItems || layoutItems.length === 0) {
            return [];
        }

        const columnItemsIntersections: ILayoutItem[][] = [];

        let sortedLayoutItems = _(layoutItems)
            .sortBy((item) => item.topLeft.x)
            .value();

        let iteration = 0;
        do {
            iteration++;
            if (iteration > this.maxIterationsCount || sortedLayoutItems.length === 0) {
                break;
            }

            const intersections = this.layoutItemsService.findIntersections([sortedLayoutItems[0]], sortedLayoutItems);

            columnItemsIntersections.push(intersections);

            sortedLayoutItems = _(sortedLayoutItems)
                .filter((item) => !intersections.some((intersection) => intersection.id === item.id))
                .sortBy((item) => item.topLeft.x)
                .value();
        } while (sortedLayoutItems.length > 0);

        return columnItemsIntersections
            .filter((columnIntersection) => columnIntersection.length > 0)
            .map((intersection) => this.mapDocumentLayoutItems(intersection));
    }

    private dividerPositionIsValid(items: ILayoutItem[], divider: ITableDivider): boolean {
        return divider.orientation === DividerOrientation.Horizontal
            ? !this.dividersService.hasHorizontalIntersection(items, divider)
            : !this.dividersService.hasVerticalIntersection(items, divider);
    }

    private getRows(layoutItems: ILayoutItem[], columnsRows: Row[][]): Row[] {
        if (!layoutItems || layoutItems.length === 0) {
            return [];
        }

        const maxRowsCountInColumn = this.getMaxRowsCountArrayLength(columnsRows);

        // Index is count of rows, value is count of columns with this (index) count of rows
        const rowsCountInColumn = new Array<number>(maxRowsCountInColumn + 1).fill(0);
        columnsRows.forEach((columnRows) => {
            rowsCountInColumn[columnRows.length]++;
        });

        const maxCountOfRowsCount = _(rowsCountInColumn).max();
        let calculatedRowsCount = rowsCountInColumn.findIndex((count) => count === maxCountOfRowsCount);

        if (calculatedRowsCount === 1) {
            // there is only one row in table
            // set columns count with one row to zero
            rowsCountInColumn[calculatedRowsCount] = 0;
            const nextMaxCountOfRowsCount = _(rowsCountInColumn).max();
            const nextCalculatedRowsCount = rowsCountInColumn.findIndex((count) => count === nextMaxCountOfRowsCount);

            if (nextCalculatedRowsCount > 0) {
                calculatedRowsCount = nextCalculatedRowsCount;
            }
        }

        const rows = columnsRows.find((columnRow) => columnRow.length === calculatedRowsCount);

        return rows ?? [];
    }

    private getColumnRows(validLayoutItems: ILayoutItem[], columns: Column[]): Row[][] {
        const columnsRows: Row[][] = [];

        if (columns.length > 0) {
            columns.forEach((column) => {
                columnsRows.push(this.getRowsInColumn(column, validLayoutItems));
            });
        } else {
            columnsRows.push(
                this.getRowsInColumn(
                    {
                        topLeft: this.layoutItemsService.getTopLeftMin(validLayoutItems),
                        bottomRight: this.layoutItemsService.getBottomRightMax(validLayoutItems)
                    },
                    validLayoutItems
                )
            );
        }
        return columnsRows;
    }

    private getRowsInColumn(column: Column, layoutItems: ILayoutItem[]): Row[] {
        if (!layoutItems || layoutItems.length === 0) {
            return [];
        }

        const columnLayoutItems = this.getSelectedLayoutItems(column.topLeft, column.bottomRight, layoutItems);
        if (columnLayoutItems.length === 1) {
            return [this.createRow(columnLayoutItems[0].topLeft, columnLayoutItems[0].bottomRight)];
        }

        let sortedLayoutItems = _(columnLayoutItems)
            .sortBy((layoutItem) => layoutItem.topLeft.y)
            .value();

        const rows: Row[] = [];

        let iteration = 1;
        do {
            iteration++;
            if (iteration > this.maxIterationsCount || !sortedLayoutItems[0]) {
                break;
            }

            const closestLayoutItems = sortedLayoutItems.filter((item) =>
                this.layoutItemsService.areOnOneLine(
                    sortedLayoutItems[0].topLeft,
                    sortedLayoutItems[0].bottomRight,
                    item
                )
            );

            const topLeftMin = this.layoutItemsService.getTopLeftMin(closestLayoutItems);
            const bottomRightMax = this.layoutItemsService.getBottomRightMax(closestLayoutItems);

            rows.push(this.createRow(topLeftMin, bottomRightMax));

            sortedLayoutItems = _(sortedLayoutItems)
                .filter((item) => !closestLayoutItems.some((closestItem) => closestItem.id === item.id))
                .sortBy((item) => item.topLeft.y)
                .value();
        } while (sortedLayoutItems.length > 0);

        return rows;
    }

    private getMaxRowsCountArrayLength(columnRows: Row[][]): number {
        const rows = _(columnRows).maxBy((row) => row.length);
        if (rows) {
            return rows.length;
        }

        return 0;
    }

    private createRow(topLeft: Point, bottomRight: Point): Row {
        return { topLeft: topLeft, bottomRight: bottomRight };
    }

    private mapDocumentLayoutItems(intersections: ILayoutItem[]): Column {
        return {
            topLeft: this.layoutItemsService.getTopLeftMin(intersections),
            bottomRight: this.layoutItemsService.getBottomRightMax(intersections)
        };
    }

    private mergeColumns(layoutItems: ILayoutItem[], columns: Column[]): void {
        if (!columns || !layoutItems) {
            return;
        }

        const leftColumnToMerge = columns.find((column) => {
            const leftColumnIndex = columns.indexOf(column);
            return (
                this.rightColumnLostHeader(layoutItems, columns, leftColumnIndex) ||
                this.rightColumnLostItemsBelowHeader(layoutItems, columns, leftColumnIndex)
            );
        });

        if (leftColumnToMerge) {
            const leftColumnIndex: number = columns.indexOf(leftColumnToMerge);
            const rightColumnIndex: number = leftColumnIndex + 1;
            const rightColumnToMerge = columns[rightColumnIndex];
            if (rightColumnToMerge) {
                const mergedColumn: Column = {
                    topLeft: {
                        x: leftColumnToMerge.topLeft.x,
                        y: Math.min(leftColumnToMerge.topLeft.y, rightColumnToMerge.topLeft.y)
                    },
                    bottomRight: {
                        x: rightColumnToMerge.bottomRight.x,
                        y: Math.max(leftColumnToMerge.bottomRight.y, rightColumnToMerge.bottomRight.y)
                    }
                };
                columns[leftColumnIndex] = mergedColumn;
                columns.splice(rightColumnIndex, 1);

                return this.mergeColumns(layoutItems, columns);
            }
        }
    }

    private rightColumnLostHeader(layoutItems: ILayoutItem[], columns: Column[], columnIndex: number): boolean {
        const currentColumn = columns[columnIndex];
        const nextColumn = columns[columnIndex + 1];

        if (!currentColumn || !nextColumn) {
            return false;
        }

        const currentColumnLayoutItems = this.layoutItemsService.getSelectedLayoutItems(
            currentColumn.topLeft,
            currentColumn.bottomRight,
            layoutItems
        );

        const nextColumnLayoutItems = this.layoutItemsService.getSelectedLayoutItems(
            nextColumn.topLeft,
            nextColumn.bottomRight,
            layoutItems
        );

        const currentBottomRight = this.layoutItemsService.getBottomRightMax(currentColumnLayoutItems);
        const currentTopLeft = this.layoutItemsService.getTopLeftMin(currentColumnLayoutItems);
        const nextTopLeft = this.layoutItemsService.getTopLeftMin(nextColumnLayoutItems);

        return (
            nextColumnLayoutItems.every(
                (item) => !this.layoutItemsService.areOnOneLine(currentTopLeft, currentBottomRight, item)
            ) && currentBottomRight.y < nextTopLeft.y
        );
    }

    private rightColumnLostItemsBelowHeader(
        layoutItems: ILayoutItem[],
        columns: Column[],
        columnIndex: number
    ): boolean {
        const currentColumn = columns[columnIndex];
        const nextColumn = columns[columnIndex + 1];

        if (!currentColumn || !nextColumn) {
            return false;
        }

        const currentColumnLayoutItems = this.layoutItemsService.getSelectedLayoutItems(
            currentColumn.topLeft,
            currentColumn.bottomRight,
            layoutItems
        );

        const nextColumnLayoutItems = this.layoutItemsService.getSelectedLayoutItems(
            nextColumn.topLeft,
            nextColumn.bottomRight,
            layoutItems
        );

        const currentBottomRight = this.layoutItemsService.getBottomRightMax(currentColumnLayoutItems);
        const currentTopLeft = this.layoutItemsService.getTopLeftMin(currentColumnLayoutItems);
        const nextBottomRight = this.layoutItemsService.getBottomRightMax(nextColumnLayoutItems);

        return (
            nextColumnLayoutItems.every(
                (item) => !this.layoutItemsService.areOnOneLine(currentTopLeft, currentBottomRight, item)
            ) && nextBottomRight.y < currentTopLeft.y
        );
    }
}
