import { useCallback, useMemo } from "react";
import { useDispatch } from "react-redux";
import {
    clearTempInvoiceLines,
    getTableSelectionMode,
    selectDocumentLayoutItems,
    submitInvoiceDataAnnotationsAsync
} from "../store/InvoiceDataCaptureToolkitStoreSlice";
import { StateModelMapper } from "../store/mappers/StateModelMapper";
import { isInvoiceSavingInProgressSelector, toolkitStateSelector, useToolkitSelector } from "../store/selectors";
import { InvoiceStatus } from "../store/state";
import { isInvoiceSubmittingInProgressSelector } from "./../store/selectors";
import { useCloseInvoiceDataAnnotations } from "./useCloseInvoiceDataAnnotations";

interface ISubmitInvoiceDataAnnotationsHookResult {
    isSubmitEnabled: boolean;
    onSubmitProcessingResult: () => void;
}

export function useSubmitInvoiceDataAnnotations(): ISubmitInvoiceDataAnnotationsHookResult {
    const dispatch = useDispatch();
    const isInvoiceSubmittingInProgress = useToolkitSelector(isInvoiceSubmittingInProgressSelector);
    const isInvoiceSavingInProgress = useToolkitSelector(isInvoiceSavingInProgressSelector);
    const state = useToolkitSelector(toolkitStateSelector);
    const { onClose } = useCloseInvoiceDataAnnotations();

    const isSubmitEnabled = useMemo(
        () =>
            !isInvoiceSavingInProgress &&
            !isInvoiceSubmittingInProgress &&
            state.invoiceStatus === InvoiceStatus.PendingReview &&
            state.invoiceFieldsValidationErrors !== undefined &&
            Object.keys(state.invoiceFieldsValidationErrors).length === 0,
        [state, isInvoiceSavingInProgress, isInvoiceSubmittingInProgress]
    );

    const onSubmitProcessingResult = useCallback(() => {
        const submitInvoice = async () => {
            if (isSubmitEnabled) {
                const mapper = new StateModelMapper(state);
                const updatedProcessingResult = mapper.getUpdatedProcessingResults();
                await Promise.all([
                    dispatch(clearTempInvoiceLines()),
                    dispatch(getTableSelectionMode(false)),
                    dispatch(selectDocumentLayoutItems([])),
                    dispatch(submitInvoiceDataAnnotationsAsync(updatedProcessingResult))
                ]);
                onClose();
            }
        };
        submitInvoice();
    }, [dispatch, isSubmitEnabled, onClose, state]);

    return {
        isSubmitEnabled: isSubmitEnabled,
        onSubmitProcessingResult: onSubmitProcessingResult
    };
}
