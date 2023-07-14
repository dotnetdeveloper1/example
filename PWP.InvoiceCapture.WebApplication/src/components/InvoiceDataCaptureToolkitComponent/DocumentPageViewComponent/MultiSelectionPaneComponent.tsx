import React from "react";
import { ReactPropsType } from "../../../helperTypes";
import { IBox, IPoint } from "../store/state";
import { DocumentLayoutItemComponent } from "./DocumentLayoutItemComponent";
import { useMultiSelectionPane } from "./hooks";
import "./MultiSelectionPaneComponent.scss";

export type MultiSelectionPaneChildElementType = React.ReactElement<ReactPropsType<typeof DocumentLayoutItemComponent>>;

interface MultiSelectionPaneComponentProps {
    selectedIds: string[];
    showCompareBoxes: boolean;
    children?: MultiSelectionPaneChildElementType[];
    onLayoutItemsSelect(nextSelectedIds: string[]): void;
    onTableSelected(table?: IBox): void;
    containerScrollOffset: IPoint;
    tableSelectionMode: boolean;
}

export const COMPONENT_NAME = "MultiSelectionPaneComponent";

export const MultiSelectionPaneComponent: React.FunctionComponent<MultiSelectionPaneComponentProps> = (props) => {
    const {
        onMouseDown,
        onMouseMove,
        onMouseUp,
        selectionBox,
        selectedIds,
        selectionRootRef,
        isLayoutItemSelected
    } = useMultiSelectionPane(props);

    const selectionBoxStyles = selectionBox && {
        top: selectionBox.top,
        left: selectionBox.left,
        width: selectionBox.width,
        height: selectionBox.height
    };

    return (
        <div
            className={`${COMPONENT_NAME}`}
            ref={selectionRootRef}
            onMouseDown={onMouseDown}
            onMouseMove={onMouseMove}
            onMouseUp={onMouseUp}>
            {React.Children.map(
                props.children,
                (layoutItem) =>
                    layoutItem &&
                    React.cloneElement(layoutItem, {
                        selected: isLayoutItemSelected(layoutItem, selectedIds)
                    })
            )}
            {selectionBox && <div className={`${COMPONENT_NAME}__selection-box`} style={selectionBoxStyles} />}
        </div>
    );
};
