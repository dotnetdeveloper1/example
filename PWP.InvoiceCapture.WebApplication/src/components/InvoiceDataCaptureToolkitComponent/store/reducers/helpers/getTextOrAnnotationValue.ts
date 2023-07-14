import { FieldType } from "./../../../../../api/models/InvoiceFields/FieldType";
import { ILayoutItem } from "./../../state/ILayoutItem";
import { sortLayoutItemsById } from "./sortLayoutItems";

interface ILayoutItemsGroup {
    layoutItemsIds: string[];
    groupValue: string;
}

export function getTextOrAnnotationValue(selectedLayoutItems: ILayoutItem[], fieldType: FieldType): ILayoutItemsGroup {
    return fieldType === FieldType.Decimal
        ? getValuesFromSelectedLayoutItems(selectedLayoutItems)
        : getPlainTextFromSelectedLayoutItems(selectedLayoutItems);
}

export function getPlainTextFromSelectedLayoutItems(layoutItems: ILayoutItem[]): ILayoutItemsGroup {
    const sortedLayoutItems = sortLayoutItemsById(layoutItems);
    return {
        layoutItemsIds: sortedLayoutItems.map((layoutItem) => layoutItem.id),
        groupValue: sortedLayoutItems.map((layoutItem) => layoutItem.text).join(" ")
    };
}

function getValuesFromSelectedLayoutItems(layoutItems: ILayoutItem[]): ILayoutItemsGroup {
    const sortedLayoutItems = sortLayoutItemsById(layoutItems);
    const layoutItemsValues: string[] = [];
    const layoutItemsToAssign: ILayoutItem[] = [];
    const plainText = sortedLayoutItems.map((layoutItem) => layoutItem.text).join(" ");
    const plainTextWithoutSpaces = plainText.replace(/\s/g, "");

    if (plainTextWithoutSpaces.match(/\d+,?\d*\.?\d*/g)?.length !== 1) {
        return {
            layoutItemsIds: sortedLayoutItems.map((layoutItem) => layoutItem.id),
            groupValue: plainText === "-" ? "0" : plainText
        };
    }

    const filteredLayoutItems = sortedLayoutItems.filter(
        (item) => item.text === "-" || item.text.match(/-?\d+\.?\d*/g)
    );

    // tslint:disable-next-line: prefer-for-of
    for (let i = 0; i < filteredLayoutItems.length; i++) {
        const cleanValue = filteredLayoutItems[i].text.replace(/[^\d\-.]/g, "");
        if (cleanValue && cleanValue.length > 0 && cleanValue !== "-") {
            layoutItemsValues.push(cleanValue);
            layoutItemsToAssign.push(filteredLayoutItems[i]);
        }
    }

    let groupValue = layoutItemsValues.join("");
    groupValue = groupValue === "-" ? "0" : groupValue;

    return {
        layoutItemsIds: layoutItemsToAssign.map((layoutItem) => layoutItem.id),
        groupValue: groupValue
    };
}
