import { Dictionary, IDictionary } from "../models/Dictionary";
import { getPlainTextFromSelectedLayoutItems } from "./../components/InvoiceDataCaptureToolkitComponent/store/reducers/helpers/getTextOrAnnotationValue";
import {
    IInvoiceTable,
    ILayoutItem,
    ITableCell,
    LineItemsFieldTypes
} from "./../components/InvoiceDataCaptureToolkitComponent/store/state";

interface IHeaderType {
    type: LineItemsFieldTypes;
    probabilityOrder?: number;
}

export class TableHeaderService {
    private readonly partialMatchWords: IDictionary<IHeaderType> = this.createPartialMatchWordsDictionary();
    private readonly partialMatchPhrases: IDictionary<IHeaderType> = this.createPartialMatchPhrasesDictionary();
    private readonly completeMatchPhrases: IDictionary<IHeaderType> = this.createCompleteMatchPhrasesDictionary();
    private replaceSymbols: string[] = ["€", "£", "$", "¥", ",", ".", "%", ":"];

    public recognizeTableHeader(table: IInvoiceTable): IInvoiceTable {
        if (!this.tableHeaderIsValid(table)) {
            table.columnsRows.forEach((tableColumn) => {
                tableColumn.forEach((row) => {
                    row.isHeader = false;
                    row.autoRecognizedColumnType = undefined;
                });
            });
            return table;
        }

        table.columnsRows.forEach((tableColumn) => {
            const firstRow = tableColumn[0];
            if (firstRow) {
                firstRow.isHeader = true;

                const columnType = this.recognizeCellHeaderColumnType(firstRow.layoutItems);
                if (columnType) {
                    if (columnType.probabilityOrder) {
                        this.assignCellTypeWithProbability(table, tableColumn, columnType);
                    } else {
                        this.assignCellType(table, tableColumn, columnType);
                    }
                }
            }
        });

        return table;
    }

    private assignCellType(table: IInvoiceTable, tableColumn: ITableCell[], columnType: IHeaderType): void {
        const columnTypeAlreadyAssigned =
            columnType &&
            table.columnsRows.some((column) =>
                column.some((cell) => cell.autoRecognizedColumnType === columnType.type)
            );

        if (!columnTypeAlreadyAssigned) {
            tableColumn.forEach((row) => {
                row.autoRecognizedColumnType = columnType?.type;
                row.autoRecognizedColumnProbabilityOrder = undefined;
            });
        }
    }

    private assignCellTypeWithProbability(
        table: IInvoiceTable,
        tableColumn: ITableCell[],
        columnType: IHeaderType
    ): void {
        if (!columnType || !columnType.probabilityOrder) {
            return;
        }

        table.columnsRows.forEach((column) => {
            column.forEach((cell) => {
                if (this.columnTypeAlreadyAssignedWithLowerProbability(cell, columnType!)) {
                    cell.autoRecognizedColumnProbabilityOrder = undefined;
                    cell.autoRecognizedColumnType = undefined;
                }
            });
        });

        if (!this.columnTypeAlreadyAssignedWithHigerProbability(table, columnType!)) {
            tableColumn.forEach((cell) => {
                cell.autoRecognizedColumnType = columnType?.type;
                cell.autoRecognizedColumnProbabilityOrder = columnType?.probabilityOrder;
            });
        }
    }

    private columnTypeAlreadyAssignedWithLowerProbability(cell: ITableCell, columnType: IHeaderType): boolean {
        return (
            !!columnType.probabilityOrder &&
            !!cell.autoRecognizedColumnProbabilityOrder &&
            cell.autoRecognizedColumnType === columnType.type &&
            columnType.probabilityOrder! < cell.autoRecognizedColumnProbabilityOrder!
        );
    }

    private columnTypeAlreadyAssignedWithHigerProbability(table: IInvoiceTable, columnType: IHeaderType): boolean {
        return (
            !!columnType.probabilityOrder &&
            table.columnsRows.some((column) =>
                column.some(
                    (cell) =>
                        !!cell.autoRecognizedColumnType &&
                        cell.autoRecognizedColumnType === columnType.type &&
                        cell.autoRecognizedColumnProbabilityOrder! < columnType.probabilityOrder!
                )
            )
        );
    }

    private recognizeCellHeaderColumnType(layoutItems: ILayoutItem[]): IHeaderType | undefined {
        const cellText: string = this.removeWrongSymbols(
            getPlainTextFromSelectedLayoutItems(layoutItems).groupValue.toLowerCase()
        );

        const headerCompleteMatchPhraseKey = this.completeMatchPhrases.keys().find((key) => cellText === key);
        if (headerCompleteMatchPhraseKey) {
            return this.completeMatchPhrases.get(headerCompleteMatchPhraseKey!);
        }

        const partialMatchPhraseKey = this.partialMatchPhrases.keys().find((key) => cellText.indexOf(key) > -1);
        if (partialMatchPhraseKey) {
            return this.partialMatchPhrases.get(partialMatchPhraseKey!);
        }

        const wordKey = this.partialMatchWords
            .keys()
            .find((key) => layoutItems.some((item) => this.removeWrongSymbols(item.value.toLowerCase()) === key));

        return wordKey ? this.partialMatchWords.get(wordKey!) : undefined;
    }

    private tableHeaderIsValid(table: IInvoiceTable): boolean {
        if (table.columnsRows.length === 0) {
            return false;
        }

        let hasAtLeastOneRecognition = false;

        table.columnsRows.forEach((tableColumn) => {
            const firstRow = tableColumn[0];
            if (firstRow) {
                const columnType = this.recognizeCellHeaderColumnType(firstRow.layoutItems);
                hasAtLeastOneRecognition = columnType !== undefined || hasAtLeastOneRecognition;
            }
        });

        return hasAtLeastOneRecognition;
    }

    private removeWrongSymbols(text: string): string {
        this.replaceSymbols.forEach((symbol) => {
            text = text.replace(symbol, "");
        });
        return text;
    }

    private createCompleteMatchPhrasesDictionary(): IDictionary<IHeaderType> {
        return new Dictionary([
            { key: "item no", value: { type: LineItemsFieldTypes.number, probabilityOrder: 1 } },
            { key: "item#", value: { type: LineItemsFieldTypes.number, probabilityOrder: 2 } },
            { key: "item№", value: { type: LineItemsFieldTypes.number, probabilityOrder: 3 } },
            { key: "id", value: { type: LineItemsFieldTypes.number, probabilityOrder: 4 } },
            { key: "№", value: { type: LineItemsFieldTypes.number, probabilityOrder: 5 } },
            { key: "#", value: { type: LineItemsFieldTypes.number, probabilityOrder: 6 } },
            { key: "line", value: { type: LineItemsFieldTypes.number, probabilityOrder: 7 } },
            { key: "item", value: { type: LineItemsFieldTypes.number, probabilityOrder: 8 } },
            { key: "itm", value: { type: LineItemsFieldTypes.number, probabilityOrder: 9 } },
            { key: "no", value: { type: LineItemsFieldTypes.number, probabilityOrder: 10 } },
            { key: "n", value: { type: LineItemsFieldTypes.number, probabilityOrder: 11 } },
            { key: "num", value: { type: LineItemsFieldTypes.number, probabilityOrder: 12 } },

            { key: "unit price", value: { type: LineItemsFieldTypes.price, probabilityOrder: 1 } },
            { key: "price/rate", value: { type: LineItemsFieldTypes.price, probabilityOrder: 2 } },

            { key: "quantity", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 1 } },
            { key: "qty", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 2 } },
            { key: "hours", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 3 } },
            { key: "qty/hours", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 4 } },

            { key: "total", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 1 } },
            { key: "total(ex-gst)", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 2 } },
            { key: "value", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 3 } },
            { key: "extension", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 4 } },
            { key: "extended", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 5 } },

            { key: "description", value: { type: LineItemsFieldTypes.description, probabilityOrder: 1 } },
            { key: "details", value: { type: LineItemsFieldTypes.description, probabilityOrder: 2 } },
            { key: "product", value: { type: LineItemsFieldTypes.description, probabilityOrder: 3 } },
            { key: "notes", value: { type: LineItemsFieldTypes.description, probabilityOrder: 4 } }
        ]).toLookup();
    }

    private createPartialMatchPhrasesDictionary(): IDictionary<IHeaderType> {
        return new Dictionary([
            { key: "contract number", value: { type: LineItemsFieldTypes.number, probabilityOrder: 100 } },
            { key: "serial number", value: { type: LineItemsFieldTypes.number, probabilityOrder: 101 } },
            { key: "item id", value: { type: LineItemsFieldTypes.number, probabilityOrder: 102 } },
            { key: "catalog #", value: { type: LineItemsFieldTypes.number, probabilityOrder: 103 } },
            { key: "catalog №", value: { type: LineItemsFieldTypes.number, probabilityOrder: 104 } },
            { key: "catalog no", value: { type: LineItemsFieldTypes.number, probabilityOrder: 105 } },
            { key: "lot #", value: { type: LineItemsFieldTypes.number, probabilityOrder: 106 } },
            { key: "lot №", value: { type: LineItemsFieldTypes.number, probabilityOrder: 107 } },
            { key: "lot no", value: { type: LineItemsFieldTypes.number, probabilityOrder: 108 } },
            { key: "line no", value: { type: LineItemsFieldTypes.number, probabilityOrder: 109 } },
            { key: "line #", value: { type: LineItemsFieldTypes.number, probabilityOrder: 110 } },
            { key: "line №", value: { type: LineItemsFieldTypes.number, probabilityOrder: 111 } },

            { key: "unit price", value: { type: LineItemsFieldTypes.price, probabilityOrder: 100 } },
            { key: "item price", value: { type: LineItemsFieldTypes.price, probabilityOrder: 101 } },
            { key: "price each", value: { type: LineItemsFieldTypes.price, probabilityOrder: 102 } },

            { key: "item description", value: { type: LineItemsFieldTypes.description, probabilityOrder: 100 } },

            { key: "item quantity", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 100 } },
            { key: "qty shipped", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 101 } },
            { key: "quantity shipped", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 102 } },

            { key: "total amount", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 100 } },
            { key: "total price", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 101 } },
            { key: "total charge", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 102 } },
            { key: "qty price", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 103 } },
            { key: "quantity price", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 104 } },
            { key: "ext price", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 105 } },
            { key: "extended amount", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 106 } },
            { key: "extended price", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 107 } }
        ]).toLookup();
    }

    private createPartialMatchWordsDictionary(): IDictionary<IHeaderType> {
        return new Dictionary([
            { key: "description", value: { type: LineItemsFieldTypes.description, probabilityOrder: 200 } },
            { key: "details", value: { type: LineItemsFieldTypes.description, probabilityOrder: 201 } },
            { key: "product", value: { type: LineItemsFieldTypes.description, probabilityOrder: 202 } },
            { key: "notes", value: { type: LineItemsFieldTypes.description, probabilityOrder: 203 } },

            { key: "№", value: { type: LineItemsFieldTypes.number, probabilityOrder: 200 } },
            { key: "#", value: { type: LineItemsFieldTypes.number, probabilityOrder: 201 } },
            { key: "item", value: { type: LineItemsFieldTypes.number, probabilityOrder: 202 } },
            { key: "number", value: { type: LineItemsFieldTypes.number, probabilityOrder: 203 } },

            { key: "quantity", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 200 } },
            { key: "qty", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 201 } },
            { key: "shipped", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 202 } },
            { key: "count", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 203 } },
            { key: "hours", value: { type: LineItemsFieldTypes.quantity, probabilityOrder: 204 } },

            { key: "price", value: { type: LineItemsFieldTypes.price, probabilityOrder: 200 } },
            { key: "rate", value: { type: LineItemsFieldTypes.price, probabilityOrder: 201 } },

            { key: "total", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 200 } },
            { key: "amount", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 201 } },
            { key: "charge", value: { type: LineItemsFieldTypes.lineTotal, probabilityOrder: 202 } }
        ]).toLookup();
    }
}
