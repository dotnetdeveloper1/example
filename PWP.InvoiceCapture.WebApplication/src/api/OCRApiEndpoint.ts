import { AxiosRequestConfig } from "axios";
import { settings } from "../settings/SettingsProvider";
import { BaseApiEndpoint } from "./BaseApiEndpoint";

export class InvoiceProcessingResultApiPaths {
    public static GET_TRAININGS_COUNT_BY_TEMPLATE_ID = (templateId: number) =>
        `/invoiceTemplates/${templateId}/trainingscount`;
}

export class OCRApiEndpoint extends BaseApiEndpoint {
    public constructor() {
        super({
            baseURL: settings.ocrApiEndpoint,
            timeout: 4000
        });
    }

    public getTrainingsCount = async (templateId: number): Promise<number> => {
        return this.get(InvoiceProcessingResultApiPaths.GET_TRAININGS_COUNT_BY_TEMPLATE_ID(templateId));
    };

    protected interceptRequestMessage = async (config: AxiosRequestConfig): Promise<AxiosRequestConfig> => {
        return {
            ...config,
            headers: {
                ...config.headers,
                "Accept": "application/json",
                "Content-Type": "application/json;charset=UTF-8",
                "apiVersion": settings.apiVersion
            }
        };
    };
}

export const ocrApiEndpoint = new OCRApiEndpoint();
