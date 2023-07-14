import { settings } from "../settings/SettingsProvider";
import { BaseApiEndpoint } from "./BaseApiEndpoint";
import { Field } from "./models/InvoiceFields/Field";

export class FieldApiEndpointPaths {
    public static GET_FIELDS = (invoiceId: number) => `fields`;
}

export class FieldApiEndpoint extends BaseApiEndpoint {
    public constructor() {
        super({
            baseURL: settings.fieldApiEndpoint,
            timeout: 4000
        });
    }

    public getInvoiceFields = async (invoiceId: number): Promise<Field[]> => {
        return this.get(FieldApiEndpointPaths.GET_FIELDS(invoiceId));
    };
}

export const fieldApi = new FieldApiEndpoint();
