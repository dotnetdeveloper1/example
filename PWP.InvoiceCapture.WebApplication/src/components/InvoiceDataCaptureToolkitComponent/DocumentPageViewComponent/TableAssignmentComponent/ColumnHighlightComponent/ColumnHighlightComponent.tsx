import classNames from "classnames";
import React from "react";
import { IPoint } from "../../../store/state";
import "./ColumnHighlightComponent.scss";

interface ColumnHighLightComponentProps {
    topLeft: IPoint;
    bottomRight: IPoint;
    title: string;
    isAssigned: boolean;
}

export const COMPONENT_NAME = "ColumnHighlightComponent";

export const ColumnHighlightComponent: React.FunctionComponent<ColumnHighLightComponentProps> = (props) => {
    const styles = [COMPONENT_NAME];
    if (!props.isAssigned) {
        styles.push(`${COMPONENT_NAME}--selected`);
    }

    return (
        <div
            className={classNames(styles)}
            style={{
                top: props.topLeft.y,
                left: props.topLeft.x,
                height: props.bottomRight.y - props.topLeft.y,
                width: props.bottomRight.x - props.topLeft.x
            }}>
            {props.isAssigned && (
                <div
                    className={`${COMPONENT_NAME}__header`}
                    style={{
                        top: -22,
                        left: -3,
                        height: 22,
                        width: props.bottomRight.x - props.topLeft.x
                    }}>
                    {props.title}
                </div>
            )}
        </div>
    );
};
