import { FormikErrors } from "formik";
import { InvoiceProcessingResult } from "../../../api/models/InvoiceManagement/InvoiceProcessingResult";
import { IHttpRequest } from "./state";
import { InvoiceStatus } from "./state";
import {
    defaultDocumentViewState,
    IDocumentView,
    IInvoiceDataAnnotation,
    IInvoiceDataCaptureErrors,
    IInvoiceFields
} from "./state";
import { IInvoice } from "./state/IInvoice";

export interface IInvoiceDataCaptureToolkitState {
    invoiceFields?: IInvoiceFields;
    invoiceDataAnnotation?: IInvoiceDataAnnotation;
    cleanProcessingResults?: InvoiceProcessingResult;
    documentView: IDocumentView;
    pendingHttpRequests: IHttpRequest[];
    invoiceStatus: InvoiceStatus;
    invoiceId?: number;
    trainingsCount?: number;
    invoiceFileName: string;
    error?: IInvoiceDataCaptureErrors;
    invoiceFieldsValidationErrors?: FormikErrors<IInvoiceFields>;
    isInvoiceSavingInProgress: boolean;
    isInvoiceSubmittingInProgress: boolean;
    isInvoiceSubmitted: boolean;
    invoicesList: IInvoice[];
    tableSelectionMode: boolean;
    cultureName?: string;
    vendorName?: string;
}

export const defaultInvoiceDataCaptureToolkitState: IInvoiceDataCaptureToolkitState = {
    invoiceFields: undefined,
    invoiceDataAnnotation: undefined,
    cleanProcessingResults: undefined,
    documentView: defaultDocumentViewState,
    invoiceStatus: InvoiceStatus.Undefined,
    invoiceId: undefined,
    invoiceFileName: "",
    pendingHttpRequests: [],
    error: undefined,
    invoiceFieldsValidationErrors: undefined,
    isInvoiceSavingInProgress: false,
    isInvoiceSubmittingInProgress: false,
    isInvoiceSubmitted: false,
    invoicesList: [],
    tableSelectionMode: false,
    cultureName: undefined,
    vendorName: undefined
};
