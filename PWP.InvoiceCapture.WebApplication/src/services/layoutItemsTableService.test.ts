import _ from "lodash";
import { DividerOrientation, ILayoutItem } from "../components/InvoiceDataCaptureToolkitComponent/store/state";
import jsonDataCapterraWithShiftedHeaders from "./jsonTestData/capterraWithShiftedHeaders.json";
import jsonFire from "./jsonTestData/fire.json";
import jsonDataAllItems from "./jsonTestData/fireAllItemsSelected.json";
import jsonDataOneLongSentence from "./jsonTestData/fireOneLongSentenceSelected.json";
import jsonGrainger from "./jsonTestData/Grainger.json";
import jsonDataNoItems from "./jsonTestData/noLayoutItems.json";
import jsonDataOneRowTwoItems from "./jsonTestData/oneRowTwoItems.json";
import jsonDataSiemensPage7 from "./jsonTestData/SiemensPage7.json";
import jsonDataWasteManagement from "./jsonTestData/WasteManagement.json";
import { LayoutItemsTableService } from "./layoutItemsTableService";

describe("LayoutItemsTableService", () => {
    test("getSelectedLayoutItems - selected only items received", async () => {
        const jsonText = JSON.stringify(jsonFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const topLeftSelection = getTopLeftSelection(jsonText);
        const bottomRightSelection = getBottomRightSelection(jsonText);

        const selectedDocumentLayoutItems = target.getSelectedLayoutItems(
            topLeftSelection,
            bottomRightSelection,
            documentLayoutItems
        );

        expect(selectedLayoutItems.length === selectedDocumentLayoutItems.length).toBeTruthy();
        expect(selectedDocumentLayoutItems.every((item) => topLeftSelection.x <= item.topLeft.x)).toBeTruthy();
        expect(selectedDocumentLayoutItems.every((item) => item.bottomRight.x <= bottomRightSelection.x)).toBeTruthy();
    });

    test("getDividers - 3 rows and 4 columns received", async () => {
        const jsonText = JSON.stringify(jsonFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Horizontal).length === 2
        ).toBeTruthy();

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Vertical).length === 3
        ).toBeTruthy();
    });

    test("getDividers - 1 row and 3 columns received from table with shifter headers", async () => {
        const jsonText = JSON.stringify(jsonDataCapterraWithShiftedHeaders);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Horizontal).length === 1
        ).toBeTruthy();

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Vertical).length === 3
        ).toBeTruthy();
    });

    test("getDividers - 0 dividers received with two close items", async () => {
        const jsonText = JSON.stringify(jsonDataOneRowTwoItems);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const columnDividers = target.getDividers(selectedLayoutItems);

        expect(columnDividers.length === 0).toBeTruthy();
    });

    test("getDividers - works faster then 0.5 seconds when all items selected", async () => {
        const jsonText = JSON.stringify(jsonDataAllItems);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const start = performance.now();
        target.getDividers(selectedLayoutItems);
        const end = performance.now();

        const executionTimeMilliseconds = end - start;

        expect(executionTimeMilliseconds < 500).toBeTruthy();
    });

    test("getRowsDividers - horizontal dividers in the middle between layout items", async () => {
        const jsonText = JSON.stringify(jsonGrainger);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const horizontalDividers = _(dividers)
            .filter((divider) => divider.orientation === DividerOrientation.Horizontal)
            .value();

        horizontalDividers.forEach((currentDivider) => {
            const firstAboveDilimeterItem = _(selectedLayoutItems)
                .filter((item) => item.bottomRight.y < currentDivider.top)
                .maxBy((item) => item.bottomRight.y)!;

            const firstBelowDilimeterItem = _(selectedLayoutItems)
                .filter((item) => item.topLeft.y > currentDivider.top)
                .minBy((item) => item.topLeft.y)!;

            const middleY =
                firstBelowDilimeterItem.topLeft.y -
                (firstBelowDilimeterItem.topLeft.y - firstAboveDilimeterItem.bottomRight.y) / 2;

            expect(middleY === currentDivider.top).toBeTruthy();
            expect(firstAboveDilimeterItem.bottomRight.y < currentDivider.top).toBeTruthy();
            expect(currentDivider.top < firstBelowDilimeterItem.topLeft.y).toBeTruthy();
        });
    });

    test("getRowsDividers - vertical dividers in the middle between layout items", async () => {
        const jsonText = JSON.stringify(jsonFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const verticalDividers = _(dividers)
            .filter((divider) => divider.orientation === DividerOrientation.Vertical)
            .value();

        expect(verticalDividers.length === 3).toBeTruthy();

        verticalDividers.forEach((currentDivider) => {
            const firstRightDilimeterItem = _(selectedLayoutItems)
                .filter((item) => currentDivider.left < item.topLeft.x)
                .minBy((item) => item.bottomRight.x)!;

            const firstLeftDilimeterItem = _(selectedLayoutItems)
                .filter((item) => item.bottomRight.x < currentDivider.left)
                .maxBy((item) => item.topLeft.x)!;

            expect(firstLeftDilimeterItem.bottomRight.x < currentDivider.left).toBeTruthy();
            expect(currentDivider.left < firstRightDilimeterItem.topLeft.x).toBeTruthy();
        });
    });

    test("getRowsDividers - 0 dividers returned when one long sentense selected", async () => {
        const jsonText = JSON.stringify(jsonDataOneLongSentence);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        expect(dividers.length === 0).toBeTruthy();
    });

    test("getRowsDividers - 0 rows, 4 columns received if 2 columns contains 2 rows and 2 columns has 5 rows, but there is intersection between layout items", async () => {
        const jsonText = JSON.stringify(jsonDataWasteManagement);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Horizontal).length === 0
        ).toBeTruthy();

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Vertical).length === 3
        ).toBeTruthy();
    });

    test("getRowsDividers - 4 rows, 6 columns received if 3 Clmns with 3 Rws And 1 Clmn with 23 Rws And 2 Clmns 5 Rws", async () => {
        const jsonText = JSON.stringify(jsonDataSiemensPage7);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Horizontal).length === 3
        ).toBeTruthy();

        expect(
            dividers.filter((divider) => divider.orientation === DividerOrientation.Vertical).length === 5
        ).toBeTruthy();
    });

    test("getTable - table with 3 rows and 4 columns received", async () => {
        const jsonText = JSON.stringify(jsonFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = target.getTable(dividers, selectedLayoutItems, topLeftSelection, bottomRightSelection);

        expect(table.columnsRows.length === 4).toBeTruthy();
        expect(table.columnsRows[0].length === 3).toBeTruthy();
    });

    test("getTable - returned 1 column and 1 row if 0 layout items and 0 dividers", async () => {
        const jsonText = JSON.stringify(jsonDataNoItems);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const table = target.getTable(dividers, selectedLayoutItems, { x: 0, y: 0 }, { x: 100, y: 100 });

        expect(table.columnsRows.length === 1).toBeTruthy();
        expect(table.columnsRows[0].length === 1).toBeTruthy();
        expect(table.columnsRows[0][0].layoutItems.length === 0).toBeTruthy();
    });

    test("getTable - table cell coordinates are correct", async () => {
        const jsonText = JSON.stringify(jsonDataSiemensPage7);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = target.getTable(dividers, selectedLayoutItems, topLeftSelection, bottomRightSelection);

        table.columnsRows.forEach((column) => {
            column.forEach((cell) => {
                expect(cell.topLeft.x < cell.bottomRight.x).toBeTruthy();
                expect(cell.topLeft.y < cell.bottomRight.y).toBeTruthy();
                expect(
                    !cell.layoutItems.some(
                        (item) =>
                            item.topLeft.x < cell.topLeft.x ||
                            item.topLeft.y < cell.topLeft.y ||
                            item.bottomRight.x > cell.bottomRight.x ||
                            item.bottomRight.y > cell.bottomRight.y
                    )
                ).toBeTruthy();
            });
        });
    });

    test("getColumnNumber - return undefined if point not inside table", async () => {
        const jsonText = JSON.stringify(jsonDataSiemensPage7);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = target.getTable(dividers, selectedLayoutItems, topLeftSelection, bottomRightSelection);

        const columnNumber = target.getColumnNumber({ x: 1, y: 1 }, table.columnsRows);

        expect(columnNumber === undefined).toBeTruthy();
    });

    test("getColumnNumber - return first and last column numbers", async () => {
        const jsonText = JSON.stringify(jsonDataSiemensPage7);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = target.getTable(dividers, selectedLayoutItems, topLeftSelection, bottomRightSelection);

        const columnNumber0 = target.getColumnNumber({ x: 86, y: 311 }, table.columnsRows);
        const columnNumber5 = target.getColumnNumber({ x: 1193, y: 864 }, table.columnsRows);

        expect(columnNumber0 === 0).toBeTruthy();
        expect(columnNumber5 === 5).toBeTruthy();
    });

    test("getColumnLayoutItems - return layout items from first colulmn", async () => {
        const jsonText = JSON.stringify(jsonDataSiemensPage7);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = target.getTable(dividers, selectedLayoutItems, topLeftSelection, bottomRightSelection);

        const point = { x: 86, y: 311 };

        const columnNumber0 = target.getColumnNumber(point, table.columnsRows);

        const expectedItems = _(table.columnsRows[columnNumber0!])
            .flatMap((column) => column.layoutItems)
            .value();

        const actualItems = target.getColumnLayoutItems(point, table.columnsRows);

        expectedItems.forEach((expectedItem) => {
            const actualItem = actualItems.find((item) => item.id === expectedItem.id);
            expect(actualItem).toBeTruthy();
        });
    });

    test("getColumnLayoutItems - return empty list for incorrect point", async () => {
        const jsonText = JSON.stringify(jsonDataSiemensPage7);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = target.getDividers(selectedLayoutItems);

        const topLeftSelection = getTopLeftSelection(jsonText)!;
        const bottomRightSelection = getBottomRightSelection(jsonText)!;

        const table = target.getTable(dividers, selectedLayoutItems, topLeftSelection, bottomRightSelection);

        const point = { x: 1, y: 1 };

        const actualItems = target.getColumnLayoutItems(point, table.columnsRows);
        expect(actualItems.length === 0).toBeTruthy();
    });

    const target = new LayoutItemsTableService();

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
