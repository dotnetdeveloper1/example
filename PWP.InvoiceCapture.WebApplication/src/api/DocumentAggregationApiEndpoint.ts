import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { settings } from "../settings/SettingsProvider";
import { ApiResponse } from "./models/ApiResponse";
import { UploadFileModel } from "./models/InvoiceManagement/UploadFileModel";

export class DocumentAggregationApiEndpointPaths {
    public static POST_INVOICE_FILE = () => `invoiceDocuments/`;
}

export class DocumentAggregationApiEndpoint {
    public constructor() {
        this.endpoint = axios.create({
            baseURL: settings.documentAggregationApiEndpoint,
            timeout: 4000
        });
        this.configureInterceptors();
    }

    protected configureInterceptors(): void {
        if (this.endpoint && this.endpoint.interceptors) {
            this.endpoint.interceptors.request.use((param: AxiosRequestConfig) => ({
                ...param
            }));
            this.endpoint.interceptors.request.use(this.interceptRequestMessage, this.interceptRequestError);
            this.endpoint.interceptors.response.use(this.interceptResponseMessage, this.interceptResponseError);
        }
    }

    protected interceptRequestMessage = async (requestConfig: AxiosRequestConfig): Promise<AxiosRequestConfig> => {
        requestConfig.headers["apiVersion"] = settings.apiVersion;
        return requestConfig;
    };

    protected interceptRequestError = async (error: any): Promise<any> => {
        return Promise.reject(error);
    };

    protected interceptResponseMessage = async (response: AxiosResponse<any>): Promise<AxiosResponse<any>> => {
        return response;
    };

    protected interceptResponseError = async (error: any): Promise<any> => {
        if (error.response) {
            return Promise.reject(error);
        }
    };

    public postInvoiceFile = async (data: UploadFileModel): Promise<ApiResponse> => {
        const formData = new FormData();
        formData.append("file", data.file);
        return this.endpoint.post(DocumentAggregationApiEndpointPaths.POST_INVOICE_FILE(), formData, {
            headers: { "Content-Type": "multipart/form-data", "Authorization": `Bearer ${data.accessToken}` }
        });
    };

    private endpoint: AxiosInstance;
}

export const documentAggregationApi = new DocumentAggregationApiEndpoint();
