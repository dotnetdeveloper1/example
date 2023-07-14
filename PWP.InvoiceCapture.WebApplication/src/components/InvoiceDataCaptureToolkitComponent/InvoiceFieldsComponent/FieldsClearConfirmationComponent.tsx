import React from "react";
import { useTranslation } from "react-i18next";
import { Modal, ModalFooter } from "reactstrap";
import "./FieldsClearConfirmationComponent.scss";

interface FieldsClearConfirmationComponentProps {
    isOpen: boolean;
    onConfirm(): void;
    onCancel(): void;
}

export const COMPONENT_NAME = "FieldsClearConfirmationComponent";

export const FieldsClearConfirmationComponent: React.FunctionComponent<FieldsClearConfirmationComponentProps> = (
    props
) => {
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
                    <div className={`${COMPONENT_NAME}__header`}>{t("MODAL_MESSAGE->CLEAR_ALL_FIELDS")}</div>
                    <div className={`${COMPONENT_NAME}__body`}>
                        <p>{t("MODAL_MESSAGE->CLEAR_INVOICE_FIELDS_WARNING")}</p>
                        <p>{t("MODAL_MESSAGE->CONFIRMATION_MESSAGE")}</p>
                    </div>
                    <ModalFooter className={`${COMPONENT_NAME}__footer`}>
                        <div className={`${COMPONENT_NAME}__cancel`} onClick={props.onCancel}>
                            {t("BUTTON_TITLE->CANCEL")}
                        </div>
                        <div className={`${COMPONENT_NAME}__confirm`} onClick={props.onConfirm}>
                            {t("BUTTON_TITLE->CONFIRM")}
                        </div>
                    </ModalFooter>
                </Modal>
            )}
        </>
    );
};
