import { AxiosRequestConfig } from "axios";
import { settings } from "../settings/SettingsProvider";
import { BaseApiEndpoint } from "./BaseApiEndpoint";
import { ApiResponse } from "./models/ApiResponse";
import { InvoiceDataAnnotationSaveModel } from "./models/InvoiceManagement/InvoiceDataAnnotation";
import { InvoiceProcessingResult } from "./models/InvoiceManagement/InvoiceProcessingResult";

export class InvoiceProcessingResultApiPaths {
    public static GET_PROCESSING_RESULT_BY_INVOICE_ID = (invoiceId: number) =>
        `/processingResults/invoices/${invoiceId}/latest`;
    public static SAVE_PROCESSING_RESULT = (processingResultId: number) =>
        `/processingResults/${processingResultId}/dataAnnotation`;
    public static SUBMIT_PROCESSING_RESULT = (processingResultId: number) =>
        `/processingResults/${processingResultId}/complete`;
}

export class InvoiceProcessingResultApiEndpoint extends BaseApiEndpoint {
    public constructor() {
        super({
            baseURL: settings.invoiceManagementApiEndpoint,
            timeout: 4000
        });
    }

    public getProcessingResults = async (invoiceId: number): Promise<InvoiceProcessingResult> => {
        return this.get(InvoiceProcessingResultApiPaths.GET_PROCESSING_RESULT_BY_INVOICE_ID(invoiceId));
    };

    public saveProcessingResults = async (
        processingResultId: number,
        model: InvoiceDataAnnotationSaveModel
    ): Promise<ApiResponse> => {
        return this.put<InvoiceDataAnnotationSaveModel, ApiResponse>(
            InvoiceProcessingResultApiPaths.SAVE_PROCESSING_RESULT(processingResultId),
            model
        );
    };

    public submitProcessingResults = async (
        processingResultId: number,
        model: InvoiceDataAnnotationSaveModel
    ): Promise<ApiResponse> => {
        return this.put<InvoiceDataAnnotationSaveModel, ApiResponse>(
            InvoiceProcessingResultApiPaths.SUBMIT_PROCESSING_RESULT(processingResultId),
            model
        );
    };

    protected interceptRequestMessage = async (config: AxiosRequestConfig): Promise<AxiosRequestConfig> => {
        return {
            ...config,
            headers: {
                ...config.headers,
                "Accept": "application/json",
                "Content-Type": "application/json;charset=UTF-8"
            }
        };
    };
}

export const invoiceProcessingResultApi = new InvoiceProcessingResultApiEndpoint();
