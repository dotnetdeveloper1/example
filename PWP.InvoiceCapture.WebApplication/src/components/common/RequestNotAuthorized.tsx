import { faInfo } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { useTranslation } from "react-i18next";

export const RequestNotAuthorized: React.FunctionComponent<{}> = () => {
    const { t } = useTranslation();

    return (
        <div className="d-flex flex-row flex-fill justify-content-center align-items-center h-100 w-100">
            <div className="row">
                <div className="d-flex flex-row justify-content-start align-items-center border border-primary-accent">
                    <div className="d-flex h-100 p-5 border-right border-primary-accent">
                        <FontAwesomeIcon icon={faInfo} size="8x" />
                    </div>
                    <div className="d-flex flex-column align-items-center display-4 p-3">
                        <p className="text-primary-accent">{t("401_TITLE")}</p>
                        <div className="h4 font-weight-normal text-primary-accent">{t("401_MESSAGE")}</div>
                    </div>
                </div>
            </div>
        </div>
    );
};
