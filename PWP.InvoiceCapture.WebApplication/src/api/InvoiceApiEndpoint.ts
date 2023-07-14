import { settings } from "../settings/SettingsProvider";
import { BaseApiEndpoint } from "./BaseApiEndpoint";
import { Invoice } from "./models/InvoiceManagement/Invoice";

export class InvoiceApiEndpointPaths {
    public static GET_DOCUMENT_FILE_LINK = (invoiceId: number) => `invoices/${invoiceId}/document`;
    public static GET_INVOICE = (invoiceId: number) => `invoices/${invoiceId}`;
}

export class InvoiceApiEndpoint extends BaseApiEndpoint {
    public constructor() {
        super({
            baseURL: settings.invoiceManagementApiEndpoint,
            timeout: 4000
        });
    }

    public getDocumentLink = async (invoiceId: number): Promise<string> => {
        return this.get(InvoiceApiEndpointPaths.GET_DOCUMENT_FILE_LINK(invoiceId));
    };

    public getInvoice = async (invoiceId: number): Promise<Invoice> => {
        return this.get(InvoiceApiEndpointPaths.GET_INVOICE(invoiceId));
    };
}

export const invoiceApi = new InvoiceApiEndpoint();
