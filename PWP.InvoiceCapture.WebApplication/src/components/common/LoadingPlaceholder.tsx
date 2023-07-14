import React from "react";
import { useTranslation } from "react-i18next";

interface LoadingPlaceholderProps {
    waitMessage?: string;
}

export const LoadingPlaceholder: React.FunctionComponent<LoadingPlaceholderProps> = (props) => {
    const { t } = useTranslation();

    return (
        <div className="d-flex flex-row flex-fill justify-content-center align-items-center h-100 w-100">
            <div className="row">
                <div className="d-flex flex-column justify-content-start align-items-center">
                    <div className="d-flex flex-column align-items-center display-4 p-5 m-5">
                        <p className="h2 font-weight-normal text-secondary-accent">
                            {props.waitMessage || t("DEFAULT_LOADING_MESSAGE")}
                        </p>
                    </div>
                    <div className="d-flex h-100 p-5">
                        <div className="spinner-grow text-secondary" role="status" style={{ transform: "scale(5)" }}>
                            <span className="sr-only">{`${t("LOADING_MESSAGE")} ...`}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
