import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { settings } from "./../settings/SettingsProvider";
import { identityApi } from "./IdentityApiEndpoint";
import { ApiDataResponse } from "./models/ApiDataResponse";

export class BaseApiEndpoint {
    protected constructor(protected endpointConfig: AxiosRequestConfig) {
        this.endpoint = axios.create(endpointConfig);
        this.configureInterceptors();
    }

    protected configureInterceptors(): void {
        if (this.endpoint && this.endpoint.interceptors) {
            this.endpoint.interceptors.request.use((param: AxiosRequestConfig) => ({
                ...this.endpointConfig,
                ...param
            }));
            this.endpoint.interceptors.request.use(this.interceptRequestMessage, this.interceptRequestError);
            this.endpoint.interceptors.response.use(this.interceptResponseMessage, this.interceptResponseError);
        }
    }

    protected interceptRequestMessage = async (requestConfig: AxiosRequestConfig): Promise<AxiosRequestConfig> => {
        const refreshToken = window.refreshToken;
        let accessToken;
        if (refreshToken) {
            accessToken = window.accessToken;
        }
        if (accessToken) {
            requestConfig.headers["Authorization"] = `Bearer ${accessToken}`;
            requestConfig.headers["apiVersion"] = settings.apiVersion;
        }
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
            if (error.response.status === 401) {
                const refreshToken = window.refreshToken;
                if (refreshToken) {
                    const tokenRequestData = {
                        grant_type: "refresh_token",
                        client_id: this.clientId,
                        client_secret: "password",
                        refresh_token: refreshToken
                    };
                    const urlEncodedData = Object.entries(tokenRequestData)
                        .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
                        .join("&");
                    const tokenResponse = await identityApi.token(urlEncodedData);
                    if (tokenResponse) {
                        window.accessToken = tokenResponse.access_token;
                    }
                }
                return Promise.reject(error.response.data.message);
            }
            if (error.response.status === 400) {
                return Promise.reject(error.response.data);
            } else {
                return Promise.reject(error);
            }
        }
    };

    protected getUri(config?: AxiosRequestConfig): string {
        return this.endpoint.getUri(config);
    }

    protected async request<ResponseModelType>(config: AxiosRequestConfig): Promise<AxiosResponse<ResponseModelType>> {
        return this.endpoint.request(config);
    }

    protected async get<ResponseModelType>(
        url: string,
        config?: AxiosRequestConfig,
        callback?: (status: number, data: ResponseModelType) => ResponseModelType
    ): Promise<ResponseModelType> {
        return this.endpoint
            .get<ResponseModelType, AxiosResponse<ApiDataResponse<ResponseModelType>>>(url, config)
            .then((response) => {
                if (response === undefined) {
                    const emptyResponse = { data: {}, code: 0 } as ApiDataResponse<ResponseModelType>;
                    return Promise.resolve(emptyResponse.data);
                }
                const apiResponse: ApiDataResponse<ResponseModelType> = response.data;
                if (callback) {
                    return Promise.resolve(callback(response.status, apiResponse.data));
                } else {
                    return Promise.resolve(apiResponse.data);
                }
            });
    }

    protected async post<RequestModelType, ResponseModelType>(
        url: string,
        data?: RequestModelType,
        config?: AxiosRequestConfig,
        callback?: (status: number, data: ResponseModelType) => ResponseModelType
    ): Promise<ResponseModelType> {
        return this.endpoint
            .post<ResponseModelType, AxiosResponse<ApiDataResponse<ResponseModelType>>>(
                url,
                data,
                config || this.endpointConfig
            )
            .then((response) => {
                const apiResponse: ApiDataResponse<ResponseModelType> = response.data;
                if (callback) {
                    return Promise.resolve(callback(response.status, apiResponse.data));
                } else {
                    return Promise.resolve(apiResponse.data);
                }
            });
    }

    protected async put<RequestModelType, ResponseModelType>(
        url: string,
        data?: RequestModelType,
        config?: AxiosRequestConfig,
        callback?: (status: number, data: ResponseModelType) => ResponseModelType
    ): Promise<ResponseModelType> {
        return this.endpoint
            .put<ResponseModelType, AxiosResponse<ResponseModelType>>(url, data, config || this.endpointConfig)
            .then((response) => {
                if (callback) {
                    return Promise.resolve(callback(response.status, response.data));
                } else {
                    return Promise.resolve(response.data);
                }
            });
    }

    protected async delete<ResponseModelType>(
        url: string,
        config?: AxiosRequestConfig,
        callback?: (status: number, data: ResponseModelType) => ResponseModelType
    ): Promise<ResponseModelType> {
        return this.endpoint.delete(url, config || this.endpointConfig).then((response) => {
            const apiResponse: ApiDataResponse<ResponseModelType> = response.data;
            if (callback) {
                return Promise.resolve(callback(response.status, apiResponse.data));
            } else {
                return Promise.resolve(apiResponse.data);
            }
        });
    }

    protected async patch<RequestModelType, ResponseModelType>(
        url: string,
        data?: RequestModelType,
        config?: AxiosRequestConfig,
        callback?: (status: number, data: ResponseModelType) => ResponseModelType
    ): Promise<ResponseModelType> {
        return this.endpoint
            .patch<ResponseModelType, AxiosResponse<ApiDataResponse<ResponseModelType>>>(
                url,
                data,
                config || this.endpointConfig
            )
            .then((response) => {
                const apiResponse: ApiDataResponse<ResponseModelType> = response.data;
                if (callback) {
                    return Promise.resolve(callback(response.status, apiResponse.data));
                } else {
                    return Promise.resolve(apiResponse.data);
                }
            });
    }

    private clientId: string = "webApplication";
    private endpoint: AxiosInstance;
}
