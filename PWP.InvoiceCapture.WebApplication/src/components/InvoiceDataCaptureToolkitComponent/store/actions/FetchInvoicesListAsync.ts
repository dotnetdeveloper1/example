import { createAsyncThunk } from "@reduxjs/toolkit";
import { getInvoicesList } from "../InvoiceDataCaptureToolkitStoreSlice";
import { invoicesManagementApi } from "./../../../../api/invoicesManagementApiEndpoint";
import { ApiModelToStateMapper } from "./../mappers/ApiModelToStateMapper";
import { IInvoiceDataCaptureErrors } from "./../state/IInvoiceDataCaptureErrors";
import { FETCH_INVOICES_LIST_ASYNC_ACTION } from "./Actions";

export const createFetchInvoicesListAsyncAction = () => {
    return createAsyncThunk<Promise<any>, string, { rejectValue: IInvoiceDataCaptureErrors }>(
        FETCH_INVOICES_LIST_ASYNC_ACTION,
        async (param: string, thunkAPI) => {
            try {
                const invoices = await invoicesManagementApi.getInvoicesList(param);
                const mapper = new ApiModelToStateMapper();
                const invoicesList = mapper.mapInvoicesList(invoices);
                if (invoicesList) {
                    thunkAPI.dispatch(getInvoicesList(invoicesList));
                }
            } catch (error) {
                return thunkAPI.rejectWithValue({ message: error, confirm: false });
            }
        }
    );
};
