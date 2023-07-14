import { AxiosRequestConfig } from "axios";
import { settings } from "../settings/SettingsProvider";
import { BaseApiEndpoint } from "./BaseApiEndpoint";
import { InvoicePage } from "./models/InvoiceManagement/InvoicePage";

export class InvoicePageApiEndpointPaths {
    public static GET_INVOICE_PAGES = (invoiceId: number) => `/pages/invoices/${invoiceId}`;
    public static GET_PAGE_IMAGE = (pageId: number) => `/pages/${pageId}/imageLink`;
}

export class InvoicePageApiEndpoint extends BaseApiEndpoint {
    public constructor() {
        super({
            baseURL: settings.invoiceManagementApiEndpoint,
            timeout: 4000
        });
    }

    public pages = async (invoiceId: number): Promise<InvoicePage[]> => {
        return this.get(InvoicePageApiEndpointPaths.GET_INVOICE_PAGES(invoiceId));
    };

    public pageImageLink = async (pageId: number): Promise<string> => {
        return this.get(InvoicePageApiEndpointPaths.GET_PAGE_IMAGE(pageId));
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

export const invoicePageApi = new InvoicePageApiEndpoint();
