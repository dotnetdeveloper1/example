import { faDownload } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React from "react";
import { useTranslation } from "react-i18next";
import { TranslationUtils } from "../../../utils/translationUtils";
import { Dropdownlist } from "../../common/DropdownlistComponent/DropdownlistComponent";
import { InvoiceStatus } from "../store/state";
import { useToolkitHeader } from "./hooks/useToolkitHeader";
import logo from "./company-logo.svg";
import "./ToolkitHeaderComponent.scss";

interface ToolkitHeaderComponentProps {
    invoiceId?: number;
    invoiceStatus?: InvoiceStatus;
    title: string;
    invoiceFileName: string;
}

export const COMPONENT_NAME = "ToolkitHeaderComponent";

export const ToolkitHeaderComponent: React.FunctionComponent<ToolkitHeaderComponentProps> = (props) => {
    const { t } = useTranslation();
    const {
        downloadingFile,
        onDownloadButtonPress,
        languageListState,
        toggleLanguageDropdownListOpen,
        languageDropdownListValueChanged
    } = useToolkitHeader(props);

    const buttonStyles = [`${COMPONENT_NAME}__download`];

    if (downloadingFile) {
        buttonStyles.push(`${COMPONENT_NAME}__download--disabled`);
    }

    return (
        <div className={`${COMPONENT_NAME}`}>
            <div className={`${COMPONENT_NAME}__title-container`}>
                <img alt={t("PWP_LOGO")} srcSet={`${logo}`} className={`${COMPONENT_NAME}__company-logo`} />
                <div className={`${COMPONENT_NAME}__title-label`}>{props.title}</div>
                {props.invoiceStatus !== undefined && props.invoiceStatus !== InvoiceStatus.Undefined && (
                    <div className={`${COMPONENT_NAME}__title-badge`}>
                        {t(TranslationUtils.getInvoiceStatusLocalizationKeyByEnum(props.invoiceStatus))}
                    </div>
                )}
            </div>
            <div className={`${COMPONENT_NAME}__buttons`}>
                {props.invoiceId && (
                    <>
                        <Dropdownlist
                            widthPx={183}
                            onToggled={toggleLanguageDropdownListOpen}
                            onValueSelected={languageDropdownListValueChanged}
                            state={languageListState}
                            noSelectionText={t("DROPDOWNLIST->SELECT_LANGUAGE")}
                        />
                        <div className={classNames(buttonStyles)} onClick={(e) => onDownloadButtonPress(e)}>
                            <FontAwesomeIcon icon={faDownload} size="lg" />
                            {t("BUTTON_TITLE->DOWNLOAD_FILE")}
                        </div>
                    </>
                )}
            </div>
        </div>
    );
};
