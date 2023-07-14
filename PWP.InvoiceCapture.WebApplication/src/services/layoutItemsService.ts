import _ from "lodash";
import { Point } from "../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { ILayoutItem } from "../components/InvoiceDataCaptureToolkitComponent/store/state";

export class LayoutItemsService {
    public getSelectedLayoutItems(topLeft: Point, bottomRight: Point, layoutItems: ILayoutItem[]): ILayoutItem[] {
        if (!layoutItems) {
            return [];
        }
        if (bottomRight.x < 0 || bottomRight.y < 0) {
            return [];
        }
        if (topLeft.x > bottomRight.x || topLeft.y > bottomRight.y) {
            return [];
        }

        const bottomRightLayoutItems = _(layoutItems)
            .filter((layoutItem) => topLeft.x <= layoutItem.bottomRight.x && topLeft.y <= layoutItem.bottomRight.y)
            .value();

        return _(bottomRightLayoutItems)
            .filter((layoutItem) => layoutItem.topLeft.x <= bottomRight.x && layoutItem.topLeft.y <= bottomRight.y)
            .value();
    }

    public getValidLayoutItems(layoutItems: ILayoutItem[]): ILayoutItem[] {
        if (!layoutItems) {
            return [];
        }

        return _(layoutItems)
            .filter(
                (layoutItem) =>
                    !!layoutItem.topLeft &&
                    !!layoutItem.topLeft.x &&
                    !!layoutItem.topLeft.y &&
                    !!layoutItem.bottomRight &&
                    !!layoutItem.bottomRight.x &&
                    !!layoutItem.bottomRight.y
            )
            .value();
    }

    public getBottomRightMax(layoutItems: ILayoutItem[]): Point {
        if (!layoutItems) {
            return { x: 0, y: 0 };
        }

        const rightItem = _(layoutItems).maxBy((layoutItem) => layoutItem.bottomRight.x);
        const bottomItem = _(layoutItems).maxBy((layoutItem) => layoutItem.bottomRight.y);

        if (rightItem && bottomItem) {
            return { x: rightItem.bottomRight.x, y: bottomItem.bottomRight.y };
        }

        return { x: 0, y: 0 };
    }

    public getTopLeftMin(layoutItems: ILayoutItem[]): Point {
        if (!layoutItems) {
            return { x: 0, y: 0 };
        }

        const leftItem = _(layoutItems).minBy((layoutItem) => layoutItem.topLeft.x);
        const topItem = _(layoutItems).minBy((layoutItem) => layoutItem.topLeft.y);

        if (leftItem && topItem) {
            return { x: leftItem.topLeft.x, y: topItem.topLeft.y };
        }

        return { x: 0, y: 0 };
    }

    public getItemHeight(item1: ILayoutItem): number {
        if (!item1) {
            return 0;
        }

        return item1.bottomRight.y - item1.topLeft.y;
    }

    public findIntersections(previousIntersections: ILayoutItem[], layoutItems: ILayoutItem[]): ILayoutItem[] {
        const previousIntersectionsTopLeft = this.getTopLeftMin(previousIntersections);
        const previousIntersectionsBottomRight = this.getBottomRightMax(previousIntersections);

        // Layout items are very close to each other   ------- -------
        let intersections = _(layoutItems)
            .filter((item) =>
                this.areOnOneLineAndVeryClose(previousIntersectionsTopLeft, previousIntersectionsBottomRight, item)
            )
            .value();

        const minX = this.getTopLeftMin(intersections).x;
        const maxX = this.getBottomRightMax(intersections).x;

        // Not this case
        // layoutItem1            -------
        // layoutItem2   -------
        intersections = _(layoutItems)
            .filter((item) => !(item.bottomRight.x < minX))
            .value();

        // And not this case
        // layoutItem1   -------
        // layoutItem2            -------
        intersections = _(layoutItems)
            .filter((item) => !(maxX < item.topLeft.x))
            .value();

        const restItems = layoutItems.filter(
            (item) => !intersections.some((intersection) => intersection.id === item.id)
        );

        if (restItems.length === 0 || intersections.length === 0) {
            return intersections;
        }

        const recursiveIntersections = this.findIntersections(intersections, restItems);

        return [...intersections, ...recursiveIntersections];
    }

    public areOnOneLine(topLeft: Point, bottomRight: Point, item: ILayoutItem): boolean {
        const bottomOnOneLine = Math.abs(bottomRight.y - item.bottomRight.y) < this.getItemHeight(item);
        const topOnOneLine = Math.abs(topLeft.y - item.topLeft.y) < this.getItemHeight(item);

        // Intersection       Item
        // --- -
        // -- ---             ----
        // - -- ----
        // --- -
        const isItemOnOneLineWithIntersection = topLeft.y <= item.topLeft.y && item.bottomRight.y <= bottomRight.y;

        return bottomOnOneLine || topOnOneLine || isItemOnOneLineWithIntersection;
    }

    private areOnOneLineAndVeryClose(topLeft: Point, bottomRight: Point, item: ILayoutItem): boolean {
        if (!this.areOnOneLine(topLeft, bottomRight, item)) {
            return false;
        }

        const rightDistance = item.topLeft.x - bottomRight.x;

        return rightDistance <= this.getItemHeight(item);
    }
}
