import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { settings } from "../settings/SettingsProvider";
import { TokenResponse } from "./models/Identity/TokenResponse";

export enum IdentityApiEndpointPaths {
    TOKEN = "/connect/token"
}

export class IdentityApiEndpoint {
    public constructor() {
        this.endpoint = axios.create({ baseURL: settings.identityApiEndpoint, timeout: 4000 });
        if (this.endpoint && this.endpoint.interceptors) {
            this.endpoint.interceptors.request.use(this.interceptRequestMessage, this.interceptRequestError);
            this.endpoint.interceptors.response.use(this.interceptResponseMessage, this.interceptResponseError);
        }
    }

    public token = async (data: string): Promise<TokenResponse> => {
        return this.endpoint.post(IdentityApiEndpointPaths.TOKEN, data).then((response) => {
            return Promise.resolve(response.data);
        });
    };

    protected interceptRequestMessage = async (config: AxiosRequestConfig): Promise<AxiosRequestConfig> => {
        return {
            ...config,
            headers: {
                ...config.headers,
                "Accept": "*/*",
                "Content-Type": "application/x-www-form-urlencoded",
                "apiVersion": settings.apiVersion
            }
        };
    };

    protected interceptRequestError = async (error: any): Promise<any> => {
        return Promise.reject(error);
    };

    protected interceptResponseMessage = async (response: AxiosResponse<any>): Promise<AxiosResponse<any>> => {
        return response;
    };

    protected interceptResponseError = async (error: any): Promise<any> => {
        return Promise.reject(error.response.data);
    };

    private endpoint: AxiosInstance;
}

export const identityApi = new IdentityApiEndpoint();
