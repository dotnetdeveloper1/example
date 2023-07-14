import classNames from "classnames";
import React from "react";
import { useTranslation } from "react-i18next";
import "./ToolkitControlPanelComponent.scss";

interface ToolkitControlPanelComponentProps {
    isSaveEnabled: boolean;
    isSubmitEnabled: boolean;
    onSaveProcessingResult(event: React.MouseEvent): void;
    onSubmitProcessingResult(event: React.MouseEvent): void;
    onClose(): void;
}

export const COMPONENT_NAME = "ToolkitControlPanelComponent";

export const ToolkitControlPanelComponent: React.FunctionComponent<ToolkitControlPanelComponentProps> = (props) => {
    const saveButtonStyles = [`${COMPONENT_NAME}__save`];
    const submitButtonStyles = [`${COMPONENT_NAME}__submit`];
    const { t } = useTranslation();
    const { isSaveEnabled, isSubmitEnabled, onSaveProcessingResult, onSubmitProcessingResult, onClose } = props;

    if (!isSaveEnabled) {
        saveButtonStyles.push(`${COMPONENT_NAME}__save--disabled`);
    }
    if (!isSubmitEnabled) {
        submitButtonStyles.push(`${COMPONENT_NAME}__submit--disabled`);
    }

    return (
        <div className={`${COMPONENT_NAME}`}>
            <div className={`${COMPONENT_NAME}__buttons`}>
                <div className={classNames(saveButtonStyles)} onClick={onSaveProcessingResult}>
                    {t("BUTTON_TITLE->SAVE")}
                </div>
                <div className={classNames(submitButtonStyles)} onClick={onSubmitProcessingResult}>
                    {t("BUTTON_TITLE->SUBMIT")}
                </div>
                <div className={`${COMPONENT_NAME}__close`} onClick={() => onClose()}>
                    {t("BUTTON_TITLE->CLOSE")}
                </div>
            </div>
        </div>
    );
};
