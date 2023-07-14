import { useCallback, useEffect, useLayoutEffect, useRef, useState } from "react";
import { BrowserIdentifier } from "../../../../../../utils/browserIdentifier";
import { findAvailableScreenMenuTopPosition } from "../../../../store/reducers/helpers";
import { getLineItemFieldNameFromLineItemFieldType } from "../../../../store/reducers/helpers/getLineItemFieldNameFromLineItemFieldType";
import { isOutOfComponent } from "../../../../store/reducers/helpers/isOutOfComponent";
import { tempLineItemsSelector, useToolkitSelector } from "../../../../store/selectors";
import { LineItemsFieldTypes } from "../../../../store/state";

import { ITableMenuState } from "../../TableAssignmentComponent";

interface ITableMenuComponentHookResult {
    isEdge: boolean;
    isAssignedFieldType: (fieldType: LineItemsFieldTypes) => boolean;
    tableMenuStyles: React.CSSProperties;
    componentRef: React.RefObject<HTMLDivElement>;
}

export function useTableMenuComponent(
    onTableMenuClosed: () => void,
    tableMenuState: ITableMenuState
): ITableMenuComponentHookResult {
    const tempLineItems = useToolkitSelector(tempLineItemsSelector);
    const componentRef = useRef<HTMLDivElement>(null);
    const [menuStyles, setMenuStyles] = useState<React.CSSProperties>({
        top: tableMenuState.top,
        left: tableMenuState.left
    });

    const isAssignedFieldType = useCallback(
        (fieldType: LineItemsFieldTypes) => {
            if (!tempLineItems || tempLineItems.length === 0) {
                return false;
            }

            const fieldName = getLineItemFieldNameFromLineItemFieldType(fieldType);
            const lineItemFieldValue = (tempLineItems[0] as any)[fieldName!];

            return lineItemFieldValue && lineItemFieldValue !== "";
        },
        [tempLineItems]
    );

    const handleMouseUp: EventListener = useCallback(
        (event): void => {
            const mouseEvent = event as MouseEvent;
            if (mouseEvent && isOutOfComponent(componentRef?.current, mouseEvent)) {
                onTableMenuClosed();
            }
        },
        [componentRef, onTableMenuClosed]
    );

    useEffect(() => {
        window.addEventListener("mouseup", handleMouseUp);
        return () => {
            window.removeEventListener("mouseup", handleMouseUp);
        };
    }, [handleMouseUp]);

    useLayoutEffect(() => {
        if (!tableMenuState.isOpen || !componentRef) {
            return;
        }
        const globalRootElement = document.getElementById("root");
        const rootBoundingBox = globalRootElement && globalRootElement.getBoundingClientRect();
        const componentBoundingBox = componentRef.current && componentRef.current.getBoundingClientRect();

        const top = findAvailableScreenMenuTopPosition(rootBoundingBox, componentBoundingBox, tableMenuState.top);

        setMenuStyles({
            top: top,
            left: tableMenuState.left
        });
    }, [tableMenuState, setMenuStyles]);

    return {
        isEdge: BrowserIdentifier.IsEdge(),
        isAssignedFieldType: isAssignedFieldType,
        tableMenuStyles: menuStyles,
        componentRef: componentRef
    };
}
