import { useCallback, useLayoutEffect, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { ReactPropsType } from "../../../../../helperTypes";
import { assignInvoiceItems } from "../../../store/InvoiceDataCaptureToolkitStoreSlice";
import { findAvailableScreenMenuTopPosition } from "../../../store/reducers/helpers";
import { invoiceItemsSelector, useToolkitSelector } from "../../../store/selectors";
import { ILineItem } from "../../../store/state";
import { ITEMS_LIST_PARENT_FIELD_ID } from "../../FieldSelectionList";
import { IItemsListMenuComponentState } from "../../hooks/useFieldAssignmentMenuControl";
import { ItemsListMenuComponent } from "../ItemsListMenuComponent";

interface IListItemsMenuHookResult {
    onOpenListItems: (event: React.MouseEvent) => void;
    isItemsListMenuOpened: boolean;
    isItemHighlighted: (orderNumber: number, fieldGroupOrderNumber: number) => boolean;
    componentRef: React.RefObject<HTMLDivElement>;
    itemsListState: IItemsListMenuComponentState;
    invoiceItemsState: ILineItem[];
    onAddInvoiceListItem: (event: React.MouseEvent) => void;
}

export function useListItemsMenu(props: ReactPropsType<typeof ItemsListMenuComponent>): IListItemsMenuHookResult {
    const componentRef = useRef<HTMLDivElement>(null);

    const dispatch = useDispatch();
    const invoiceItemsState = useToolkitSelector(invoiceItemsSelector);

    const onAddInvoiceListItem = useCallback(
        (event: React.MouseEvent) => {
            const itemsCount = invoiceItemsState?.length ?? 0;
            dispatch(assignInvoiceItems({ orderNumber: itemsCount + 1 }));
        },
        [dispatch, invoiceItemsState]
    );

    const [itemsListState, setItemsListState] = useState<IItemsListMenuComponentState>({
        isOpen: props.isOpen,
        top: 0,
        left: 0
    });

    useLayoutEffect(() => {
        if (!props.isOpen) {
            return;
        }
        const globalRootElement = document.getElementById("root");
        const rootBoundingBox = globalRootElement && globalRootElement.getBoundingClientRect();
        const listItemsDiv = document.getElementById(ITEMS_LIST_PARENT_FIELD_ID);
        const listItemsDivBox = listItemsDiv && listItemsDiv.getBoundingClientRect();
        const componentBoundingBox = componentRef.current && componentRef.current.getBoundingClientRect();

        const top = findAvailableScreenMenuTopPosition(
            rootBoundingBox,
            componentBoundingBox,
            listItemsDivBox?.top ?? 0
        );

        const itemsState = {
            isOpen: props.isOpen,
            left: listItemsDivBox?.right ?? 0,
            top: top
        };
        setItemsListState(itemsState);
    }, [props, setItemsListState, componentRef]);

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

    const isItemHighlighted = (orderNumber: number, fieldGroupOrderNumber: number) => {
        return orderNumber === fieldGroupOrderNumber;
    };

    return {
        isItemsListMenuOpened: itemsListState.isOpen,
        isItemHighlighted: isItemHighlighted,
        componentRef: componentRef,
        itemsListState: itemsListState,
        onOpenListItems: onOpenListItems,
        onAddInvoiceListItem: onAddInvoiceListItem,
        invoiceItemsState: invoiceItemsState ?? []
    };
}
