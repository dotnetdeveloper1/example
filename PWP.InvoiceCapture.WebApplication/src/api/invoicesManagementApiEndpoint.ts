import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { settings } from "../settings/SettingsProvider";
import { ApiDataResponse } from "./models/ApiDataResponse";
import { Invoice } from "./models/InvoiceManagement/Invoice";

export class InvoicesManagementApiEndpointPaths {
    public static GET_INVOICES_LIST = () => `invoices/`;
}

export class InvoicesManagementApiEndpoint {
    public constructor() {
        this.endpoint = axios.create({
            baseURL: settings.invoiceManagementApiEndpoint,
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

    public getInvoicesList = async (accessToken: string): Promise<Invoice[]> => {
        return this.endpoint
            .get(InvoicesManagementApiEndpointPaths.GET_INVOICES_LIST(), {
                headers: { Authorization: `Bearer ${accessToken}` }
            })
            .then((response) => {
                if (response === undefined) {
                    const emptyResponse = { data: {}, code: 0 } as ApiDataResponse<Promise<Invoice[]>>;
                    return Promise.resolve(emptyResponse.data);
                }
                const apiResponse: ApiDataResponse<Promise<Invoice[]>> = response.data;

                return Promise.resolve(apiResponse.data);
            });
    };

    private endpoint: AxiosInstance;
}

export const invoicesManagementApi = new InvoicesManagementApiEndpoint();
