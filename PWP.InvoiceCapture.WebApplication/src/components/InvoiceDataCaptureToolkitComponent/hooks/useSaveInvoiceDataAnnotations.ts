import { useCallback, useMemo } from "react";
import { useDispatch } from "react-redux";
import {
    clearTempInvoiceLines,
    saveInvoiceDataAnnotationsAsync,
    selectDocumentLayoutItems
} from "../store/InvoiceDataCaptureToolkitStoreSlice";
import { StateModelMapper } from "../store/mappers/StateModelMapper";
import { isInvoiceSavingInProgressSelector, toolkitStateSelector, useToolkitSelector } from "../store/selectors";
import { InvoiceStatus } from "../store/state";
import { isInvoiceSubmittingInProgressSelector } from "./../store/selectors";

interface ISaveInvoiceDataAnnotationsHookResult {
    isSaveEnabled: boolean;
    onSaveProcessingResult: () => void;
}

export function useSaveInvoiceDataAnnotations(): ISaveInvoiceDataAnnotationsHookResult {
    const dispatch = useDispatch();
    const isInvoiceSubmittingInProgress = useToolkitSelector(isInvoiceSubmittingInProgressSelector);
    const isInvoiceSavingInProgress = useToolkitSelector(isInvoiceSavingInProgressSelector);
    const state = useToolkitSelector(toolkitStateSelector);

    const isSaveEnabled = useMemo(
        () =>
            !isInvoiceSavingInProgress &&
            !isInvoiceSubmittingInProgress &&
            state.invoiceStatus === InvoiceStatus.PendingReview &&
            state.invoiceFieldsValidationErrors !== undefined &&
            Object.keys(state.invoiceFieldsValidationErrors).length === 0,
        [state, isInvoiceSavingInProgress, isInvoiceSubmittingInProgress]
    );

    const onSaveProcessingResult = useCallback(() => {
        if (isSaveEnabled) {
            const mapper = new StateModelMapper(state);
            const updatedProcessingResult = mapper.getUpdatedProcessingResults();
            dispatch(selectDocumentLayoutItems([]));
            dispatch(clearTempInvoiceLines());
            dispatch(saveInvoiceDataAnnotationsAsync(updatedProcessingResult));
        }
    }, [dispatch, isSaveEnabled, state]);

    return {
        isSaveEnabled: isSaveEnabled,
        onSaveProcessingResult: onSaveProcessingResult
    };
}
