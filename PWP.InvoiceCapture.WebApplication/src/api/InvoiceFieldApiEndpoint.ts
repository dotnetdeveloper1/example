import { settings } from "../settings/SettingsProvider";
import { BaseApiEndpoint } from "./BaseApiEndpoint";
import { InvoiceField } from "./models/InvoiceFields/InvoiceField";

export class InvoiceFieldApiEndpointPaths {
    public static GET_INVOICE_FIELDS = (invoiceId: number) => `fieldgroups`;
}

export class InvoiceFieldApiEndpoint extends BaseApiEndpoint {
    public constructor() {
        super({
            baseURL: settings.invoiceFieldApiEndpoint,
            timeout: 4000
        });
    }

    public getInvoiceFields = async (invoiceId: number): Promise<InvoiceField[]> => {
        return this.get(InvoiceFieldApiEndpointPaths.GET_INVOICE_FIELDS(invoiceId));
    };
}

export const invoiceFieldApi = new InvoiceFieldApiEndpoint();
