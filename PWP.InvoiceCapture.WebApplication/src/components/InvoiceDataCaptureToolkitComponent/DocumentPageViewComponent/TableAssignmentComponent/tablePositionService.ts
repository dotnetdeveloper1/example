import { DividerOrientation, IBox, IPoint, ITableDivider } from "../../store/state/index";
export class TablePositionService {
    private readonly initialScaleRatio: number;
    private readonly currentScaleRatio: number;
    private readonly pageOffset: IPoint;
    private readonly tableSelection: IBox;

    private readonly relativeScaleRatio: number;
    private readonly scaledControlAreaOffset: number;
    private readonly dividersControlArea: IBox;

    private readonly dividersControlAreaOffset = 40;

    constructor(
        initialScaleRatio: number,
        currentScaleRatio: number,
        pageOffset: IPoint,
        tableSelection: IBox,
        scrollOffset: IPoint
    ) {
        this.initialScaleRatio = initialScaleRatio;
        this.currentScaleRatio = currentScaleRatio;
        this.pageOffset = pageOffset;
        this.tableSelection = tableSelection;

        this.relativeScaleRatio = this.calculateRelativeScaleRatio();
        this.scaledControlAreaOffset = this.calculateScaledControlAreaOffset();
        this.dividersControlArea = this.calculateDividersControlArea();
    }

    public getDividersControlArea(): IBox {
        return this.dividersControlArea;
    }

    public getRelativeScaleRatio(): number {
        return this.relativeScaleRatio;
    }

    public getScaledControlAreaOffset(): number {
        return this.scaledControlAreaOffset;
    }

    public getPointRelativeToPage(point: IPoint): IPoint {
        return {
            x: point.x - this.dividersControlArea.left,
            y: point.y - this.dividersControlArea.top
        };
    }

    public getDraftDividerPosition(point: IPoint): IPoint {
        return {
            x: point.x - this.pageOffset.x - this.dividersControlArea.left,
            y: point.y - this.pageOffset.y - this.dividersControlArea.top
        };
    }

    public getDividerAbsolutePosition(divider: ITableDivider): IPoint {
        return {
            x: divider.left + this.pageOffset.x,
            y: divider.top + this.pageOffset.y
        };
    }

    public getNewDividerPosition(point: IPoint): IPoint {
        return {
            x: (point.x - this.pageOffset.x) / this.relativeScaleRatio,
            y: (point.y - this.pageOffset.y) / this.relativeScaleRatio
        };
    }

    public getDividerNextPosition(divider: ITableDivider, mousePosition: IPoint): IPoint {
        const mousePositionXRelative = mousePosition.x - this.pageOffset.x - this.dividersControlArea.left;
        const mousePositionYRelative = mousePosition.y - this.pageOffset.y - this.dividersControlArea.top;
        return {
            x:
                mousePositionXRelative >= this.dividersControlArea.width - this.scaledControlAreaOffset ||
                mousePositionXRelative <= this.scaledControlAreaOffset
                    ? divider.left / this.relativeScaleRatio
                    : (mousePositionXRelative + this.dividersControlArea.left) / this.relativeScaleRatio,
            y:
                mousePositionYRelative >= this.dividersControlArea.height - this.scaledControlAreaOffset ||
                mousePositionYRelative <= this.scaledControlAreaOffset
                    ? divider.top / this.relativeScaleRatio
                    : (mousePositionYRelative + this.dividersControlArea.top) / this.relativeScaleRatio
        };
    }

    public getDividerRelativeCoordinates(divider: ITableDivider): ITableDivider {
        return {
            ...divider,
            top: divider.orientation === DividerOrientation.Vertical ? 0 : divider.top - this.dividersControlArea.top,
            left:
                divider.orientation === DividerOrientation.Horizontal ? 0 : divider.left - this.dividersControlArea.left
        };
    }

    public scaleDividers(dividers: ITableDivider[]): ITableDivider[] {
        return dividers.map((divider) => ({
            ...divider,
            left: divider.left * this.relativeScaleRatio,
            top: divider.top * this.relativeScaleRatio
        }));
    }

    public getTable(): IBox {
        return {
            left: this.scaledControlAreaOffset,
            top: this.scaledControlAreaOffset,
            height: this.tableSelection.height * this.relativeScaleRatio,
            width: this.tableSelection.width * this.relativeScaleRatio
        };
    }

    public getTopControlArea(): IBox {
        return {
            left: this.scaledControlAreaOffset,
            top: 0,
            height: this.scaledControlAreaOffset,
            width: this.dividersControlArea.width - 2 * this.scaledControlAreaOffset
        };
    }

    public getBottomControlArea(): IBox {
        return {
            left: this.scaledControlAreaOffset,
            top: this.dividersControlArea.height - this.scaledControlAreaOffset,
            height: this.scaledControlAreaOffset,
            width: this.dividersControlArea.width - 2 * this.scaledControlAreaOffset
        };
    }

    public getLeftControlArea(): IBox {
        return {
            left: 0,
            top: this.scaledControlAreaOffset,
            height: this.dividersControlArea.height - 2 * this.scaledControlAreaOffset,
            width: this.scaledControlAreaOffset
        };
    }

    public getRightControlArea(): IBox {
        return {
            left: this.dividersControlArea.width - this.scaledControlAreaOffset,
            top: this.scaledControlAreaOffset,
            height: this.dividersControlArea.height - 2 * this.scaledControlAreaOffset,
            width: this.scaledControlAreaOffset
        };
    }

    private calculateScaledControlAreaOffset(): number {
        return this.dividersControlAreaOffset * Math.min(this.currentScaleRatio, 1);
    }

    private calculateRelativeScaleRatio(): number {
        return this.currentScaleRatio / this.initialScaleRatio;
    }

    private calculateDividersControlArea(): IBox {
        return {
            left: this.tableSelection.left * this.relativeScaleRatio - this.scaledControlAreaOffset,
            top: this.tableSelection.top * this.relativeScaleRatio - this.scaledControlAreaOffset,
            width: this.tableSelection.width * this.relativeScaleRatio + 2 * this.scaledControlAreaOffset,
            height: this.tableSelection.height * this.relativeScaleRatio + 2 * this.scaledControlAreaOffset
        };
    }
}
