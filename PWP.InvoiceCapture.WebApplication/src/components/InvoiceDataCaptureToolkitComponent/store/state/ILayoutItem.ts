export interface IPoint {
    readonly x: number;
    readonly y: number;
}

export interface ILayoutItem {
    readonly id: string;
    readonly text: string;
    readonly value: string;
    readonly topLeft: IPoint;
    readonly bottomRight: IPoint;
    assigned: boolean;
    selected: boolean;
    inFocus?: boolean;
}
