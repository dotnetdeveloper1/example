import { IInvoiceDocumentPage } from "./IInvoiceDocumentPage";
export interface IDocumentView {
    compareBoxesVisible: boolean;
    pageCount: number;
    currentPageNumber: number;
    pages?: IInvoiceDocumentPage[];
    selectedDocumentLayoutItemIds?: string[];
    selectedPlainTextValue?: string;
    selectedAnnotationValue?: string;
}

export const defaultDocumentViewState: IDocumentView = {
    compareBoxesVisible: true,
    pageCount: 0,
    currentPageNumber: 0,
    pages: undefined,
    selectedDocumentLayoutItemIds: undefined,
    selectedPlainTextValue: undefined,
    selectedAnnotationValue: undefined
};
