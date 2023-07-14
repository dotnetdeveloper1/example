import { IInvoiceLineItemDataAnnotation } from "../../state";

export function tryAssignSelectedLayoutItems(
    invoiceFieldsAnnotation: IInvoiceLineItemDataAnnotation[],
    fieldType: string,
    selectedAnnotationValue: string,
    selectedDocumentLayoutItemsIds: string[]
): boolean {
    const fieldDataAnnotations = invoiceFieldsAnnotation.filter((annotation) => annotation.fieldType === fieldType);

    if (fieldDataAnnotations && fieldDataAnnotations.length > 0) {
        fieldDataAnnotations.forEach((assignment) => {
            assignment.userCreated = true;
            assignment.fieldValue = selectedAnnotationValue;
            assignment.documentLayoutItemIds = selectedDocumentLayoutItemsIds;
        });

        return true;
    }

    return false;
}
