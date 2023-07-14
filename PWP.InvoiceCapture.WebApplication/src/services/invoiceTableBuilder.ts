import { Point } from "../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { InvoiceTable } from "../models/InvoiceTable";
import {
    IInvoiceTable,
    ILayoutItem,
    ITableCell,
    ITableDivider
} from "./../components/InvoiceDataCaptureToolkitComponent/store/state";
import { DividersService } from "./dividersService";
import { LayoutItemsService } from "./layoutItemsService";
import { TableHeaderService } from "./tableHeaderService";

export class InvoiceTableBuilder {
    private readonly invoiceTable: IInvoiceTable;
    private readonly layoutItemsService: LayoutItemsService;
    private readonly dividersService: DividersService;
    private readonly tableHeaderService: TableHeaderService;

    private invoiceLayoutItems: ILayoutItem[];
    private tableDividers: ITableDivider[];
    private topLeft: Point;
    private bottomRight: Point;

    constructor() {
        this.layoutItemsService = new LayoutItemsService(); // TODO: use DI
        this.dividersService = new DividersService(); // TODO: use DI
        this.tableHeaderService = new TableHeaderService(); // TODO: use DI
        this.invoiceTable = new InvoiceTable();
        this.invoiceLayoutItems = [];
        this.tableDividers = [];
        this.topLeft = { x: 0, y: 0 };
        this.bottomRight = { x: 0, y: 0 };
    }

    public dividers(dividers: ITableDivider[]): InvoiceTableBuilder {
        this.tableDividers = dividers ?? [];
        return this;
    }

    public borders(topLeft: Point, bottomRight: Point): InvoiceTableBuilder {
        this.topLeft = topLeft ?? { x: 0, y: 0 };
        this.bottomRight = bottomRight ?? { x: 0, y: 0 };
        return this;
    }

    public layoutItems(layoutItems: ILayoutItem[]): InvoiceTableBuilder {
        this.invoiceLayoutItems = this.layoutItemsService.getValidLayoutItems(layoutItems);
        return this;
    }

    public build(): IInvoiceTable {
        const verticalDividers = this.dividersService.getVerticalDividers(this.tableDividers);
        const horizontalDividers = this.dividersService.getHorizontalDividers(this.tableDividers);

        for (let vertDivider = 0; vertDivider < verticalDividers.length + 1; vertDivider++) {
            const currentVertDivider = verticalDividers[vertDivider];
            const previousVertDivider = verticalDividers[vertDivider - 1];

            const cellTopLeftX = previousVertDivider ? previousVertDivider.left : this.topLeft.x;
            const cellBottomRightX = currentVertDivider ? currentVertDivider.left : this.bottomRight.x;

            const columnRows: ITableCell[] = [];

            for (let horDivider = 0; horDivider < horizontalDividers.length + 1; horDivider++) {
                const currentHorDivider = horizontalDividers[horDivider];
                const previousHorDivider = horizontalDividers[horDivider - 1];

                const cellTopLeftY = previousHorDivider ? previousHorDivider.top : this.topLeft.y;
                const cellBottomRightY = currentHorDivider ? currentHorDivider.top : this.bottomRight.y;
                const topLeft: Point = { x: cellTopLeftX, y: cellTopLeftY };
                const bottomRight: Point = { x: cellBottomRightX, y: cellBottomRightY };

                columnRows.push({
                    topLeft: topLeft,
                    bottomRight: bottomRight,
                    layoutItems: this.layoutItemsService.getSelectedLayoutItems(
                        topLeft,
                        bottomRight,
                        this.invoiceLayoutItems
                    ),
                    isHeader: false,
                    autoRecognizedColumnType: undefined,
                    autoRecognizedColumnProbabilityOrder: undefined
                });
            }

            this.invoiceTable.columnsRows.push(columnRows);
        }

        this.invoiceTable.dividers = [...verticalDividers, ...horizontalDividers];

        return this.tableHeaderService.recognizeTableHeader(this.invoiceTable);
    }
}
