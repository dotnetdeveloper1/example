export interface Contact {
    id: number;
    invoiceId: number;
    contactTypeId: number;
    name: string;
    address: string;
    phone: string;
    website: string;
    email: string;
    createdDate: Date;
    modifiedDate: Date;
}
