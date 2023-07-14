export interface InvoiceDataAnnotationSaveModel {
    dataAnnotation: InvoiceDataAnnotation;
    cultureName: string;
}

export interface InvoiceDataAnnotation {
    plainDocumentText: string;
    documentLayoutItems: DocumentLayoutItem[];
    invoiceAnnotations: InvoiceAnnotation[];
    invoiceLineAnnotations: LineAnnotation[];
}

export interface DocumentLayoutItem {
    id: string;
    text: string;
    value: string;
    pageNumber: number;
    topLeft: Point;
    bottomRight: Point;
    selected: boolean;
}

export interface Point {
    x: number;
    y: number;
}

export interface Row {
    topLeft: Point;
    bottomRight: Point;
}

export interface Column {
    topLeft: Point;
    bottomRight: Point;
}

export interface TableDividerRow {
    y: number;
}

export interface TableDividerColumn {
    x: number;
}

export interface InvoiceAnnotation {
    fieldType: string;
    fieldValue: string;
    userCreated: boolean;
    documentLayoutItemIds: Array<string>;
}

export interface InvoiceProcessingResultsUpdateModel {
    invoiceId: number;
    processingResultId: number;
    dataAnnotation: InvoiceDataAnnotationSaveModel;
}

export interface LineAnnotation {
    orderNumber: number;
    lineItemAnnotations: Annotation[];
}

export interface Annotation {
    fieldType: string;
    fieldValue: string;
    userCreated: boolean;
    documentLayoutItemIds: string[];
}
