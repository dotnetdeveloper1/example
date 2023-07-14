import { IPoint } from "./index";
export interface ITable {
    topLeft: IPoint;
    bottomRight: IPoint;
    isAccepted: boolean;
}
