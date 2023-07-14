import { PayloadAction, SerializedError } from "@reduxjs/toolkit";
import { InvoiceProcessingResultsUpdateModel } from "../../../../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION } from "../actions/Actions";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDataCaptureErrors } from "../state";

export function saveInvoiceDataAnnotationsAsyncRejectedReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<
        IInvoiceDataCaptureErrors | undefined,
        string,
        { arg: InvoiceProcessingResultsUpdateModel; requestId: string; aborted: boolean },
        SerializedError
    >
): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== SAVE_INVOICE_PROCESSING_RESULT_ASYNC_ACTION
    );

    if (action.payload) {
        state.error = { ...action.payload, confirm: false };
    } else {
        state.error = { message: "Internal Error", confirm: false };
    }
}
