import { useLayoutEffect, useRef, useState } from "react";
import { ReactPropsType } from "../../../../../helperTypes";
import { IItemFieldsMenuComponentState } from "../../hooks/useFieldAssignmentMenuControl";
import { ItemFieldsMenuComponent } from "../ItemFieldsMenuComponent";
import { LIST_ITEM_SUFFIX } from "../ListItem";

interface IListItemHookResult {
    isItemFieldsMenuOpened: boolean;
    componentRef: React.RefObject<HTMLDivElement>;
    itemFieldsState: IItemFieldsMenuComponentState;
}

export function useItemFieldsMenu(props: ReactPropsType<typeof ItemFieldsMenuComponent>): IListItemHookResult {
    const componentRef = useRef<HTMLDivElement>(null);

    const [itemFieldsState, setItemFieldsState] = useState<IItemFieldsMenuComponentState>({
        isOpen: props.isOpen,
        top: 0,
        left: 0,
        orderNumber: 1
    });

    useLayoutEffect(() => {
        if (!props.isOpen) {
            return;
        }
        const globalRootElement = document.getElementById("root");
        const rootBoundingBox = globalRootElement && globalRootElement.getBoundingClientRect();
        const listItemDiv = document.getElementById(`${props.orderNumber}${LIST_ITEM_SUFFIX}`);
        const listItemDivBox = listItemDiv && listItemDiv.getBoundingClientRect();
        const componentBoundingBox = componentRef.current && componentRef.current.getBoundingClientRect();

        const top = findAvailableScreenTopPosition(rootBoundingBox, componentBoundingBox, listItemDivBox?.top ?? 0);
        const fieldsState = {
            isOpen: props.isOpen,
            left: listItemDivBox?.right ?? 0,
            top: top,
            orderNumber: props.orderNumber
        };
        setItemFieldsState(fieldsState);
    }, [props, setItemFieldsState, componentRef]);

    return {
        isItemFieldsMenuOpened: itemFieldsState.isOpen,
        componentRef: componentRef,
        itemFieldsState: itemFieldsState
    };
}

function findAvailableScreenTopPosition(
    rootBoundingBox: DOMRect | null,
    componentBoundingBox: DOMRect | null,
    top: number
): number {
    return rootBoundingBox && componentBoundingBox && rootBoundingBox.height < componentBoundingBox.height + top
        ? rootBoundingBox.height - componentBoundingBox.height
        : top;
}
