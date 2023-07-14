import { useCallback, useEffect, useRef } from "react";
import { ReactPropsType } from "../../../../helperTypes";
import { DocumentLayoutItemComponent } from "../DocumentLayoutItemComponent";
import { BrowserIdentifier } from "./../../../../utils/browserIdentifier";

interface IDocumentLayoutItemHookResult {
    computedStyles: React.CSSProperties;
    componentRef: React.RefObject<HTMLDivElement>;
}

export function useDocumentLayoutItem(
    props: ReactPropsType<typeof DocumentLayoutItemComponent>
): IDocumentLayoutItemHookResult {
    const { topLeft, bottomRight } = props;

    const componentRef = useRef<HTMLDivElement>(null);

    const computedStyles = {
        top: topLeft.y,
        left: topLeft.x,
        width: bottomRight.x - topLeft.x,
        height: bottomRight.y - topLeft.y
    };

    const handleContextMenu = useCallback(
        (event: Event) => {
            // In safari, it seems like the focus is given to the context menu when right-clicking, so the context menu receives the mouseup event rather than the target element.
            if (BrowserIdentifier.IsSafari()) {
                event.preventDefault();
            }
            if (props.selected && props.displayed) {
                const mouseEvent = event as MouseEvent;
                if (mouseEvent) {
                    event.preventDefault();
                    props.onSelectedItemContextMenu(mouseEvent);
                }
            }
        },
        [props]
    );

    useEffect(() => {
        componentRef?.current?.addEventListener("contextmenu", handleContextMenu);
        return () => {
            componentRef?.current?.removeEventListener("contextmenu", handleContextMenu);
        };
    }, [handleContextMenu]);

    return {
        computedStyles: computedStyles,
        componentRef: componentRef
    };
}
