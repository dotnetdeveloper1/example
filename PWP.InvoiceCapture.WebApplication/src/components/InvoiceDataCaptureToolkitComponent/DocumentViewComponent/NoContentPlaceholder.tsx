import { faFile } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { useTranslation } from "react-i18next";
import { COMPONENT_NAME } from "./DocumentViewComponent";

export const NoContentPlaceholder: React.FunctionComponent = () => {
    const { t } = useTranslation();

    return (
        <div className={`${COMPONENT_NAME}__no-content`}>
            <FontAwesomeIcon icon={faFile} size="8x" />
            <h5>{`${t("DOCUMENT_CANNOT_BE_DISPLAYED_MESSAGE")}`}</h5>
        </div>
    );
};
