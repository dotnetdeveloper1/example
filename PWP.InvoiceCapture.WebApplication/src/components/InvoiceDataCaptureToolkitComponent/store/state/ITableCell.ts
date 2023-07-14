import { IPoint } from "./index";
import { ILayoutItem, LineItemsFieldTypes } from "./index";

export interface ITableCell {
    topLeft: IPoint;
    bottomRight: IPoint;
    layoutItems: ILayoutItem[];
    isHeader: boolean;
    autoRecognizedColumnType: LineItemsFieldTypes | undefined;
    autoRecognizedColumnProbabilityOrder: number | undefined;
}
