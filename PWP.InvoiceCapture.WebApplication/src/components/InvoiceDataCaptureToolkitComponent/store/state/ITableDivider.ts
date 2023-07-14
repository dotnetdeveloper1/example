export enum DividerOrientation {
    Horizontal,
    Vertical
}

export interface ITableDivider {
    id: number;
    top: number;
    left: number;
    height?: number;
    width?: number;
    orientation: DividerOrientation;
    isValid: boolean;
}
