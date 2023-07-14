import { useCallback, useEffect, useLayoutEffect, useRef, useState } from "react";
import { FieldAssignmentMenuComponent } from "../FieldAssignmentMenuComponent";
import { ITEM_FIELDS_CONTAINER_ID } from "../ItemsListAssignmentMenuComponent/ItemFieldsMenuComponent";
import { LIST_ITEMS_MENU_CONTAINER_ID } from "../ItemsListAssignmentMenuComponent/ItemsListMenuComponent";
import { ReactPropsType } from "./../../../../helperTypes";
import { BrowserIdentifier } from "./../../../../utils/browserIdentifier";
import { LineItemsFieldTypes } from "./../../store/state";
import { IItemFieldsMenuComponentState, IItemsListMenuComponentState } from "./useFieldAssignmentMenuControl";

interface IFieldAssignmentMenuHookResult {
    isEdge: boolean;
    topOffset: number;
    componentRef: React.RefObject<HTMLDivElement>;
    onAssignField(fieldId: string): () => void;
    onAssignLineItemField(fieldType: LineItemsFieldTypes, orderNumber: number): () => void;
    onOpenListItems: (event: React.MouseEvent) => void;
    onEnterMainMenuField: () => void;
    onOpenItemFields: (event: React.MouseEvent, orderNumber: number) => void;
    itemFieldsMenuState: IItemFieldsMenuComponentState;
    listItemsMenuState: IItemsListMenuComponentState;
}

export function useFieldAssignmentMenu(
    props: ReactPropsType<typeof FieldAssignmentMenuComponent>
): IFieldAssignmentMenuHookResult {
    const componentRef = useRef<HTMLDivElement>(null);

    const [topOffset, setTopOffset] = useState<number>(props.top);
    const [itemsListState, setItemsListState] = useState<IItemsListMenuComponentState>(getDefaultItemsListState);
    const [itemFieldsState, setItemFieldsState] = useState<IItemFieldsMenuComponentState>(getDefaultItemFieldsState);

    const onAssignField = useCallback(
        (fieldName) => () => {
            props.onAssignItems(fieldName);
            props.onClose();
        },
        [props]
    );

    const onAssignLineItemField = useCallback(
        (fieldName, orderNumber) => () => {
            props.onAssignLineItems(fieldName, orderNumber);
            props.onClose();
        },
        [props]
    );

    const onCloseListItemsMenu = useCallback(() => {
        setItemsListState(getDefaultItemsListState());
        setItemFieldsState(getDefaultItemFieldsState());
    }, []);

    const handleGlobalMouseDown = useCallback(onOutOfContextMenuBoxClick(componentRef, props, onCloseListItemsMenu), [
        componentRef,
        props
    ]);

    useEffect(() => {
        window.addEventListener("mousedown", handleGlobalMouseDown);

        return () => {
            window.removeEventListener("mousedown", handleGlobalMouseDown);
        };
    }, [handleGlobalMouseDown]);

    useLayoutEffect(() => {
        const globalRootElement = document.getElementById("root");
        const rootBoundingBox = globalRootElement && globalRootElement.getBoundingClientRect();
        const componentBoundingBox = componentRef.current && componentRef.current.getBoundingClientRect();
        setTopOffset(findAvailableScreenTopPosition(rootBoundingBox, componentBoundingBox, props.top));
    }, [props, setTopOffset, componentRef]);

    const onOpenListItems = useCallback(
        (event: React.MouseEvent) => {
            const itemsListDiv = event.target as HTMLElement;
            const itemsListDivBoundingBox = itemsListDiv && itemsListDiv.getBoundingClientRect();

            setItemsListState({
                isOpen: true,
                left: itemsListDivBoundingBox?.right ?? 0,
                top: itemsListDivBoundingBox?.top ?? 0
            });
        },
        [setItemsListState]
    );

    const onOpenListItemFields = useCallback(
        (event: React.MouseEvent, orderNumber: number) => {
            const itemDiv = event.target as HTMLElement;
            const itemDivBoundingBox = itemDiv && itemDiv.getBoundingClientRect();

            setItemFieldsState({
                isOpen: true,
                left: itemDivBoundingBox?.right ?? 0,
                top: itemDivBoundingBox?.top ?? 0,
                orderNumber: orderNumber
            });
        },
        [setItemFieldsState]
    );

    return {
        isEdge: BrowserIdentifier.IsEdge(),
        topOffset: topOffset,
        componentRef: componentRef,
        onAssignField: onAssignField,
        onAssignLineItemField: onAssignLineItemField,
        onOpenListItems: onOpenListItems,
        onEnterMainMenuField: onCloseListItemsMenu,
        onOpenItemFields: onOpenListItemFields,
        itemFieldsMenuState: itemFieldsState,
        listItemsMenuState: itemsListState
    };
}

function getDefaultItemFieldsState(): IItemFieldsMenuComponentState {
    return {
        isOpen: false,
        left: 0,
        top: 0,
        orderNumber: 1
    };
}

function getDefaultItemsListState(): IItemsListMenuComponentState {
    return {
        isOpen: false,
        left: 0,
        top: 0
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

function onOutOfContextMenuBoxClick(
    componentRef: React.RefObject<HTMLDivElement>,
    props: ReactPropsType<typeof FieldAssignmentMenuComponent>,
    closeSubMenus: () => void
): (event: MouseEvent) => void {
    return (event) => {
        const isOutOfComponentBounding = isOutOfComponent(componentRef.current as HTMLDivElement, event);

        const listItemsDiv = document.getElementById(LIST_ITEMS_MENU_CONTAINER_ID) as HTMLDivElement;
        const isOutOfListItemsBounding = isOutOfComponent(listItemsDiv, event);

        const itemFieldsDiv = document.getElementById(ITEM_FIELDS_CONTAINER_ID) as HTMLDivElement;
        const isOutOfItemFieldsBounding = isOutOfComponent(itemFieldsDiv, event);

        if (props.isOpen && isOutOfComponentBounding && isOutOfListItemsBounding && isOutOfItemFieldsBounding) {
            closeSubMenus();
            props.onClose();
        }
    };

    function isOutOfComponent(elementRef: HTMLDivElement, event: MouseEvent): boolean {
        if (!elementRef) {
            return true;
        }

        const elementBoundingBox = elementRef.getBoundingClientRect();

        return (
            !!elementBoundingBox &&
            (event.pageX < elementBoundingBox.left ||
                event.pageX > elementBoundingBox.left + elementBoundingBox.width ||
                event.pageY < elementBoundingBox.top ||
                event.pageY > elementBoundingBox.top + elementBoundingBox.height)
        );
    }
}
