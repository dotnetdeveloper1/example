import { useSelector } from "react-redux";
import { RootState } from "../../../../store/configuration";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { InvoiceStatus } from "../state";
import {
    FETCH_INVOICE_DATA_ASYNC_ACTION,
    SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION,
    SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION
} from "./../actions/Actions";
import {
    assignedDocumentLayoutItemsPerPageSelector,
    IDocumentLayoutItemsPerPageLookup as IDocumentLayoutItemsPerPageLookupState
} from "./assignedDocumentLayoutItemsPerPageSelector";
import { assignedDocumentLayoutItemsSelector } from "./assignedDocumentLayoutItemsSelector";
import { invoiceFieldsInFocusSelector } from "./invoiceFieldsInFocusSelector";
import { lineItemsFieldsInFocusSelector } from "./lineItemsFieldsInFocusSelector";
import { tempLineItemsSelector } from "./tempLineItemsSelector";

export function useToolkitSelector<TSelected = unknown>(
    selector: (state: IInvoiceDataCaptureToolkitState) => TSelected,
    equalityFn?: (left: TSelected, right: TSelected) => boolean
): TSelected {
    return useSelector<RootState, TSelected>((state) => selector(state.invoiceDataCaptureToolkit), equalityFn);
}

export const toolkitStateSelector = (state: IInvoiceDataCaptureToolkitState) => state;

export const invoiceFieldsSelector = (state: IInvoiceDataCaptureToolkitState) => state.invoiceFields;

export const invoiceFieldsValidationErrorsSelector = (state: IInvoiceDataCaptureToolkitState) =>
    state.invoiceFieldsValidationErrors;

export const invoiceDataAnnotationSelector = (state: IInvoiceDataCaptureToolkitState) => state.invoiceDataAnnotation;

export const errorSelector = (state: IInvoiceDataCaptureToolkitState) => state.error;

export const errorConfirmedSelector = (state: IInvoiceDataCaptureToolkitState) => !state.error || state.error.confirm;

export const emptyStateSelector = (state: IInvoiceDataCaptureToolkitState) =>
    !state.pendingHttpRequests.some((request) => request.type === FETCH_INVOICE_DATA_ASYNC_ACTION) &&
    !state.cleanProcessingResults &&
    !state.error;

export const isFetchingInProgressSelector = (state: IInvoiceDataCaptureToolkitState) =>
    state.pendingHttpRequests.some((request) => request.type === FETCH_INVOICE_DATA_ASYNC_ACTION);

export const isInvoiceSavingInProgressSelector = (state: IInvoiceDataCaptureToolkitState) =>
    state.pendingHttpRequests.some((request) => request.type === SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION);

export const isInvoiceSubmittingInProgressSelector = (state: IInvoiceDataCaptureToolkitState) =>
    state.pendingHttpRequests.some((request) => request.type === SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION);

export const isAnyHttpRequestPendingSelector = (state: IInvoiceDataCaptureToolkitState) =>
    state.pendingHttpRequests.length > 0;

export const isDataAnnotationsSaveEnabledSelector = (state: IInvoiceDataCaptureToolkitState) =>
    !state.isInvoiceSavingInProgress &&
    !state.isInvoiceSubmittingInProgress &&
    state.invoiceStatus === InvoiceStatus.PendingReview &&
    state.invoiceFieldsValidationErrors !== undefined &&
    Object.keys(state.invoiceFieldsValidationErrors).length === 0;

export const invoiceStatusSelector = (state: IInvoiceDataCaptureToolkitState) => state.invoiceStatus;

export const documentViewSelector = (state: IInvoiceDataCaptureToolkitState) => state.documentView;
export const invoiceItemsSelector = (state: IInvoiceDataCaptureToolkitState) => state.invoiceFields?.lineItems;

export const invoicesListSelector = (state: IInvoiceDataCaptureToolkitState) => state.invoicesList;

export const invoiceFileNameSelector = (state: IInvoiceDataCaptureToolkitState) => state.invoiceFileName;

export const tableSelectionModeSelector = (state: IInvoiceDataCaptureToolkitState) => state.tableSelectionMode;

export const cultureSelector = (state: IInvoiceDataCaptureToolkitState) => state.cultureName;

export const templateIdSelector = (state: IInvoiceDataCaptureToolkitState) => state.cleanProcessingResults?.templateId;

export const trainingsCountSelector = (state: IInvoiceDataCaptureToolkitState) => state.trainingsCount;

export const vendorNameSelector = (state: IInvoiceDataCaptureToolkitState) => state.vendorName;

export const lineItemAnnotationsSelector = (state: IInvoiceDataCaptureToolkitState) =>
    state.invoiceDataAnnotation?.invoiceLineItemAnnotation;

export type IDocumentLayoutItemsPerPageLookup = IDocumentLayoutItemsPerPageLookupState;

export {
    assignedDocumentLayoutItemsSelector,
    assignedDocumentLayoutItemsPerPageSelector,
    invoiceFieldsInFocusSelector,
    lineItemsFieldsInFocusSelector,
    tempLineItemsSelector
};
