import _ from "lodash";
import { useCallback, useEffect, useLayoutEffect, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { ReactPropsType } from "../../../../helperTypes";
import { BrowserIdentifier } from "./../../../../utils/browserIdentifier";
import { clearTempInvoiceLines, removeNoAssignmentLineItems } from "./../../store/InvoiceDataCaptureToolkitStoreSlice";
import { IBox, IPoint } from "./../../store/state";
import { MultiSelectionPaneChildElementType, MultiSelectionPaneComponent } from "./../MultiSelectionPaneComponent";

interface IMultiSelectionPaneComponentState {
    mouseButtonDown: boolean;
    appendMode: boolean;
    readonly rootElementBounds?: IBox;
    readonly startPoint?: IPoint;
    readonly endPoint?: IPoint;
    readonly selectionBox?: IBox;
    readonly selectedIds: string[];
    readonly startScrollOffset?: IPoint;
}

interface IMultiSelectionPaneHookResult {
    selectionBox: IMultiSelectionPaneComponentState["selectionBox"];
    selectedIds: IMultiSelectionPaneComponentState["selectedIds"];
    selectionRootRef: React.RefObject<HTMLDivElement>;
    onMouseDown(event: React.MouseEvent): void;
    onMouseMove(event: React.MouseEvent): void;
    onMouseUp(event: React.MouseEvent): void;
    isLayoutItemSelected(layoutItem: MultiSelectionPaneChildElementType, selectedIds: string[]): boolean;
}

export function useMultiSelectionPane(
    props: ReactPropsType<typeof MultiSelectionPaneComponent>
): IMultiSelectionPaneHookResult {
    const selectionRootRef = useRef<HTMLDivElement>(null);

    const [state, setState] = useState<IMultiSelectionPaneComponentState>({
        mouseButtonDown: false,
        appendMode: false,
        startPoint: undefined,
        endPoint: undefined,
        selectionBox: undefined,
        selectedIds: [],
        startScrollOffset: undefined
    });

    const dispatch = useDispatch();
    const clearTempInvoices = useCallback((): void => {
        dispatch(clearTempInvoiceLines());
    }, [dispatch]);

    const removeEmptyLineItems = useCallback((): void => {
        dispatch(removeNoAssignmentLineItems());
    }, [dispatch]);

    const onMouseMove = useCallback(onMultiSelectionPaneMouseMove(state, setState), [state, setState]);

    const onMouseUp = useCallback(
        onMultiSelectionPaneMouseUp(removeEmptyLineItems, clearTempInvoices, props, state, setState),
        [props, state, setState]
    );

    const onMouseDown = useCallback(onMultiSelectionPaneMouseDown(props, selectionRootRef, setState), [
        props,
        selectionRootRef,
        setState
    ]);

    const handleGlobalMouseMove: EventListener = useCallback(
        (event): void => {
            onMultiSelectionPaneMouseMove(state, setState)(event as MouseEvent);
        },
        [state, setState]
    );

    const handleGlobalMouseUp: EventListener = useCallback(
        (event): void => {
            onMultiSelectionPaneMouseUp(
                removeEmptyLineItems,
                clearTempInvoiceLines,
                props,
                state,
                setState
            )(event as MouseEvent);
        },
        [removeEmptyLineItems, props, state]
    );

    useEffect(() => {
        window.addEventListener("mousemove", handleGlobalMouseMove);
        window.addEventListener("mouseup", handleGlobalMouseUp);
        return () => {
            window.removeEventListener("mousemove", handleGlobalMouseMove);
            window.removeEventListener("mouseup", handleGlobalMouseUp);
        };
    }, [handleGlobalMouseMove, handleGlobalMouseUp]);

    useLayoutEffect(() => {
        updateSelection(props, state, setState);
    }, [props, state, setState]);

    return {
        selectionBox: state.selectionBox,
        selectedIds: state.selectedIds,
        selectionRootRef: selectionRootRef,
        onMouseDown: onMouseDown,
        onMouseMove: onMouseMove,
        onMouseUp: onMouseUp,
        isLayoutItemSelected: isLayoutItemSelected
    };
}

function updateSelection(
    props: ReactPropsType<typeof MultiSelectionPaneComponent>,
    state: IMultiSelectionPaneComponentState,
    setState: React.Dispatch<React.SetStateAction<IMultiSelectionPaneComponentState>>
): void {
    const {
        rootElementBounds,
        startPoint,
        endPoint,
        selectionBox,
        mouseButtonDown,
        appendMode,
        startScrollOffset
    } = state;

    if (mouseButtonDown && rootElementBounds && startPoint) {
        const nextSelectionBox = getNextSelectionBox(
            rootElementBounds,
            startPoint,
            endPoint,
            startScrollOffset,
            props.containerScrollOffset
        );
        if (
            !selectionBox ||
            selectionBox.width !== nextSelectionBox.width ||
            selectionBox.height !== nextSelectionBox.height ||
            selectionBox.left !== nextSelectionBox.left ||
            selectionBox.top !== nextSelectionBox.top
        ) {
            const nextSelectedItemIds = getNextSelectedItemIds(
                props.selectedIds,
                props.children,
                nextSelectionBox,
                appendMode
            );

            setState({
                ...state,
                selectionBox: nextSelectionBox,
                selectedIds: nextSelectedItemIds
            });
        }
    }
}

function onMultiSelectionPaneMouseDown(
    props: ReactPropsType<typeof MultiSelectionPaneComponent>,
    selectionRootElement: React.RefObject<HTMLDivElement>,
    setState: React.Dispatch<React.SetStateAction<IMultiSelectionPaneComponentState>>
): (event: React.MouseEvent<Element, MouseEvent>) => void {
    return (event: React.MouseEvent) => {
        // NOTE handle left and right mouse button
        if (event.button !== 0 && event.button !== 2) {
            return;
        }
        // NOTE if right mouse button pressed, but selection has already formed, skip selection handling logic
        if (event.button === 2 && props.selectedIds && props.selectedIds.length > 1) {
            return;
        }

        if (!props.showCompareBoxes) {
            return;
        }
        const rootBoundingRectangle =
            selectionRootElement && selectionRootElement.current
                ? selectionRootElement.current.getBoundingClientRect()
                : undefined;

        setState({
            mouseButtonDown: true,
            appendMode: event.ctrlKey,
            rootElementBounds: rootBoundingRectangle && {
                left: rootBoundingRectangle.x || rootBoundingRectangle.left,
                top: rootBoundingRectangle.y || rootBoundingRectangle.top,
                width: rootBoundingRectangle.width,
                height: rootBoundingRectangle.height
            },
            startPoint: { x: event.pageX, y: event.pageY },
            endPoint: undefined,
            selectionBox: undefined,
            selectedIds: [],
            startScrollOffset: props.containerScrollOffset
        });
    };
}

function onMultiSelectionPaneMouseMove(
    state: IMultiSelectionPaneComponentState,
    setState: React.Dispatch<React.SetStateAction<IMultiSelectionPaneComponentState>>
): (event: MouseEvent | React.MouseEvent<Element, MouseEvent>) => void {
    return (event: React.MouseEvent | MouseEvent) => {
        if (event.button !== 0 || event.buttons !== 1 || !state.mouseButtonDown) {
            return;
        }

        setState({
            ...state,
            endPoint: { x: event.pageX, y: event.pageY },
            appendMode: event.ctrlKey
        });
    };
}

function onMultiSelectionPaneMouseUp(
    removeEmptyInvoiceLines: () => void,
    clearTempInvoiceLines: () => void,
    props: ReactPropsType<typeof MultiSelectionPaneComponent>,
    state: IMultiSelectionPaneComponentState,
    setState: React.Dispatch<React.SetStateAction<IMultiSelectionPaneComponentState>>
): (event: MouseEvent | React.MouseEvent<Element, MouseEvent>) => void {
    return (event: React.MouseEvent | MouseEvent) => {
        const isLeftMouseButton = event.button === 0;
        const isRightMouseButton = event.button === 2;

        if ((!isLeftMouseButton && !isRightMouseButton) || !state.mouseButtonDown) {
            return;
        }

        if (!props.showCompareBoxes) {
            return;
        }

        if (isLeftMouseButton) {
            event.stopPropagation();
        }

        const { selectedIds } = state;
        props.onLayoutItemsSelect(selectedIds);

        if (props.tableSelectionMode && isLeftMouseButton) {
            let selectionBox: IBox | undefined;
            if (selectedIds && selectedIds.length > 0) {
                selectionBox = state.selectionBox;
            } else {
                removeEmptyInvoiceLines();
            }
            clearTempInvoiceLines();

            props.onTableSelected(selectionBox);
        }

        // In safari, it seems like the focus is given to the context menu when right-clicking, so the context menu receives the mouseup event rather than the target element.
        if (BrowserIdentifier.IsSafari() && isRightMouseButton) {
            const selectedLayouts = props.children?.filter((layout) => isLayoutItemSelected(layout, selectedIds));
            if (selectedLayouts && selectedLayouts.length) {
                selectedLayouts[0].props.onSelectedItemContextMenu(event as MouseEvent);
            }
        }

        setState({
            mouseButtonDown: false,
            appendMode: false,
            rootElementBounds: undefined,
            startPoint: undefined,
            endPoint: undefined,
            selectionBox: undefined,
            selectedIds: [],
            startScrollOffset: undefined
        });
    };
}

function getNextSelectedItemIds(
    preselectedIds: string[],
    componentChildren: ReactPropsType<typeof MultiSelectionPaneComponent>["children"],
    nextSelectionBox: IBox,
    appendMode: boolean
): string[] {
    const selectedLayoutItems =
        componentChildren && componentChildren.length > 0
            ? componentChildren.filter(isLayoutItemCollideWithSelectionBox(nextSelectionBox))
            : [];

    if (selectedLayoutItems && selectedLayoutItems.length > 0) {
        return _(selectedLayoutItems.map((item) => item.props.id))
            .push(...(appendMode ? preselectedIds : []))
            .sortedUniq()
            .value();
    }

    return appendMode ? preselectedIds : [];
}

function getNextSelectionBox(
    rootElementBounds: IBox,
    startPoint: IPoint,
    endPoint: IPoint | undefined,
    startScrollOffset: IPoint | undefined,
    currentScrollOffset: IPoint
): IBox {
    const scrollTransition: IPoint =
        startScrollOffset &&
        (startScrollOffset.x !== currentScrollOffset.x || startScrollOffset.y !== currentScrollOffset.y)
            ? { x: currentScrollOffset.x - startScrollOffset.x, y: currentScrollOffset.y - startScrollOffset.y }
            : { x: 0, y: 0 };

    const startX = startPoint.x;
    const endX = endPoint
        ? endPoint.x + scrollTransition.x < rootElementBounds.left
            ? rootElementBounds.left
            : endPoint.x + scrollTransition.x > rootElementBounds.left + rootElementBounds.width
            ? rootElementBounds.left + rootElementBounds.width
            : endPoint.x + scrollTransition.x
        : startPoint.x + 1;
    const startY = startPoint.y;
    const endY = endPoint
        ? endPoint.y + scrollTransition.y < rootElementBounds.top
            ? rootElementBounds.top
            : endPoint.y + scrollTransition.y > rootElementBounds.top + rootElementBounds.height
            ? rootElementBounds.top + rootElementBounds.height
            : endPoint.y + scrollTransition.y
        : startPoint.y + 1;

    const nextLeft = Math.min(startX, endX) - rootElementBounds.left;
    const nextTop = Math.min(startY, endY) - rootElementBounds.top;
    const nextWidth = Math.max(startX, endX) - Math.min(startX, endX);
    const nextHeight = Math.max(startY, endY) - Math.min(startY, endY);
    return {
        top: nextTop,
        left: nextLeft,
        width: nextWidth,
        height: nextHeight
    };
}

function isLayoutItemCollideWithSelectionBox(
    nextSelectionBox: IBox
): (layoutItem: MultiSelectionPaneChildElementType) => boolean {
    return (layoutItem) => {
        const { topLeft, bottomRight } = layoutItem.props;

        const itemTop = topLeft.y;
        const itemLeft = topLeft.x;
        const itemWidth = bottomRight.x - topLeft.x;
        const itemHeight = bottomRight.y - topLeft.y;

        return (
            nextSelectionBox &&
            nextSelectionBox.left <= itemLeft + itemWidth &&
            nextSelectionBox.left + nextSelectionBox.width >= itemLeft &&
            nextSelectionBox.top <= itemTop + itemHeight &&
            nextSelectionBox.top + nextSelectionBox.height >= itemTop
        );
    };
}

function isLayoutItemSelected(layoutItem: MultiSelectionPaneChildElementType, selectedIds: string[]): boolean {
    return layoutItem.props.selected || selectedIds.find((id) => id === layoutItem.props.id) !== undefined;
}
