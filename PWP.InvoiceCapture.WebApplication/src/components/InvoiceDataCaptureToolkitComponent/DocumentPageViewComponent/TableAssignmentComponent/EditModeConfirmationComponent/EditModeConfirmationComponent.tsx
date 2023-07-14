import React from "react";
import { useTranslation } from "react-i18next";
import { Modal, ModalFooter } from "reactstrap";
import "./EditModeConfirmationComponent.scss";

interface EditModeConfirmationComponentProps {
    isOpen: boolean;
    onConfirm(): void;
    onCancel(): void;
}

export const COMPONENT_NAME = "EditModeConfirmationComponent";

export const EditModeConfirmationComponent: React.FunctionComponent<EditModeConfirmationComponentProps> = (props) => {
    const { t } = useTranslation();

    return (
        <>
            {props.isOpen && (
                <Modal
                    className={`${COMPONENT_NAME}`}
                    contentClassName={`${COMPONENT_NAME}__content`}
                    centered={true}
                    isOpen={props.isOpen}
                    toggle={props.onCancel}>
                    <div className={`${COMPONENT_NAME}__header`}>{t("MODAL_MESSAGE->SWITCH_TO_EDIT_MODE")}</div>
                    <div className={`${COMPONENT_NAME}__body`}>
                        <p>{t("MODAL_MESSAGE->SWITCH_TO_EDIT_MODE_WARNING")}</p>
                        <p>{t("MODAL_MESSAGE->CONTINUE_QUESTION")}</p>
                    </div>
                    <ModalFooter className={`${COMPONENT_NAME}__footer`}>
                        <div className={`${COMPONENT_NAME}__cancel`} onClick={props.onCancel}>
                            {t("BUTTON_TITLE->NO")}
                        </div>
                        <div className={`${COMPONENT_NAME}__confirm`} onClick={props.onConfirm}>
                            {t("BUTTON_TITLE->YES")}
                        </div>
                    </ModalFooter>
                </Modal>
            )}
        </>
    );
};
