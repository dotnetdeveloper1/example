import { settings } from "../settings/SettingsProvider";
import { BaseApiEndpoint } from "./BaseApiEndpoint";
import { FieldGroup } from "./models/InvoiceFields/FieldGroup";

export class FieldGroupApiEndpointPaths {
    public static GET_FIELD_GROUPS = (invoiceId: number) => `fieldgroups`;
}

export class FieldGroupApiEndpoint extends BaseApiEndpoint {
    public constructor() {
        super({
            baseURL: settings.fieldGroupApiEndpoint,
            timeout: 4000
        });
    }

    public getFieldGroups = async (invoiceId: number): Promise<FieldGroup[]> => {
        return this.get(FieldGroupApiEndpointPaths.GET_FIELD_GROUPS(invoiceId));
    };
}

export const fieldGroupApi = new FieldGroupApiEndpoint();
