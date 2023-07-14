import React from "react";
import { useTranslation } from "react-i18next";
import { ReactPropsType } from "../../../helperTypes";
import { COMPONENT_NAME, DocumentPageNavigationComponent } from "./DocumentPageNavigationComponent";
import { usePageNavigationView } from "./hooks";

interface PageNavigationViewProps extends ReactPropsType<typeof DocumentPageNavigationComponent> {}

export const PageNavigationView: React.FunctionComponent<PageNavigationViewProps> = (props) => {
    const { t } = useTranslation();

    const { inputValue, onInputChange, onInputBlur } = usePageNavigationView(props);

    return (
        <div className={`${COMPONENT_NAME}__page-navigation-view`}>
            <p>{t("PAGE_TITLE")}</p>
            <input
                type="number"
                value={inputValue}
                onChange={onInputChange}
                onBlur={onInputBlur}
                min={1}
                max={props.totalPages}
            />
            <p>{`${t("OF_TITLE")} ${props.totalPages}`}</p>
        </div>
    );
};
