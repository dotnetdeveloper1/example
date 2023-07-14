import _ from "lodash";
import { ILayoutItem } from "../components/InvoiceDataCaptureToolkitComponent/store/state";
import { DividersService } from "./dividersService";
import fire from "./jsonTestData/fire.json";
import wasteManagement from "./jsonTestData/WasteManagement.json";
import { LayoutItemsTableService } from "./layoutItemsTableService";

describe("DividersService", () => {
    test("moveDividers - move divider to the right", async () => {
        const jsonText = JSON.stringify(fire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = new LayoutItemsTableService().getDividers(selectedLayoutItems);

        // site word coordinates
        // "topLeft": { "x": 314, "y": 623 },
        // "bottomRight": { "x": 340, "y": 636 },
        const intersectionWithWordX = 315;
        dividers[0].left = intersectionWithWordX;

        const movedDividers = target.moveDividers(selectedLayoutItems, dividers);

        expect(movedDividers[0].left > intersectionWithWordX).toBeTruthy();
    });

    test("moveDividers - move divider below", async () => {
        const jsonText = JSON.stringify(fire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = new LayoutItemsTableService().getDividers(selectedLayoutItems);

        // site word coordinates
        // "topLeft": { "x": 314, "y": 623 },
        // "bottomRight": { "x": 340, "y": 636 },
        const intersectionWithWordY = 624;
        dividers[3].top = intersectionWithWordY;

        const movedDividers = target.moveDividers(selectedLayoutItems, dividers);

        expect(movedDividers[3].top > intersectionWithWordY).toBeTruthy();
    });

    test("validateDividers - validate vertical dividers", async () => {
        const jsonText = JSON.stringify(fire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = new LayoutItemsTableService().getDividers(selectedLayoutItems);

        // site word coordinates
        // "topLeft": { "x": 314, "y": 623 },
        // "bottomRight": { "x": 340, "y": 636 },
        const intersectionWithWordY = 624;
        dividers[3].top = intersectionWithWordY;

        const validatedDividers = target.validateDividers(selectedLayoutItems, dividers);

        const validDividers = validatedDividers.filter((divider) => divider.isValid);
        const invalidDividers = validatedDividers.filter((divider) => !divider.isValid);

        expect(validDividers.length === 4).toBeTruthy();
        expect(invalidDividers.length === 1).toBeTruthy();
        expect(invalidDividers[0].top === intersectionWithWordY).toBeTruthy();
    });

    test("validateDividers - validate horizontal dividers", async () => {
        const jsonText = JSON.stringify(fire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = new LayoutItemsTableService().getDividers(selectedLayoutItems);

        // site word coordinates
        // "topLeft": { "x": 314, "y": 623 },
        // "bottomRight": { "x": 340, "y": 636 },
        const intersectionWithWordX = 315;
        dividers[0].left = intersectionWithWordX;

        const validatedDividers = target.validateDividers(selectedLayoutItems, dividers);

        const validDividers = validatedDividers.filter((divider) => divider.isValid);
        const invalidDividers = validatedDividers.filter((divider) => !divider.isValid);

        expect(validDividers.length === 4).toBeTruthy();
        expect(invalidDividers.length === 1).toBeTruthy();
        expect(invalidDividers[0].left === intersectionWithWordX).toBeTruthy();
    });

    test("validateDividers - all valid", async () => {
        const jsonText = JSON.stringify(fire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = new LayoutItemsTableService().getDividers(selectedLayoutItems);

        const validatedDividers = target.validateDividers(selectedLayoutItems, dividers);

        const validDividers = validatedDividers.filter((divider) => divider.isValid);
        const invalidDividers = validatedDividers.filter((divider) => !divider.isValid);

        expect(validDividers.length === 5).toBeTruthy();
        expect(invalidDividers.length === 0).toBeTruthy();
    });

    test("getHorizontalDividers - return 2 dividers", async () => {
        const jsonText = JSON.stringify(fire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = new LayoutItemsTableService().getDividers(selectedLayoutItems);

        const horizontalDividers = target.getHorizontalDividers(dividers);

        expect(horizontalDividers.length === 2).toBeTruthy();
    });

    test("getVerticalDividers - return 2 dividers", async () => {
        const jsonText = JSON.stringify(fire);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const dividers = new LayoutItemsTableService().getDividers(selectedLayoutItems);

        const verticalDividers = target.getVerticalDividers(dividers);

        expect(verticalDividers.length === 3).toBeTruthy();
    });

    test("calculateColumnDividers - return 25 not validated dividers", async () => {
        const jsonText = JSON.stringify(wasteManagement);
        const documentLayoutItems = JSON.parse(jsonText).pageLayoutItems as ILayoutItem[];
        const selectedLayoutItems = _(documentLayoutItems)
            .filter((item) => item.selected === true)
            .value();

        const notValidatedVerticalDividers = target.calculateColumnDividers(selectedLayoutItems);

        expect(notValidatedVerticalDividers.length === 25).toBeTruthy();
    });

    const target = new DividersService();
});
