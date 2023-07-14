import { PayloadAction, SerializedError } from "@reduxjs/toolkit";
import { UploadFileModel } from "./../../../../api/models/InvoiceManagement/UploadFileModel";
import { POST_INVOICE_FILE_ASYNC_ACTION } from "./../actions/Actions";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";
import { IInvoiceDataCaptureErrors } from "./../state/IInvoiceDataCaptureErrors";

export function postInvoiceFileAsyncRejectedReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<
        IInvoiceDataCaptureErrors | undefined,
        string,
        { arg: UploadFileModel; requestId: string; aborted: boolean },
        SerializedError
    >
): void {
    state.pendingHttpRequests = state.pendingHttpRequests.filter(
        (request) => request.type !== POST_INVOICE_FILE_ASYNC_ACTION
    );
    if (action.payload) {
        state.error = { ...action.payload, confirm: false };
    } else {
        state.error = { message: "Internal Error", confirm: false };
    }
}
