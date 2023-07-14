import { IconDefinition } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React from "react";
import "./AccordionComponent.scss";

interface AccordionComponentProps {
    headerText: string;
    expanded?: boolean;
    buttonText?: string;
    buttonIcon?: IconDefinition;
    onButtonClick?: () => void;
    buttonDisabled?: boolean;
}

const COMPONENT_NAME = "AccordionComponent";

export const AccordionComponent: React.FunctionComponent<AccordionComponentProps> = (props) => {
    const buttonStyles = [`${COMPONENT_NAME}__header__button`];
    if (props.buttonDisabled) {
        buttonStyles.push(`${COMPONENT_NAME}__header__button--disabled`);
    }

    return (
        <div className={COMPONENT_NAME}>
            <input type="checkbox" id={props.headerText} defaultChecked={props.expanded} />
            <div className={`${COMPONENT_NAME}__header`}>
                {/* <FontAwesomeIcon icon={faChevronUp} className={`${COMPONENT_NAME}__header__icon`} /> */}
                <label htmlFor={props.headerText} className={`${COMPONENT_NAME}__header__label`}>
                    {props.headerText}
                </label>

                {props.buttonText && props.onButtonClick && (
                    <div className={classNames(buttonStyles)} onClick={props.onButtonClick}>
                        {props.buttonIcon && <FontAwesomeIcon icon={props.buttonIcon} size="1x" />}
                        {props.buttonText}
                    </div>
                )}
            </div>
            <div className={`${COMPONENT_NAME}__content`}>{props.children}</div>
        </div>
    );
};
