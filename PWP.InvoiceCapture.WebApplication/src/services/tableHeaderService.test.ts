import _ from "lodash";
import { ILayoutItem, LineItemsFieldTypes } from "../components/InvoiceDataCaptureToolkitComponent/store/state";
import json4imprint from "./jsonTestData/4imprint.json";
import jsonAlloSource from "./jsonTestData/alloSourceWithHeader.json";
import jsonFire from "./jsonTestData/fire.json";
import jsonSiemens from "./jsonTestData/SiemensPage1WithHeader.json";
import { LayoutItemsTableService } from "./layoutItemsTableService";
import { TableHeaderService } from "./tableHeaderService";

describe("TableHeaderService", () => {
    test("recognizeTableHeader - return no headers", async () => {
        const jsonText = JSON.stringify(jsonFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];

        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = layoutItemsTableService.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = layoutItemsTableService.getTable(
            dividers,
            selectedLayoutItems,
            topLeftSelection,
            bottomRightSelection
        );

        const tableWithHeaders = target.recognizeTableHeader(table);

        const hasHeader = tableWithHeaders.columnsRows.some((row) => row.some((cell) => cell.isHeader));

        expect(hasHeader === false).toBeTruthy();
        expect(tableWithHeaders.hasHeader() === false).toBeTruthy();
    });

    test("recognizeTableHeader - return headers from allo source", async () => {
        const jsonText = JSON.stringify(jsonAlloSource);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];

        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = layoutItemsTableService.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = layoutItemsTableService.getTable(
            dividers,
            selectedLayoutItems,
            topLeftSelection,
            bottomRightSelection
        );

        const tableWithHeaders = target.recognizeTableHeader(table);

        const hasHeader = tableWithHeaders.columnsRows.some(
            (row, index) => index === 0 && row.some((cell) => cell.isHeader)
        );

        const belowCellsHaveHeader = tableWithHeaders.columnsRows.some((row) =>
            row.some((cell, index) => index > 0 && cell.isHeader)
        );

        expect(hasHeader === true).toBeTruthy();
        expect(tableWithHeaders.hasHeader() === true).toBeTruthy();
        expect(belowCellsHaveHeader === false).toBeTruthy();

        tableWithHeaders.columnsRows[0].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === undefined).toBeTruthy()
        );
        tableWithHeaders.columnsRows[1].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.number).toBeTruthy()
        );
        tableWithHeaders.columnsRows[2].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.description).toBeTruthy()
        );
        tableWithHeaders.columnsRows[3].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.quantity).toBeTruthy()
        );
        tableWithHeaders.columnsRows[4].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.price).toBeTruthy()
        );
        tableWithHeaders.columnsRows[5].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.lineTotal).toBeTruthy()
        );
    });

    test("recognizeTableHeader - return headers without duplicates from siemens", async () => {
        const jsonText = JSON.stringify(jsonSiemens);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];

        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = layoutItemsTableService.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = layoutItemsTableService.getTable(
            dividers,
            selectedLayoutItems,
            topLeftSelection,
            bottomRightSelection
        );

        const tableWithHeaders = target.recognizeTableHeader(table);

        const hasHeader = tableWithHeaders.columnsRows.some(
            (row, index) => index === 0 && row.some((cell) => cell.isHeader)
        );

        const belowCellsHaveHeader = tableWithHeaders.columnsRows.some((row) =>
            row.some((cell, index) => index > 0 && cell.isHeader)
        );

        expect(hasHeader === true).toBeTruthy();
        expect(tableWithHeaders.hasHeader() === true).toBeTruthy();
        expect(belowCellsHaveHeader === false).toBeTruthy();

        tableWithHeaders.columnsRows[0].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.number).toBeTruthy()
        );
        tableWithHeaders.columnsRows[1].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === undefined).toBeTruthy()
        );
        tableWithHeaders.columnsRows[2].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.description).toBeTruthy()
        );
        tableWithHeaders.columnsRows[4].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.quantity).toBeTruthy()
        );
        tableWithHeaders.columnsRows[5].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.price).toBeTruthy()
        );
        tableWithHeaders.columnsRows[7].forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.lineTotal).toBeTruthy()
        );
    });

    test("4imprint - return return headers when text contains currecy symbol", async () => {
        const jsonText = JSON.stringify(json4imprint);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];

        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = layoutItemsTableService.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = layoutItemsTableService.getTable(
            dividers,
            selectedLayoutItems,
            topLeftSelection,
            bottomRightSelection
        );

        const tableWithHeaders = target.recognizeTableHeader(table);

        const hasHeader = tableWithHeaders.columnsRows.some(
            (row, index) => index === 0 && row.some((cell) => cell.isHeader)
        );

        const belowCellsHaveHeader = tableWithHeaders.columnsRows.some((row) =>
            row.some((cell, index) => index > 0 && cell.isHeader)
        );

        expect(hasHeader === true).toBeTruthy();
        expect(tableWithHeaders.hasHeader() === true).toBeTruthy();
        expect(belowCellsHaveHeader === false).toBeTruthy();

        const priceColumnWithDollarSymbol = tableWithHeaders.columnsRows[tableWithHeaders.columnsRows.length - 2];
        const totalColumnWithDollarSymbol = tableWithHeaders.columnsRows[tableWithHeaders.columnsRows.length - 1];

        priceColumnWithDollarSymbol.forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.price).toBeTruthy()
        );

        totalColumnWithDollarSymbol.forEach((cell) =>
            expect(cell.autoRecognizedColumnType === LineItemsFieldTypes.lineTotal).toBeTruthy()
        );
    });

    const target = new TableHeaderService();
    const layoutItemsTableService = new LayoutItemsTableService();

    const getBottomRightSelection = (jsonText: string) => {
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();
        const rightItem = _(selectedLayoutItems).maxBy((layoutItem) => layoutItem.bottomRight.x)!;
        const bottomItem = _(selectedLayoutItems).maxBy((layoutItem) => layoutItem.bottomRight.y)!;

        return { x: rightItem.bottomRight.x, y: bottomItem.bottomRight.y };
    };

    const getTopLeftSelection = (jsonText: string) => {
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];

        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const leftItem = _(selectedLayoutItems).minBy((layoutItem) => layoutItem.topLeft.x)!;
        const topItem = _(selectedLayoutItems).minBy((layoutItem) => layoutItem.topLeft.y)!;

        return { x: leftItem.topLeft.x, y: topItem.topLeft.y };
    };
});
