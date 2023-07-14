import _ from "lodash";
import { invoicePageApi } from "../../../../api/InvoicePageApiEndpoint";
import { FieldGroup } from "../../../../api/models/InvoiceFields/FieldGroup";
import {
    Annotation,
    DocumentLayoutItem,
    InvoiceAnnotation,
    LineAnnotation
} from "../../../../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { InvoicePage } from "../../../../api/models/InvoiceManagement/InvoicePage";
import { InvoiceProcessingResult } from "../../../../api/models/InvoiceManagement/InvoiceProcessingResult";
import {
    IInvoiceDataAnnotation,
    IInvoiceDocumentPage,
    IInvoiceFieldDataAnnotation,
    IInvoiceFields,
    ILayoutItem
} from "../state";
import { IInvoiceField } from "../state/IInvoiceFields";
import { IInvoiceLineItemAnnotation } from "../state/IInvoiceLineItemAnnotation";
import { LineItemsFieldTypesMap } from "../state/LineItemsFieldTypes";
import { Field } from "./../../../../api/models/InvoiceFields/Field";
import { IInvoiceLineItemDataAnnotation } from "./../state/index";

export class ApiModelMapper {
    constructor(processingResult: InvoiceProcessingResult, invoicePages: InvoicePage[]) {
        if (!processingResult) {
            throw new Error("Processing Result object is empty");
        }

        if (!processingResult.dataAnnotation) {
            throw new Error("Invoice Document Data Annotations object is empty");
        }

        if (!processingResult.dataAnnotation.documentLayoutItems) {
            throw new Error("Document Layout Item collection is empty");
        }

        if (!invoicePages || invoicePages.length === 0) {
            throw new Error("Page collection is empty");
        }

        if (!processingResult.dataAnnotation.invoiceAnnotations) {
            throw new Error("Invoice Data Annotation is undefined");
        }

        this.plainDocumentText = processingResult.dataAnnotation.plainDocumentText || "";
        this.invoicePages = invoicePages;
        this.documentLayoutItems = processingResult.dataAnnotation.documentLayoutItems.map(this.mapLayoutItem);
        this.invoiceDataAnnotations = processingResult.dataAnnotation.invoiceAnnotations;
        this.invoiceLineAnnotations =
            processingResult.dataAnnotation.invoiceLineAnnotations === null
                ? []
                : processingResult.dataAnnotation.invoiceLineAnnotations;
        this.invoicePageLookup = {};
        this.cultureName = processingResult.cultureName;

        this.fillInvoicePageLookup();
    }

    public selectInvoiceFields = (
        fieldAnnotations: IInvoiceFieldDataAnnotation[],
        fieldGroups: FieldGroup[]
    ): IInvoiceFields => {
        return {
            invoiceFields: fieldGroups
                .map((group) => this.selectFields(group, fieldAnnotations))
                .flat()
                .filter((field) => !field.isDeleted),
            lineItems: [],
            tableTemporaryLineItems: []
        };
    };

    public selectDocumentPages = async (): Promise<IInvoiceDocumentPage[]> => {
        const documentPages: IInvoiceDocumentPage[] = await Promise.all(
            _(this.invoicePages)
                .map(this.mapInvoicePage)
                .value()
        );

        return documentPages;
    };

    public selectInvoiceDataAnnotation = (fieldGroups: FieldGroup[]): IInvoiceDataAnnotation => {
        const invoiceDataAnnotation: IInvoiceDataAnnotation = {
            // invoiceFieldsAnnotation: _(this.invoiceDataAnnotations)
            //     .map(this.mapInvoiceFieldAnnotation)
            //     .value(),

            invoiceFieldsAnnotation: fieldGroups.flatMap((group) =>
                group.fields.map((field) => this.mapInvoiceFieldAnnotation(field))
            ),

            invoiceLineItemAnnotation: _(this.invoiceLineAnnotations)
                .map(this.mapInvoiceLineAnnotations)
                .value(),

            plainDocumentText: this.plainDocumentText
        };
        return invoiceDataAnnotation;
    };

    public selectCulture = (): string | undefined => {
        return this.cultureName;
    };

    private selectFields(fieldGroup: FieldGroup, fieldAnnotations: IInvoiceFieldDataAnnotation[]): IInvoiceField[] {
        return fieldGroup.fields.map((field) => ({
            groupId: fieldGroup.id.toString(),
            groupName: fieldGroup.displayName,
            groupOrder: fieldGroup.orderNumber,
            isDeleted: field.isDeleted,
            fieldOrder: field.orderNumber,
            customValidationFormula: field.formula,
            fieldId: field.id.toString(),
            fieldName: field.displayName,
            fieldType: field.type,
            isRequired: field.isRequired,
            maxLength: field.maxLength,
            minLength: field.minLength,
            maxValue: field.maxValue,
            minValue: field.minValue,
            defaultValue: field.defaultValue,

            value:
                fieldAnnotations.find((annotation) => annotation.fieldId === field.id)?.fieldValue ||
                field.defaultValue ||
                ""
        }));
    }

    private mapInvoicePage = async (page: InvoicePage): Promise<IInvoiceDocumentPage> => {
        const pageTemporaryImageLink = await invoicePageApi.pageImageLink(page.id);

        const { startHeight, endHeight } = this.findGlobalPagePosition(page);

        const pageDocumentLayoutItems = _(this.documentLayoutItems)
            .filter(this.isDocumentLayoutDisplayedOnSelectedPage(startHeight, endHeight))
            .map(this.transformLayoutItemToLocalPageCoordinates(startHeight))
            .value();
        return {
            id: page.id,
            number: page.number,
            width: page.width,
            height: page.height,
            imageFileId: page.imageFileId,
            current: false,
            pageImageLink: pageTemporaryImageLink,
            pageLayoutItems: pageDocumentLayoutItems
        };
    };

    private isDocumentLayoutDisplayedOnSelectedPage = (
        startHeight: number,
        endHeight: number
    ): ((layoutItem: DocumentLayoutItem) => boolean) => {
        return (layoutItem) => {
            return layoutItem.topLeft.y > startHeight && layoutItem.topLeft.y < endHeight;
        };
    };

    private transformLayoutItemToLocalPageCoordinates = (
        offsetTop: number
    ): ((layoutItem: DocumentLayoutItem) => ILayoutItem) => {
        return (layoutItem: DocumentLayoutItem): ILayoutItem => {
            return {
                id: layoutItem.id,
                text: layoutItem.text,
                value: layoutItem.value,
                topLeft: { x: layoutItem.topLeft.x, y: layoutItem.topLeft.y - offsetTop },
                bottomRight: {
                    x: layoutItem.bottomRight.x,
                    y: layoutItem.bottomRight.y - offsetTop
                },
                assigned: false,
                selected: false,
                inFocus: undefined
            };
        };
    };

    private findGlobalPagePosition = (
        selectedPage: InvoicePage
    ): {
        startHeight: number;
        endHeight: number;
    } => {
        const previousPages = _(this.getInvoicePageNumbers())
            .filter((pageNumber) => selectedPage.number > pageNumber)
            .map((pageNumber) => this.invoicePageLookup[pageNumber])
            .value();

        let globalPageStartHeight = 0;
        let globalPageEndHeight = selectedPage.height;

        if (previousPages && previousPages.length > 0) {
            globalPageStartHeight = _(previousPages)
                .map((page) => page.height)
                .sum();

            globalPageEndHeight = globalPageStartHeight + selectedPage.height;
        }

        return { startHeight: globalPageStartHeight, endHeight: globalPageEndHeight };
    };

    private fillInvoicePageLookup = (): void => {
        this.invoicePageLookup = {};

        this.invoicePages.forEach((page) => {
            this.invoicePageLookup[page.number] = page;
        });
    };

    private getInvoicePageNumbers = (): number[] => {
        return _(this.invoicePageLookup)
            .keys()
            .map((invoicePage) => parseInt(invoicePage, 10))
            .sort((left, right) => left - right)
            .value();
    };

    private mapInvoiceFieldAnnotation = (field: Field): IInvoiceFieldDataAnnotation => {
        const annotation = this.invoiceDataAnnotations.find(
            (invoiceAnnotation) => invoiceAnnotation.fieldType === field.id.toString()
        );
        return {
            fieldId: field.id.toString(),
            fieldValue: annotation?.fieldValue || field.defaultValue || "",
            userCreated: annotation?.userCreated || false,
            documentLayoutItemIds: annotation?.documentLayoutItemIds || [],
            selected: false
        };
    };

    private mapInvoiceLineAnnotation = (annotation: Annotation): IInvoiceLineItemDataAnnotation => {
        return {
            fieldType: LineItemsFieldTypesMap.get(annotation.fieldType)!,
            fieldValue: annotation.fieldValue,
            userCreated: annotation.userCreated,
            documentLayoutItemIds: annotation.documentLayoutItemIds,
            selected: false
        };
    };
    private mapInvoiceLineAnnotations = (line: LineAnnotation): IInvoiceLineItemAnnotation => {
        return {
            orderNumber: line.orderNumber,
            lineItemAnnotations: line.lineItemAnnotations
                .filter(
                    (annotation) =>
                        !_.isEmpty(annotation) && LineItemsFieldTypesMap.get(annotation.fieldType) !== undefined
                )
                .map((x) => this.mapInvoiceLineAnnotation(x))
        };
    };

    private mapLayoutItem = (item: DocumentLayoutItem): DocumentLayoutItem => {
        const width = this.invoicePages.find((page) => page.number === item.pageNumber)?.width;
        const height = this.invoicePages.find((page) => page.number === item.pageNumber)?.height;

        if (!width) {
            throw new Error("Page width is undefined");
        }
        if (!height) {
            throw new Error("Page height is undefined");
        }

        return {
            topLeft: {
                x: Math.round(item.topLeft.x * width),
                y: Math.round(this.getCurrentHeight(item.pageNumber) + item.topLeft.y * height)
            },
            bottomRight: {
                x: Math.round(item.bottomRight.x * width),
                y: Math.round(this.getCurrentHeight(item.pageNumber) + item.bottomRight.y * height)
            },
            id: item.id,
            pageNumber: item.pageNumber,
            text: item.text,
            value: item.value,
            selected: false
        };
    };

    private getCurrentHeight = (pageNumber: number): number => {
        return this.invoicePages
            .filter((page) => page.number < pageNumber)
            .reduce((sum, current) => sum + current.height, 0);
    };

    private cultureName?: string;
    private invoicePages: InvoicePage[];
    private invoiceDataAnnotations: InvoiceAnnotation[];
    private invoiceLineAnnotations: LineAnnotation[];
    private documentLayoutItems: DocumentLayoutItem[];
    private plainDocumentText: string;
    private invoicePageLookup: { [pageNumber: number]: InvoicePage };
}
