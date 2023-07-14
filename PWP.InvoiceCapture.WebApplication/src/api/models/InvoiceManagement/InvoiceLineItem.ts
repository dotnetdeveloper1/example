export interface InvoiceLineItem {
    orderNumber: number;
    number: string;
    description: string;
    quantity?: number;
    price?: number;
    lineTotal: number;
}
