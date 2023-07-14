import { faChevronLeft } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React, { useCallback } from "react";
import { ReactPropsType } from "../../../helperTypes";
import { COMPONENT_NAME, DocumentPageNavigationComponent } from "./DocumentPageNavigationComponent";

interface PreviousButtonProps
    extends Pick<ReactPropsType<typeof DocumentPageNavigationComponent>, "pageNumber" | "onPageChange"> {}

export const PreviousButton: React.FunctionComponent<PreviousButtonProps> = (props) => {
    const buttonStyles = [`${COMPONENT_NAME}__previous-button`];

    if (props.pageNumber - 1 < 1) {
        buttonStyles.push(`${COMPONENT_NAME}__previous-button--disabled`);
    }

    const onButtonClick = useCallback(() => props.onPageChange(props.pageNumber - 1), [props]);

    return (
        <div className={classNames(buttonStyles)} onClick={onButtonClick}>
            <FontAwesomeIcon size="sm" icon={faChevronLeft} />
        </div>
    );
};
