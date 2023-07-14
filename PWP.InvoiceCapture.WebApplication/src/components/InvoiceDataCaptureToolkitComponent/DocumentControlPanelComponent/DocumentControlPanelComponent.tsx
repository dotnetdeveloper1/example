import React, { useCallback } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { ReactPropsType } from "../../../helperTypes";
import { Toggle } from "../../common/ToggleComponent/ToggleComponent";
import { clearTempInvoiceLines, getTableSelectionMode } from "../store/InvoiceDataCaptureToolkitStoreSlice";
import { InvoiceStatus } from "../store/state";
import "./DocumentControlPanelComponent.scss";
import { DocumentPageNavigationComponent } from "./DocumentPageNavigationComponent";
import { ToggleCompareBoxesButton } from "./ToggleCompareBoxesButton";

interface DocumentControlPanelComponentProps extends ReactPropsType<typeof ToggleCompareBoxesButton> {
    currentPageNumber: number;
    pageCount: number;
    onPageChange(pageNumber: number, autoScroll: boolean): void;
    tableSelectionMode: boolean;
    invoiceStatus: InvoiceStatus;
}

export const COMPONENT_NAME = "DocumentControlPanelComponent";

export const DocumentControlPanelComponent: React.FunctionComponent<DocumentControlPanelComponentProps> = (props) => {
    const dispatch = useDispatch();
    const { t } = useTranslation();

    const onPageChange = useCallback((pageNumber: number) => props.onPageChange(pageNumber, true), [props]);
    const onTableSelectionModeChanged = useCallback(
        (value) => {
            dispatch(getTableSelectionMode(value));
            dispatch(clearTempInvoiceLines());
        },
        [dispatch]
    );

    return (
        <div className={`${COMPONENT_NAME}`}>
            <div className={`${COMPONENT_NAME}__buttons`}>
                <ToggleCompareBoxesButton
                    compareBoxes={props.compareBoxes}
                    onToggleCompareBoxes={props.onToggleCompareBoxes}
                />
            </div>

            <div className={`${COMPONENT_NAME}__page-navigation-container`}>
                <DocumentPageNavigationComponent
                    pageNumber={props.currentPageNumber}
                    totalPages={props.pageCount}
                    onPageChange={onPageChange}
                />
            </div>

            <div className={`${COMPONENT_NAME}__toggle_buttons`}>
                <Toggle
                    title={t("BUTTON_TITLE->TABLE_SELECTION_MODE")}
                    isToggled={props.tableSelectionMode}
                    onToggled={onTableSelectionModeChanged}
                    enabled={props.invoiceStatus === InvoiceStatus.PendingReview}
                />
            </div>
        </div>
    );
};
