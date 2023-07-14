import { ILayoutItem } from "./ILayoutItem";

export interface IInvoiceDocumentPage {
    readonly id: number;
    readonly number: number;
    readonly width: number;
    readonly height: number;
    readonly imageFileId: string;
    pageImageLink?: string;
    current?: boolean;
    autoScroll?: boolean;
    pageLayoutItems?: ILayoutItem[];
}
