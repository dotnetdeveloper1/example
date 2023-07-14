import { faEye } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React, { useCallback } from "react";
import { useTranslation } from "react-i18next";
import { COMPONENT_NAME } from "./DocumentControlPanelComponent";

interface ToggleCompareBoxesButtonProps {
    compareBoxes: boolean;
    onToggleCompareBoxes(): void;
}

export const ToggleCompareBoxesButton: React.FunctionComponent<ToggleCompareBoxesButtonProps> = (props) => {
    const { t } = useTranslation();

    const styleClasses = [
        `${COMPONENT_NAME}__show-annotations`,
        props.compareBoxes
            ? `${COMPONENT_NAME}__show-annotations--enabled`
            : `${COMPONENT_NAME}__show-annotations--disabled`
    ];

    const onButtonClick = useCallback(() => {
        props.onToggleCompareBoxes();
    }, [props]);

    return (
        <div className={classNames(styleClasses)} onClick={onButtonClick}>
            <FontAwesomeIcon icon={faEye} size="lg" />
            {!props.compareBoxes ? t("BUTTON_TITLE->SHOW_COMPARE_BOXES") : t("BUTTON_TITLE->HIDE_COMPARE_BOXES")}
        </div>
    );
};
