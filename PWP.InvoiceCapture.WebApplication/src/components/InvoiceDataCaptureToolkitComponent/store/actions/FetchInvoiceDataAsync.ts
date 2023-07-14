import { createAsyncThunk } from "@reduxjs/toolkit";
import { invoiceApi } from "../../../../api/InvoiceApiEndpoint";
import { invoicePageApi } from "../../../../api/InvoicePageApiEndpoint";
import { invoiceProcessingResultApi } from "../../../../api/InvoiceProcessingResultApiEndpoint";
import { InvoiceProcessingResult } from "../../../../api/models/InvoiceManagement/InvoiceProcessingResult";
import { ocrApiEndpoint } from "../../../../api/OCRApiEndpoint";
import {
    clearInvoiceFields,
    getCleanProcessingResults,
    getCulture,
    getInvoiceDataAnnotation,
    getInvoiceFields,
    getInvoiceFileName,
    getInvoiceId,
    getInvoicePages,
    getInvoiceState,
    getTrainingsCount,
    getVendorName
} from "../InvoiceDataCaptureToolkitStoreSlice";
import { IInvoiceDataCaptureErrors, InvoiceStatus } from "../state";
import { fieldGroupApi } from "./../../../../api/FieldGroupApiEndpoint";
import { ApiModelMapper } from "./../mappers/ApiModelMapper";
import { FETCH_INVOICE_DATA_ASYNC_ACTION } from "./Actions";

export const createFetchInvoiceDataAsyncAction = () => {
    return createAsyncThunk<Promise<any>, number, { rejectValue: IInvoiceDataCaptureErrors }>(
        FETCH_INVOICE_DATA_ASYNC_ACTION,
        async (invoiceId: number, thunkAPI) => {
            try {
                const invoice = await invoiceApi.getInvoice(invoiceId);
                const invoiceStatus =
                    invoice && invoice.status !== undefined ? invoice.status : InvoiceStatus.Undefined;
                thunkAPI.dispatch(getInvoiceState(invoiceStatus));
                thunkAPI.dispatch(getInvoiceId(invoiceId));
                thunkAPI.dispatch(getInvoiceFileName(invoice.fileName));
                if (invoiceStatus > InvoiceStatus.InProgress && invoiceStatus < InvoiceStatus.Error) {
                    const processingResults = await invoiceProcessingResultApi.getProcessingResults(invoiceId);
                    const invoicePages = await invoicePageApi.pages(invoiceId);

                    thunkAPI.dispatch(getCleanProcessingResults(processingResults));

                    if (processingResults.templateId) {
                        const trainingsCount = await ocrApiEndpoint.getTrainingsCount(processingResults.templateId!);
                        thunkAPI.dispatch(getTrainingsCount(trainingsCount));
                    }

                    if (processingResults.vendorName) {
                        thunkAPI.dispatch(getVendorName(processingResults.vendorName));
                    }

                    const mapper = new ApiModelMapper(processingResults, invoicePages);
                    const fieldGroups = await fieldGroupApi.getFieldGroups(invoiceId);

                    const invoiceDataAnnotation = mapper.selectInvoiceDataAnnotation(fieldGroups);
                    thunkAPI.dispatch(getInvoiceDataAnnotation(invoiceDataAnnotation));

                    const cultureName = mapper.selectCulture();
                    thunkAPI.dispatch(getCulture(cultureName));

                    const pages = await mapper.selectDocumentPages();
                    thunkAPI.dispatch(getInvoicePages(pages));

                    const fields = mapper.selectInvoiceFields(
                        invoiceDataAnnotation.invoiceFieldsAnnotation,
                        fieldGroups
                    );
                    thunkAPI.dispatch(getInvoiceFields(fields));
                } else {
                    const emptyResult = {} as InvoiceProcessingResult;
                    thunkAPI.dispatch(getCleanProcessingResults(emptyResult));
                    thunkAPI.dispatch(clearInvoiceFields());
                }
            } catch (error) {
                return thunkAPI.rejectWithValue({ message: error, confirm: false });
            }
        }
    );
};
