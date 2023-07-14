import * as React from "react";
import { useTranslation } from "react-i18next";
import { LoadingPlaceholder } from "../common/LoadingPlaceholder";
import { ControlPanel } from "./ControlPanel";
import { ErrorNotification } from "./ErrorNotification";
import { Header } from "./Header";
import {
    useCloseInvoiceDataAnnotations,
    useDocumentViewState,
    useFetchInvoiceData,
    useInvoiceFieldsState,
    useSaveInvoiceDataAnnotations,
    useSubmitInvoiceDataAnnotations
} from "./hooks";
import "./InvoiceDataCaptureToolkitComponent.scss";
import { invoiceFileNameSelector, useToolkitSelector } from "./store/selectors";
import { InvoiceStatus } from "./store/state";
import { Toolkit } from "./Toolkit";

export const COMPONENT_NAME = "InvoiceDataCaptureToolkitComponent";

export const InvoiceDataCaptureToolkitComponent: React.FunctionComponent = React.memo(() => {
    const { isAnyHttpRequestPending, isErrorConfirmed, invoiceId, invoiceStatus } = useFetchInvoiceData();

    const {
        documentViewState,
        onDocumentViewPageChange,
        onLayoutItemsSelect,
        onAssignItemsToInvoiceField,
        onAssignLineItemsToInvoiceField,
        onAssignTableColumnToInvoiceLineItemsField,
        tableSelectionMode
    } = useDocumentViewState();

    const {
        invoiceFieldsState,
        invoiceFieldTypesInFocus,
        lineItemsFieldTypesInFocus,
        onFieldFocus,
        onLineItemFieldFocus,
        onFieldBlur,
        onFieldsClearAll,
        onFieldsSubmit,
        onFormValidationStateChanged
    } = useInvoiceFieldsState();

    const { isSaveEnabled, onSaveProcessingResult } = useSaveInvoiceDataAnnotations();
    const { isSubmitEnabled, onSubmitProcessingResult } = useSubmitInvoiceDataAnnotations();
    const { onClose } = useCloseInvoiceDataAnnotations();
    const invoiceFileName = useToolkitSelector(invoiceFileNameSelector);

    const { t } = useTranslation();

    let currentId = 0;
    if (invoiceId) {
        currentId = invoiceId;
    }

    const invoiceIsLoading = isAnyHttpRequestPending || invoiceStatus === InvoiceStatus.Undefined;

    return (
        <>
            <div className={`${COMPONENT_NAME}`}>
                {invoiceIsLoading ? (
                    <LoadingPlaceholder waitMessage={t("APPLICATION_LOADING_MESSAGE")} />
                ) : (
                    <>
                        <Header
                            invoiceId={currentId}
                            invoiceStatus={invoiceStatus}
                            title={t("INVOICE_DATA_CAPTURE_REVIEW_HEADER_TITLE")}
                            invoiceFileName={invoiceFileName}
                        />
                        <Toolkit
                            pages={documentViewState.pages}
                            showCompareBoxes={documentViewState.compareBoxesVisible}
                            selectedPlainText={documentViewState.selectedPlainTextValue}
                            onPageChange={onDocumentViewPageChange}
                            onLayoutItemsSelect={onLayoutItemsSelect}
                            onAssignItems={onAssignItemsToInvoiceField}
                            onAssignLineItems={onAssignLineItemsToInvoiceField}
                            onAssignColumnToLineItems={onAssignTableColumnToInvoiceLineItemsField}
                            fieldsState={invoiceFieldsState}
                            fieldTypesInFocus={invoiceFieldTypesInFocus}
                            lineItemsFieldTypesInFocus={lineItemsFieldTypesInFocus}
                            onFieldFocus={onFieldFocus}
                            onLineItemFieldFocus={onLineItemFieldFocus}
                            onFieldBlur={onFieldBlur}
                            onFieldsClearAll={onFieldsClearAll}
                            onFieldsSubmit={onFieldsSubmit}
                            onFormValidationStateChanged={onFormValidationStateChanged}
                            invoiceStatus={invoiceStatus}
                            tableSelectionMode={tableSelectionMode}
                        />
                        <ControlPanel
                            compareBoxes={documentViewState.compareBoxesVisible}
                            currentPageNumber={documentViewState.currentPageNumber}
                            pageCount={documentViewState.pageCount}
                            onPageChange={onDocumentViewPageChange}
                            isSaveEnabled={isSaveEnabled}
                            isSubmitEnabled={isSubmitEnabled}
                            onSaveProcessingResult={onSaveProcessingResult}
                            onSubmitProcessingResult={onSubmitProcessingResult}
                            onClose={onClose}
                            tableSelectionMode={tableSelectionMode}
                            invoiceStatus={invoiceStatus}
                        />
                    </>
                )}
            </div>
            <ErrorNotification isErrorConfirmed={isErrorConfirmed} />
        </>
    );
});
