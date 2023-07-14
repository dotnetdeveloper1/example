import classNames from "classnames";
import React from "react";
import "./ToggleComponent.scss";

interface ToggleProps {
    isToggled: boolean;
    onToggled(value: any): void;
    title: string;
    enabled: boolean;
}

export const COMPONENT_NAME = "ToggleComponent";

export const Toggle: React.FunctionComponent<ToggleProps> = (props) => {
    const inputStyles = [`${COMPONENT_NAME}__button`];
    const labelStyles = [`${COMPONENT_NAME}__label`];

    if (!props.enabled) {
        inputStyles.push(`${COMPONENT_NAME}__button--disabled`);
        labelStyles.push(`${COMPONENT_NAME}__label--disabled`);
    }

    return (
        <div className={COMPONENT_NAME}>
            <input
                id="toggle-button"
                type="checkbox"
                onChange={(e) => props.onToggled(e.target.checked)}
                checked={props.isToggled}
                className={classNames(inputStyles)}
            />

            <label htmlFor="toggle-button" className={classNames(labelStyles)}>
                {props.title}
            </label>
        </div>
    );
};
