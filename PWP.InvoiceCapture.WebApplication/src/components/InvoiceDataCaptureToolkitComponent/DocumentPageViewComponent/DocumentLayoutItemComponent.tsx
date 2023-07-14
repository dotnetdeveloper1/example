import classNames from "classnames";
import React from "react";
import { ILayoutItem } from "../store/state";
import "./DocumentLayoutItemComponent.scss";
import { useDocumentLayoutItem } from "./hooks";

interface DocumentLayoutItemComponentProps extends ILayoutItem {
    displayed: boolean;
    onSelectedItemContextMenu(event: MouseEvent): void;
}

export const COMPONENT_NAME = "DocumentLayoutItemComponent";

export const DocumentLayoutItemComponent: React.FunctionComponent<DocumentLayoutItemComponentProps> = (props) => {
    const { computedStyles, componentRef } = useDocumentLayoutItem(props);
    const { displayed, assigned, selected, inFocus } = props;

    const styleClasses = [
        `${COMPONENT_NAME}`,
        selectDocumentLayoutItemStateStyle(displayed, assigned, selected, inFocus)
    ];

    return <div ref={componentRef} className={classNames(styleClasses)} style={computedStyles} />;
};

function selectDocumentLayoutItemStateStyle(
    displayed: boolean,
    hasFieldAssignment: boolean,
    selected: boolean,
    inFocus: boolean | undefined
): string {
    let stateClass = "disabled";

    if (displayed && selected) {
        stateClass = "selected";
    } else if (displayed && hasFieldAssignment) {
        stateClass = "assigned";
    } else if (displayed) {
        stateClass = "enabled";
    }

    if (displayed && inFocus !== undefined) {
        return `${COMPONENT_NAME}--${stateClass}-${inFocus ? "focus-in" : "focus-out"}`;
    }

    return `${COMPONENT_NAME}--${stateClass}`;
}
