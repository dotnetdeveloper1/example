import { FieldType } from "./FieldType";

export interface Field {
    id: string;
    groupId: string;
    orderNumber: number;
    isProtected: boolean;
    isDeleted: boolean;
    displayName: string;
    type: FieldType;
    defaultValue: string | undefined;
    isRequired: boolean;
    minValue: number | undefined;
    maxValue: number | undefined;
    minLength: number | undefined;
    maxLength: number | undefined;
    createdDate: Date;
    modifiedDate: Date;
    formula: string;
}
