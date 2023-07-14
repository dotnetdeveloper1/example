import { faBorderAll } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { useTranslation } from "react-i18next";
import { EditModeConfirmationComponent } from "../EditModeConfirmationComponent/EditModeConfirmationComponent";
import { useEditModeConfirmation } from "../EditModeConfirmationComponent/hooks/useEditModeConfirmation";
import { useSwitchToEditModeComponent } from "./hooks/useSwitchToEditModeComponent";
import "./SwitchToEditModeComponent.scss";

export const COMPONENT_NAME = "SwitchToEditModeComponent";

interface SwitchToEditModeComponentProps {
    beforeSwitchToEditMode: () => void;
    askConfirmation: boolean;
    isMenuStyles: boolean;
}

export const SwitchToEditModeComponent: React.FunctionComponent<SwitchToEditModeComponentProps> = (props) => {
    const { t } = useTranslation();
    const { switchToEditMode, isVisible } = useSwitchToEditModeComponent();

    const toEditMode = () => {
        props.beforeSwitchToEditMode();
        switchToEditMode();
    };

    const { isEditModeModalOpen, onConfirm, onCancel, onOpen } = useEditModeConfirmation(toEditMode);

    const componentStyle = props.isMenuStyles
        ? `${COMPONENT_NAME}__field-menu-item-button`
        : `${COMPONENT_NAME}__field-button`;

    const buttonStyle = props.isMenuStyles
        ? `${COMPONENT_NAME}__switch-to-edit-mode-menu`
        : `${COMPONENT_NAME}__switch-to-edit-mode`;

    return (
        <div className={COMPONENT_NAME} style={{ visibility: isVisible ? "visible" : "collapse" }}>
            {isVisible && (
                <>
                    <EditModeConfirmationComponent
                        isOpen={isEditModeModalOpen}
                        onCancel={onCancel}
                        onConfirm={onConfirm}
                    />
                    <div className={componentStyle} onClick={props.askConfirmation ? onOpen : toEditMode}>
                        <div className={buttonStyle}>
                            <div className={`${COMPONENT_NAME}__button-icon`}>
                                <FontAwesomeIcon icon={faBorderAll} />
                            </div>
                            <div className={`${COMPONENT_NAME}__button-text`}>{t("SWITCH_TO_EDIT_MODE")}</div>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};
