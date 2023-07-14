import { PayloadAction } from "@reduxjs/toolkit";
import { FormikErrors } from "formik";
import { IInvoiceFields } from "../state/IInvoiceFields";
import { IInvoiceDataCaptureToolkitState } from "./../InvoiceDataCaptureToolkitState";

export function getInvoiceFieldsValidationErrorsReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<FormikErrors<IInvoiceFields>>
): void {
    state.invoiceFieldsValidationErrors = action.payload;
}
