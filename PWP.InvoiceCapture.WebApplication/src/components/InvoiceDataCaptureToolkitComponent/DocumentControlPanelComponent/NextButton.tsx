import { faChevronRight } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React, { useCallback } from "react";
import { ReactPropsType } from "../../../helperTypes";
import { COMPONENT_NAME, DocumentPageNavigationComponent } from "./DocumentPageNavigationComponent";

interface NextButtonProps extends ReactPropsType<typeof DocumentPageNavigationComponent> {}

export const NextButton: React.FunctionComponent<NextButtonProps> = (props) => {
    const buttonStyles = [`${COMPONENT_NAME}__next-button`];

    if (props.pageNumber + 1 > props.totalPages) {
        buttonStyles.push(`${COMPONENT_NAME}__next-button--disabled`);
    }

    const onButtonClick = useCallback(() => props.onPageChange(props.pageNumber + 1), [props]);

    return (
        <div className={classNames(buttonStyles)} onClick={onButtonClick}>
            <FontAwesomeIcon size="sm" icon={faChevronRight} />
        </div>
    );
};
