import { createSlice } from "@reduxjs/toolkit";
import { createFetchInvoiceDataAsyncAction } from "./actions/FetchInvoiceDataAsync";
import { createFetchInvoicesListAsyncAction } from "./actions/FetchInvoicesListAsync";
import { createPostInvoiceFileAsyncAction } from "./actions/PostInvoiceFileAsync";
import { createSaveInvoiceDataAnnotationsAsyncAction } from "./actions/SaveInvoiceDataAnnotationsAsync";
import { createSubmitInvoiceDataAnnotationsAsyncAction } from "./actions/SubmitInvoiceDataAnnotationsAsync";
import { defaultInvoiceDataCaptureToolkitState } from "./InvoiceDataCaptureToolkitState";
import {
    assignInvoiceItemsReducer,
    assignLayoutItemsToInvoiceFieldReducer,
    assignLayoutLineItemsToInvoiceFieldReducer,
    assignTableColumnToInvoiceLineItemsFieldReducer,
    changeCurrentPageReducer,
    changeInvoiceFieldDataAnnotationReducer,
    clearEmptyDataAnnotationsReducer,
    clearInvoiceFieldDataAnnotationSelectionReducer,
    clearInvoiceFieldsReducer,
    clearTempInvoiceLinesReducer,
    confirmErrorReducer,
    deleteInvoiceItemsReducer,
    fetchInvoiceDataAsyncFulfilledReducer,
    fetchInvoiceDataAsyncPendingReducer,
    fetchInvoiceDataAsyncRejectedReducer,
    fetchInvoicesListAsyncFulfilledReducer,
    fetchInvoicesListAsyncPendingReducer,
    fetchInvoicesListAsyncRejectedReducer,
    getCleanProcessingResultsReducer,
    getCultureReducer,
    getInvoiceDataAnnotationReducer,
    getInvoiceFieldsReducer,
    getInvoiceFieldsValidationErrorsReducer,
    getInvoiceFileNameReducer,
    getInvoiceIdReducer,
    getInvoicePagesReducer,
    getInvoicesListReducer,
    getInvoiceStateReducer,
    getTrainingsCountReducer,
    getVendorNameReducer,
    postInvoiceFileAsyncFulfilledReducer,
    postInvoiceFileAsyncPendingReducer,
    postInvoiceFileAsyncRejectedReducer,
    removeNoAssignmentLineItemsReducer,
    saveInvoiceDataAnnotationsAsyncFulfilledReducer,
    saveInvoiceDataAnnotationsAsyncPendingReducer,
    saveInvoiceDataAnnotationsAsyncRejectedReducer,
    selectDocumentLayoutItemsReducer,
    selectInvoiceFieldDataAnnotationReducer,
    selectInvoiceLineItemFieldDataAnnotationReducer,
    submitInvoiceDataAnnotationsAsyncFulfilledReducer,
    submitInvoiceDataAnnotationsAsyncPendingReducer,
    submitInvoiceDataAnnotationsAsyncRejectedReducer,
    switchToEditModeReducer,
    tableSelectionReducer,
    toggleCompareBoxVisibilityReducer,
    updateInvoiceFieldsAnnotationReducer
} from "./reducers";

export const fetchInvoiceDataAsync = createFetchInvoiceDataAsyncAction();
export const fetchInvoicesListAsync = createFetchInvoicesListAsyncAction();
export const postInvoiceFileAsync = createPostInvoiceFileAsyncAction();
export const saveInvoiceDataAnnotationsAsync = createSaveInvoiceDataAnnotationsAsyncAction();
export const submitInvoiceDataAnnotationsAsync = createSubmitInvoiceDataAnnotationsAsyncAction();

export const InvoiceDataCaptureToolkitStoreSlice = createSlice({
    name: "InvoiceDataCaptureToolkit",
    initialState: defaultInvoiceDataCaptureToolkitState,
    reducers: {
        getInvoicePages: getInvoicePagesReducer,
        getInvoiceFields: getInvoiceFieldsReducer,
        getInvoicesList: getInvoicesListReducer,
        deleteInvoiceItems: deleteInvoiceItemsReducer,
        getCleanProcessingResults: getCleanProcessingResultsReducer,
        getCulture: getCultureReducer,
        getInvoiceState: getInvoiceStateReducer,
        getInvoiceId: getInvoiceIdReducer,
        getTrainingsCount: getTrainingsCountReducer,
        getVendorName: getVendorNameReducer,
        getInvoiceFileName: getInvoiceFileNameReducer,
        getInvoiceDataAnnotation: getInvoiceDataAnnotationReducer,
        selectDocumentLayoutItems: selectDocumentLayoutItemsReducer,
        selectInvoiceFieldDataAnnotation: selectInvoiceFieldDataAnnotationReducer,
        selectInvoiceLineItemFieldDataAnnotation: selectInvoiceLineItemFieldDataAnnotationReducer,
        switchToEditMode: switchToEditModeReducer,
        clearInvoiceFieldDataAnnotationSelection: clearInvoiceFieldDataAnnotationSelectionReducer,
        clearInvoiceFields: clearInvoiceFieldsReducer,
        clearTempInvoiceLines: clearTempInvoiceLinesReducer,
        changeInvoiceFieldDataAnnotation: changeInvoiceFieldDataAnnotationReducer,
        assignSelectedLayoutItemsToInvoiceField: assignLayoutItemsToInvoiceFieldReducer,
        assignSelectedLayoutLineItemsToInvoiceField: assignLayoutLineItemsToInvoiceFieldReducer,
        assignTableColumnToInvoiceLineItemsField: assignTableColumnToInvoiceLineItemsFieldReducer,
        assignInvoiceItems: assignInvoiceItemsReducer,
        changeCurrentPage: changeCurrentPageReducer,
        toggleCompareBoxVisibility: toggleCompareBoxVisibilityReducer,
        confirmError: confirmErrorReducer,
        getInvoiceFieldsValidationErrors: getInvoiceFieldsValidationErrorsReducer,
        updateInvoiceFieldsAnnotation: updateInvoiceFieldsAnnotationReducer,
        clearEmptyDataAnnotations: clearEmptyDataAnnotationsReducer,
        getTableSelectionMode: tableSelectionReducer,
        removeNoAssignmentLineItems: removeNoAssignmentLineItemsReducer
    },
    extraReducers: (builder) => {
        builder.addCase(fetchInvoiceDataAsync.pending, fetchInvoiceDataAsyncPendingReducer);
        builder.addCase(fetchInvoiceDataAsync.fulfilled, fetchInvoiceDataAsyncFulfilledReducer);
        builder.addCase(fetchInvoiceDataAsync.rejected, fetchInvoiceDataAsyncRejectedReducer);

        builder.addCase(fetchInvoicesListAsync.pending, fetchInvoicesListAsyncPendingReducer);
        builder.addCase(fetchInvoicesListAsync.fulfilled, fetchInvoicesListAsyncFulfilledReducer);
        builder.addCase(fetchInvoicesListAsync.rejected, fetchInvoicesListAsyncRejectedReducer);

        builder.addCase(postInvoiceFileAsync.pending, postInvoiceFileAsyncPendingReducer);
        builder.addCase(postInvoiceFileAsync.fulfilled, postInvoiceFileAsyncFulfilledReducer);
        builder.addCase(postInvoiceFileAsync.rejected, postInvoiceFileAsyncRejectedReducer);

        builder.addCase(saveInvoiceDataAnnotationsAsync.pending, saveInvoiceDataAnnotationsAsyncPendingReducer);
        builder.addCase(saveInvoiceDataAnnotationsAsync.fulfilled, saveInvoiceDataAnnotationsAsyncFulfilledReducer);
        builder.addCase(saveInvoiceDataAnnotationsAsync.rejected, saveInvoiceDataAnnotationsAsyncRejectedReducer);

        builder.addCase(submitInvoiceDataAnnotationsAsync.pending, submitInvoiceDataAnnotationsAsyncPendingReducer);
        builder.addCase(submitInvoiceDataAnnotationsAsync.fulfilled, submitInvoiceDataAnnotationsAsyncFulfilledReducer);
        builder.addCase(submitInvoiceDataAnnotationsAsync.rejected, submitInvoiceDataAnnotationsAsyncRejectedReducer);
    }
});

export const {
    getInvoicesList,
    getInvoiceFields,
    getInvoicePages,
    getCleanProcessingResults,
    getCulture,
    getInvoiceDataAnnotation,
    getInvoiceState,
    getInvoiceId,
    getTrainingsCount,
    getInvoiceFileName,
    toggleCompareBoxVisibility,
    changeCurrentPage,
    changeInvoiceFieldDataAnnotation,
    selectInvoiceFieldDataAnnotation,
    selectInvoiceLineItemFieldDataAnnotation,
    clearInvoiceFieldDataAnnotationSelection,
    clearEmptyDataAnnotations,
    clearTempInvoiceLines,
    confirmError,
    selectDocumentLayoutItems,
    assignSelectedLayoutItemsToInvoiceField,
    assignSelectedLayoutLineItemsToInvoiceField,
    assignTableColumnToInvoiceLineItemsField,
    assignInvoiceItems,
    clearInvoiceFields,
    getInvoiceFieldsValidationErrors,
    updateInvoiceFieldsAnnotation,
    deleteInvoiceItems,
    getTableSelectionMode,
    switchToEditMode,
    removeNoAssignmentLineItems,
    getVendorName
} = InvoiceDataCaptureToolkitStoreSlice.actions;

export const InvoiceDataCaptureToolkitReducer = InvoiceDataCaptureToolkitStoreSlice.reducer;
