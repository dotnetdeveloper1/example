import { faCheckSquare, faSquare } from "@fortawesome/free-regular-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React from "react";
import "./SwitchButtonComponent.scss";

interface SwitchButtonComponentProps {
    enabled: boolean;
    checked: boolean;
    isVisible: boolean;
    title: string;
    onCheckedChanged: (isChecked: boolean) => void;
}

export const COMPONENT_NAME = "SwitchButtonComponent";

export const SwitchButtonComponent: React.FunctionComponent<SwitchButtonComponentProps> = (props) => {
    const onValueChanged = () => {
        if (props.enabled) {
            props.onCheckedChanged(!props.checked);
        }
    };

    const buttonStyles = [`${COMPONENT_NAME}__field-button`];
    const iconStyles = [`${COMPONENT_NAME}__switch`];

    if (!props.enabled) {
        buttonStyles.push(`${COMPONENT_NAME}__field-button--disabled`);
        iconStyles.push(`${COMPONENT_NAME}__switch--disabled`);
    }

    return (
        <div style={{ visibility: props.isVisible ? "visible" : "hidden" }} className={COMPONENT_NAME}>
            <div className={classNames(buttonStyles)}>
                <div className={classNames(iconStyles)} onClick={onValueChanged}>
                    <div className={`${COMPONENT_NAME}__button-icon`}>
                        <FontAwesomeIcon icon={props.checked ? faCheckSquare : faSquare} />
                    </div>
                    <div className={`${COMPONENT_NAME}__button-text`}>{props.title}</div>
                </div>
            </div>
        </div>
    );
};
