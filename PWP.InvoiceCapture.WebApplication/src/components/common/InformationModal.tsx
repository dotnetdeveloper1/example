import classNames from "classnames";
import React from "react";
import { useCallback } from "react";
import { useTranslation } from "react-i18next";
import { Modal, ModalFooter } from "reactstrap";
import "./InformationModal.scss";

interface InformationModalProps {
    isOpen: boolean;
    title: string;
    description: string;
    modalClassName?: string;
    onClose(): void;
}

export const COMPONENT_NAME = "InformationModal";

export const InformationModal: React.FunctionComponent<InformationModalProps> = (props) => {
    const { t } = useTranslation();

    const onClose = useCallback(() => props.onClose(), [props]);

    return (
        <Modal
            className={classNames(`${COMPONENT_NAME}`, props.modalClassName)}
            contentClassName={`${COMPONENT_NAME}__content`}
            centered={true}
            isOpen={props.isOpen}>
            <div className={`${COMPONENT_NAME}__header`}>{props.title}</div>
            <div className={`${COMPONENT_NAME}__body`}>{props.description}</div>
            <ModalFooter className={`${COMPONENT_NAME}__footer`}>
                <div className={`${COMPONENT_NAME}__cancel`} onClick={onClose}>
                    {t("BUTTON_TITLE->CANCEL")}
                </div>
            </ModalFooter>
        </Modal>
    );
};
