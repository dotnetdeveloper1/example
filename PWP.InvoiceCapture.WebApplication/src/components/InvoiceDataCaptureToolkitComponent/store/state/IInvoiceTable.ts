import { ITableCell, ITableDivider } from "./index";
export interface IInvoiceTable {
    // First index is column, second index is row
    columnsRows: ITableCell[][];
    dividers: ITableDivider[];
    hasHeader(): boolean;
}
