import _ from "lodash";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";
import { IInvoiceDocumentPage } from "../state/IInvoiceDocumentPage";
import {
    Annotation,
    DocumentLayoutItem,
    InvoiceAnnotation,
    InvoiceDataAnnotation,
    InvoiceDataAnnotationSaveModel,
    InvoiceProcessingResultsUpdateModel,
    LineAnnotation
} from "./../../../../api/models/InvoiceManagement/InvoiceDataAnnotation";
import { IInvoiceFieldDataAnnotation } from "./../state/IInvoiceFieldDataAnnotation";
import { IInvoiceLineItemAnnotation } from "./../state/IInvoiceLineItemAnnotation";
import { IInvoiceLineItemDataAnnotation } from "./../state/index";
import { LineItemsFieldTypes, LineItemsFieldTypesKeyMap } from "./../state/LineItemsFieldTypes";
export class StateModelMapper {
    constructor(state: IInvoiceDataCaptureToolkitState) {
        if (!state) {
            throw new Error("State object is empty");
        }
        this.state = state;
    }

    public getUpdatedProcessingResults(): InvoiceProcessingResultsUpdateModel {
        return {
            invoiceId: this.state.invoiceId!,
            processingResultId: this.state.cleanProcessingResults!.id,
            dataAnnotation: this.getDataAnnotationSaveModel()
        };
    }

    private getDataAnnotationSaveModel(): InvoiceDataAnnotationSaveModel {
        return {
            dataAnnotation: this.getDataAnnotation(),
            cultureName: this.state.cultureName ?? ""
        };
    }

    public getDataAnnotation(): InvoiceDataAnnotation {
        return {
            plainDocumentText: this.state.invoiceDataAnnotation?.plainDocumentText!,
            documentLayoutItems: this.mapDocumentLayoutItems(this.state.documentView.pages),
            invoiceAnnotations: this.state.invoiceDataAnnotation?.invoiceFieldsAnnotation
                .filter((annotation) => annotation.fieldValue !== "")
                .map(this.mapInvoiceFieldAnnotation)!,
            invoiceLineAnnotations: this.clearEmptyLinesDataAnnotations(
                this.state.invoiceDataAnnotation?.invoiceLineItemAnnotation
            )?.map(this.mapInvoiceLinesAnnotation)!
        };
    }

    private clearEmptyLinesDataAnnotations = (invoiceLineItemAnnotations?: IInvoiceLineItemAnnotation[]) => {
        const updatedAnnotations: IInvoiceLineItemAnnotation[] = [];
        if (invoiceLineItemAnnotations) {
            invoiceLineItemAnnotations.forEach((annotations) => {
                let lineItemAnnotations: IInvoiceLineItemDataAnnotation[] = annotations.lineItemAnnotations;
                lineItemAnnotations = lineItemAnnotations.filter((annotation) => annotation.fieldValue !== "");
                updatedAnnotations.push({
                    orderNumber: annotations.orderNumber,
                    lineItemAnnotations: lineItemAnnotations
                });
            });
        }
        return updatedAnnotations;
    };

    private mapDocumentLayoutItems = (pages: IInvoiceDocumentPage[] | undefined): DocumentLayoutItem[] => {
        return _.flatten(
            pages?.map((page) =>
                page.pageLayoutItems?.map((layout) => {
                    const width = this.state.documentView.pages!.find(
                        (documentViewPage) => page.number === documentViewPage.number
                    )?.width;
                    const height = this.state.documentView.pages!.find(
                        (documentViewPage) => page.number === documentViewPage.number
                    )?.height;

                    if (!width) {
                        throw new Error("Page width is undefined");
                    }
                    if (!height) {
                        throw new Error("Page height is undefined");
                    }

                    return {
                        id: layout.id,
                        text: layout.text,
                        value: layout.value,
                        pageNumber: page.number,
                        topLeft: {
                            x: Number((layout.topLeft.x / width).toFixed(6)),
                            y: Number((layout.topLeft.y / height).toFixed(6))
                        },
                        bottomRight: {
                            x: Number((layout.bottomRight.x / width).toFixed(6)),
                            y: Number((layout.bottomRight.y / height).toFixed(6))
                        }
                    };
                })
            )
        ) as DocumentLayoutItem[];
    };

    private mapInvoiceFieldAnnotation = (annotation: IInvoiceFieldDataAnnotation): InvoiceAnnotation => {
        // TODO: remove whitespaces from string type annotation value
        return {
            fieldType: annotation.fieldId,
            fieldValue: annotation.fieldValue,
            userCreated: annotation.userCreated,
            documentLayoutItemIds: annotation.documentLayoutItemIds
        };
    };

    private mapInvoiceLinesAnnotation = (annotation: IInvoiceLineItemAnnotation): LineAnnotation => {
        return {
            orderNumber: annotation.orderNumber,
            lineItemAnnotations: annotation.lineItemAnnotations.map(this.mapInvoiceLineAnnotation)
        };
    };

    private mapInvoiceLineAnnotation = (annotation: IInvoiceLineItemDataAnnotation): Annotation => {
        let fieldValue = annotation.fieldValue;
        if (
            annotation.fieldType === LineItemsFieldTypes.lineTotal ||
            annotation.fieldType === LineItemsFieldTypes.price ||
            annotation.fieldType === LineItemsFieldTypes.quantity
        ) {
            fieldValue = fieldValue.split(" ").join("");
        }

        return {
            fieldType: LineItemsFieldTypesKeyMap.get(annotation.fieldType)!,
            fieldValue: fieldValue,
            userCreated: annotation.userCreated,
            documentLayoutItemIds: annotation.documentLayoutItemIds
        };
    };

    private readonly state: IInvoiceDataCaptureToolkitState;
}
