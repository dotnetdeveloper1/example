import { PayloadAction, SerializedError } from "@reduxjs/toolkit";
import { InvoiceProcessingResultsUpdateModel } from "../../../../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDataCaptureErrors } from "../state";
import { SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION } from "./../actions/Actions";

export function submitInvoiceDataAnnotationsAsyncRejectedReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<
        IInvoiceDataCaptureErrors | undefined,
        string,
        { arg: InvoiceProcessingResultsUpdateModel; requestId: string; aborted: boolean },
        SerializedError
    >
): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION
    );
    if (action.payload) {
        state.error = { ...action.payload, confirm: false };
    } else {
        state.error = { message: "Internal Error", confirm: false };
    }
}
