import React from "react";
import { useTranslation } from "react-i18next";
import "./DocumentPageImagePaneComponent.scss";

interface DocumentPageImagePaneComponentProps {
    pageNumber: number;
    pageImageLink: string;
    width: number;
    height: number;
}

export const COMPONENT_NAME = "DocumentPageImagePaneComponent";

export const DocumentPageImagePaneComponent: React.FunctionComponent<DocumentPageImagePaneComponentProps> = (props) => {
    const { t } = useTranslation();
    const { pageNumber, pageImageLink, width, height } = props;

    return (
        <img
            className={`${COMPONENT_NAME}`}
            alt={`${t("PAGE_TITLE")} ${pageNumber}`}
            src={pageImageLink}
            width={width}
            height={height}
        />
    );
};
