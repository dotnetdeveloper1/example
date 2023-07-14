import _ from "lodash";
import { Point } from "../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { ILayoutItem } from "../components/InvoiceDataCaptureToolkitComponent/store/state";
import jsonDataFire from "./jsonTestData/fire.json";
import jsonDataSiemensPage7 from "./jsonTestData/SiemensPage7.json";
import { LayoutItemsService } from "./layoutItemsService";

describe("LayoutItemsService", () => {
    test("areOnOneLine - word is on one line", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const layoutItem = selectedLayoutItems[0];

        const topLeft = layoutItem.topLeft;
        const bottomRight = layoutItem.bottomRight;

        const insideLine = target.areOnOneLine(
            { x: topLeft.x, y: topLeft.y - 10 },
            { x: bottomRight.x, y: bottomRight.y + 10 },
            layoutItem
        );

        const sameCoordinates = target.areOnOneLine(
            { x: topLeft.x, y: topLeft.y },
            { x: bottomRight.x, y: bottomRight.y },
            layoutItem
        );

        const includeLine = target.areOnOneLine(
            { x: topLeft.x, y: topLeft.y + 1 },
            { x: bottomRight.x, y: bottomRight.y - 1 },
            layoutItem
        );

        const onlyBottomInsideLine = target.areOnOneLine(
            { x: topLeft.x, y: topLeft.y + 1 },
            { x: bottomRight.x, y: bottomRight.y },
            layoutItem
        );

        const onlyTopInsideLine = target.areOnOneLine(
            { x: topLeft.x, y: topLeft.y },
            { x: bottomRight.x, y: bottomRight.y + 1 },
            layoutItem
        );

        expect(insideLine).toBeTruthy();
        expect(sameCoordinates).toBeTruthy();
        expect(includeLine).toBeTruthy();
        expect(onlyBottomInsideLine).toBeTruthy();
        expect(onlyTopInsideLine).toBeTruthy();
    });

    test("areOnOneLine - word is outside line", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const layoutItem = selectedLayoutItems[0];

        const topLeft = layoutItem.topLeft;
        const bottomRight = layoutItem.bottomRight;

        const belowLine = !target.areOnOneLine(
            { x: topLeft.x, y: topLeft.y - 100 },
            { x: bottomRight.x, y: bottomRight.y - 90 },
            layoutItem
        );

        const upperLine = !target.areOnOneLine(
            { x: topLeft.x, y: topLeft.y + 90 },
            { x: bottomRight.x, y: bottomRight.y + 100 },
            layoutItem
        );

        expect(belowLine).toBeTruthy();
        expect(upperLine).toBeTruthy();
    });

    test("getSelectedLayoutItems - contains one word", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const layoutItem = selectedLayoutItems[0];

        const topLeft = layoutItem.topLeft;
        const bottomRight = layoutItem.bottomRight;

        const selectedItems = target.getSelectedLayoutItems(
            { x: topLeft.x - 1, y: topLeft.y - 1 },
            { x: bottomRight.x + 1, y: bottomRight.y + 1 },
            selectedLayoutItems
        );

        expect(selectedItems.length === 1).toBeTruthy();
    });

    test("getItemHeight - height calculated using bottomRight and topLeft", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const layoutItem = selectedLayoutItems[0];

        const actualHeight = target.getItemHeight(layoutItem);

        const expectedHeight = layoutItem.bottomRight.y - layoutItem.topLeft.y;

        expect(expectedHeight === actualHeight).toBeTruthy();
    });

    test("getTopLeftMin - return top y and left x", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const leftItem = _(selectedLayoutItems).minBy((layoutItem) => layoutItem.topLeft.x)!;
        const topItem = _(selectedLayoutItems).minBy((layoutItem) => layoutItem.topLeft.y)!;

        const expectedPoint: Point = { x: leftItem.topLeft.x, y: topItem.topLeft.y };

        const topLeftMin = target.getTopLeftMin(selectedLayoutItems);

        expect(topLeftMin.x === expectedPoint.x).toBeTruthy();
        expect(topLeftMin.y === expectedPoint.y).toBeTruthy();
    });

    test("getBottomRightMax - return bottom y and right x", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const rightItem = _(selectedLayoutItems).maxBy((layoutItem) => layoutItem.bottomRight.x)!;
        const bottomItem = _(selectedLayoutItems).maxBy((layoutItem) => layoutItem.bottomRight.y)!;

        const expectedPoint: Point = { x: rightItem.bottomRight.x, y: bottomItem.bottomRight.y };

        const bottomRightMax = target.getBottomRightMax(selectedLayoutItems);

        expect(bottomRightMax.x === expectedPoint.x).toBeTruthy();
        expect(bottomRightMax.y === expectedPoint.y).toBeTruthy();
    });

    test("findIntersections - return 71 intersected words", async () => {
        const jsonText = JSON.stringify(jsonDataSiemensPage7);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const intersections = target.findIntersections(
            [selectedLayoutItems[2], selectedLayoutItems[3], selectedLayoutItems[4]],
            selectedLayoutItems
        );

        expect(intersections.length === 71).toBeTruthy();
    });

    test("findIntersections - return 3 intersected words", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const intersections = target.findIntersections([selectedLayoutItems[0]], selectedLayoutItems);

        expect(intersections.length === 3).toBeTruthy();
    });

    test("findIntersections - return 3 intersected words in case we start from below element", async () => {
        const jsonText = JSON.stringify(jsonDataFire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const intersections = target.findIntersections([selectedLayoutItems[37]], selectedLayoutItems);

        expect(intersections.length === 3).toBeTruthy();
    });

    const target = new LayoutItemsService();
});
