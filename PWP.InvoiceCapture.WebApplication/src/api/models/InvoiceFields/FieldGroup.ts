import { Field } from "./Field";

export interface FieldGroup {
    id: string;
    displayName: string;
    fields: Field[];
    orderNumber: number;
    isProtected: boolean;
    createdDate: Date;
    modifiedDate: Date;
}
