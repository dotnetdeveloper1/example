import { createAsyncThunk } from "@reduxjs/toolkit";
import { documentAggregationApi } from "./../../../../api/DocumentAggregationApiEndpoint";
import { UploadFileModel } from "./../../../../api/models/InvoiceManagement/UploadFileModel";
import { IInvoiceDataCaptureErrors } from "./../state/IInvoiceDataCaptureErrors";
import { POST_INVOICE_FILE_ASYNC_ACTION } from "./Actions";

export const createPostInvoiceFileAsyncAction = () => {
    return createAsyncThunk<Promise<any>, UploadFileModel, { rejectValue: IInvoiceDataCaptureErrors }>(
        POST_INVOICE_FILE_ASYNC_ACTION,
        async (data: UploadFileModel, thunkAPI) => {
            await documentAggregationApi.postInvoiceFile(data);
        }
    );
};
