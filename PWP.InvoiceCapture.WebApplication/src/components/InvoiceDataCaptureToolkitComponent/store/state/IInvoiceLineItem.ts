export interface IInvoiceLineItem {
    orderNumber: number;
    number: string;
    description: string;
    quantity: number;
    price: number;
    lineTotal: number;
}

export const defaultInvoiceLineItem: IInvoiceLineItem = {
    orderNumber: 1,
    description: "",
    number: "",
    quantity: "" as any,
    price: "" as any,
    lineTotal: "" as any
};
