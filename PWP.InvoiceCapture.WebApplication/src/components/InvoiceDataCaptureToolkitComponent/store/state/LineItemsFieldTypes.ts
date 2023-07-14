export enum LineItemsFieldTypes {
    number = "Number #",
    description = "Description",
    quantity = "Qty",
    price = "Unit Price",
    lineTotal = "Line Total"
}

export type LineItemsFieldTypesKey = keyof typeof LineItemsFieldTypes;

export const LineItemsFieldTypesMap: Map<string, LineItemsFieldTypes> = new Map<string, LineItemsFieldTypes>(
    Object.entries(LineItemsFieldTypes).map(([key, value]: [string, LineItemsFieldTypes]) => [key, value])
);

export const LineItemsFieldTypesKeyMap: Map<LineItemsFieldTypes, LineItemsFieldTypesKey> = new Map<
    LineItemsFieldTypes,
    LineItemsFieldTypesKey
>(
    Object.entries(LineItemsFieldTypes).map(([key, value]: [string, LineItemsFieldTypes]) => [
        value,
        key as LineItemsFieldTypesKey
    ])
);
