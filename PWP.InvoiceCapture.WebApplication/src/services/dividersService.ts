import _ from "lodash";
import { Column, Row } from "../api/models/InvoiceManagement/InvoiceDataAnnotation";

import {
    DividerOrientation,
    ILayoutItem,
    ITableDivider
} from "../components/InvoiceDataCaptureToolkitComponent/store/state";
import { LayoutItemsService } from "./layoutItemsService";

export class DividersService {
    private readonly itemAndDividerMinDistance: number = 1;

    private readonly layoutItemsService: LayoutItemsService;

    constructor() {
        this.layoutItemsService = new LayoutItemsService(); // TODO: use DI
    }

    public getVerticalDividers(dividers: ITableDivider[]): ITableDivider[] {
        return _(dividers)
            .filter((divider) => divider.orientation === DividerOrientation.Vertical)
            .sortBy((divider) => divider.left)
            .value();
    }

    public getHorizontalDividers(dividers: ITableDivider[]): ITableDivider[] {
        return _(dividers)
            .filter((divider) => divider.orientation === DividerOrientation.Horizontal)
            .sortBy((divider) => divider.top)
            .value();
    }

    public moveDividers(layoutItems: ILayoutItem[], dividers: ITableDivider[]): ITableDivider[] {
        if (!layoutItems || !dividers) {
            return [];
        }

        const verticalDividers = this.getVerticalDividers(dividers);
        const horizontalDividers = this.getHorizontalDividers(dividers);

        const validDividers: ITableDivider[] = [];
        const validLayoutItems = this.layoutItemsService.getValidLayoutItems(layoutItems);

        verticalDividers.forEach((divider) => {
            validDividers.push(this.getVerticalShiftedDivider(validLayoutItems, divider));
        });

        horizontalDividers.forEach((divider) => {
            validDividers.push(this.getHorizontalShiftedDivider(validLayoutItems, divider));
        });

        return validDividers;
    }

    public validateDividers(layoutItems: ILayoutItem[], dividers: ITableDivider[]): ITableDivider[] {
        if (!layoutItems || !dividers) {
            return [];
        }

        const verticalDividers = this.getVerticalDividers(dividers);
        const horizontalDividers = this.getHorizontalDividers(dividers);

        const validLayoutItems = this.layoutItemsService.getValidLayoutItems(layoutItems);

        verticalDividers.forEach((divider) => {
            divider.isValid = !this.hasVerticalIntersection(validLayoutItems, divider);
        });

        horizontalDividers.forEach((divider) => {
            divider.isValid = !this.hasHorizontalIntersection(validLayoutItems, divider);
        });

        return [...verticalDividers, ...horizontalDividers];
    }

    public calculateColumnDividers(columns: Column[]): ITableDivider[] {
        if (!columns || columns.length < 2) {
            return [];
        }

        const sortedColumns = _(columns)
            .sortBy((item) => item.bottomRight.x)
            .value();

        const dividers: ITableDivider[] = [];

        for (let column = 1; column < sortedColumns.length; column++) {
            const previousColumn = sortedColumns[column - 1];
            const nextColumn = sortedColumns[column];

            const columnMiddleDistanceX = Math.abs(nextColumn.topLeft.x - previousColumn.bottomRight.x) / 2;

            const divider: ITableDivider = {
                id: column,
                left: previousColumn.bottomRight.x + columnMiddleDistanceX,
                top: nextColumn.topLeft.y,
                orientation: DividerOrientation.Vertical,
                isValid: true
            };

            dividers.push(divider);
        }

        return dividers;
    }

    public calculateRowDividers(rows: Row[], dividerStartId: number, layoutItems: ILayoutItem[]): ITableDivider[] {
        if (!rows || rows.length < 2 || !layoutItems || layoutItems.length === 0) {
            return [];
        }

        const sortedRows = _(rows)
            .sortBy((item) => item.bottomRight.y)
            .value();

        const dividers: ITableDivider[] = [];

        for (let row = 1; row < sortedRows.length; row++) {
            const nextRow = sortedRows[row];

            const firstAboveDilimeterItemByBottom = _(layoutItems)
                .filter((item) => item.bottomRight.y < nextRow.topLeft.y)
                .maxBy((item) => item.bottomRight.y);

            if (!firstAboveDilimeterItemByBottom) {
                continue;
            }

            const firstAboveDilimeterItemByTop = _(layoutItems)
                .filter(
                    (item) =>
                        firstAboveDilimeterItemByBottom.bottomRight.y < item.topLeft.y &&
                        item.topLeft.y < nextRow.topLeft.y
                )
                .minBy((item) => item.topLeft.y);

            const useItemWithTopCoordinate =
                firstAboveDilimeterItemByTop &&
                firstAboveDilimeterItemByTop.topLeft.y > firstAboveDilimeterItemByBottom.bottomRight.y;

            let rowMiddleDistanceY = 0;
            if (useItemWithTopCoordinate) {
                const goUpperBelowRowDistance = Math.abs(nextRow.topLeft.y - firstAboveDilimeterItemByTop!.topLeft.y);
                const nextRowTopleft = nextRow.topLeft.y - goUpperBelowRowDistance;
                rowMiddleDistanceY =
                    (nextRowTopleft - firstAboveDilimeterItemByBottom!.bottomRight.y) / 2 + goUpperBelowRowDistance;
            } else {
                rowMiddleDistanceY = firstAboveDilimeterItemByBottom
                    ? (nextRow.topLeft.y - firstAboveDilimeterItemByBottom.bottomRight.y) / 2
                    : 0;
            }

            dividers.push({
                id: row + dividerStartId,
                left: nextRow.topLeft.x,
                top: nextRow.topLeft.y - rowMiddleDistanceY,
                orientation: DividerOrientation.Horizontal,
                isValid: true
            });
        }

        return dividers;
    }

    public hasHorizontalIntersection(layoutItems: ILayoutItem[], divider: ITableDivider): ILayoutItem | undefined {
        return layoutItems.find((item) => item.topLeft.y <= divider.top && divider.top <= item.bottomRight.y);
    }

    public hasVerticalIntersection(layoutItems: ILayoutItem[], divider: ITableDivider): ILayoutItem | undefined {
        return layoutItems.find((item) => item.topLeft.x <= divider.left && divider.left <= item.bottomRight.x);
    }

    private getVerticalShiftedDivider(layoutItems: ILayoutItem[], divider: ITableDivider): ITableDivider {
        const intersectedItem = this.hasVerticalIntersection(layoutItems, divider);

        if (intersectedItem) {
            const shiftedValueX = intersectedItem.bottomRight.x + this.itemAndDividerMinDistance;
            const maxX = _(layoutItems).maxBy((item) => item.bottomRight.x)?.bottomRight.x;

            divider.left = maxX && maxX < shiftedValueX ? maxX : shiftedValueX;

            return this.getVerticalShiftedDivider(layoutItems, divider);
        }

        return divider;
    }

    private getHorizontalShiftedDivider(layoutItems: ILayoutItem[], divider: ITableDivider): ITableDivider {
        const intersectedItem = this.hasHorizontalIntersection(layoutItems, divider);

        if (intersectedItem) {
            const shiftedValueY = intersectedItem.bottomRight.y + this.itemAndDividerMinDistance;
            const maxY = _(layoutItems).maxBy((item) => item.bottomRight.y)?.bottomRight.y;

            divider.top = maxY && maxY < shiftedValueY ? maxY : shiftedValueY;

            return this.getHorizontalShiftedDivider(layoutItems, divider);
        }

        return divider;
    }
}
