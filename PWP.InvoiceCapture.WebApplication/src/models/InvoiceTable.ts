import { IInvoiceTable, ITableCell, ITableDivider } from "../components/InvoiceDataCaptureToolkitComponent/store/state";

export class InvoiceTable implements IInvoiceTable {
    public constructor() {
        this.columnsRows = [];
        this.dividers = [];
    }

    public hasHeader(): boolean {
        return this.columnsRows.some((row) => row.some((cell) => cell.isHeader));
    }

    public columnsRows: ITableCell[][];
    public dividers: ITableDivider[];
}
