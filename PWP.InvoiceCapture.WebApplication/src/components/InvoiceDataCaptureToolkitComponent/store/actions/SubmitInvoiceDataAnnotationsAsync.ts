import { createAsyncThunk } from "@reduxjs/toolkit";
import { InvoiceProcessingResultsUpdateModel } from "../../../../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { IInvoiceDataCaptureErrors } from "../state";
import { invoiceProcessingResultApi } from "./../../../../api/InvoiceProcessingResultApiEndpoint";
import { SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION } from "./Actions";

export const createSubmitInvoiceDataAnnotationsAsyncAction = () => {
    return createAsyncThunk<
        Promise<any>,
        InvoiceProcessingResultsUpdateModel,
        { rejectValue: IInvoiceDataCaptureErrors }
    >(SUBMIT_INVOICE_PROCESSING_RESULT_ASYNC_ACTION, async (data: InvoiceProcessingResultsUpdateModel, thunkAPI) => {
        try {
            await invoiceProcessingResultApi.submitProcessingResults(data.processingResultId, data.dataAnnotation);
        } catch (error) {
            return thunkAPI.rejectWithValue(JSON.parse(JSON.stringify(error)));
        }
    });
};
