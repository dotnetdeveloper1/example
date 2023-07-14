import { FormikErrors } from "formik";
import { useCallback } from "react";
import { useDispatch } from "react-redux";
import { ReactPropType } from "../../../helperTypes";
import { InvoiceFieldsComponent } from "../InvoiceFieldsComponent";
import {
    assignSelectedLayoutItemsToInvoiceField,
    assignSelectedLayoutLineItemsToInvoiceField,
    clearEmptyDataAnnotations,
    clearInvoiceFieldDataAnnotationSelection,
    clearInvoiceFields,
    getInvoiceFields,
    getInvoiceFieldsValidationErrors,
    selectDocumentLayoutItems,
    selectInvoiceFieldDataAnnotation,
    selectInvoiceLineItemFieldDataAnnotation,
    updateInvoiceFieldsAnnotation
} from "../store/InvoiceDataCaptureToolkitStoreSlice";
import {
    invoiceFieldsInFocusSelector,
    invoiceFieldsSelector,
    invoiceFieldsValidationErrorsSelector,
    lineItemsFieldsInFocusSelector,
    useToolkitSelector
} from "../store/selectors";
import { IInvoiceFields, ISelectedLineItemsFieldTypes } from "../store/state";

interface IInvoiceFieldsStateHookResult {
    invoiceFieldsState: IInvoiceFields | undefined;
    invoiceFieldTypesInFocus: string[];
    lineItemsFieldTypesInFocus: ISelectedLineItemsFieldTypes[];
    onFieldFocus: ReactPropType<typeof InvoiceFieldsComponent, "onFieldFocus">;
    onLineItemFieldFocus: ReactPropType<typeof InvoiceFieldsComponent, "onLineItemFieldFocus">;
    onFieldBlur: ReactPropType<typeof InvoiceFieldsComponent, "onFieldBlur">;
    onFieldsClearAll: ReactPropType<typeof InvoiceFieldsComponent, "onFieldsClearAll">;
    onFieldsSubmit: ReactPropType<typeof InvoiceFieldsComponent, "onFieldsSubmit">;
    onFormValidationStateChanged: (errors: FormikErrors<IInvoiceFields>) => void;
}

export function useInvoiceFieldsState(): IInvoiceFieldsStateHookResult {
    const dispatch = useDispatch();

    const invoiceFieldsState = useToolkitSelector(invoiceFieldsSelector);
    const invoiceFieldTypesInFocus = useToolkitSelector(invoiceFieldsInFocusSelector);
    const lineItemFieldTypesInFocus = useToolkitSelector(lineItemsFieldsInFocusSelector);
    const invoiceFieldsValidationErrors = useToolkitSelector(invoiceFieldsValidationErrorsSelector);

    const onFormValidationStateChanged = useCallback(
        (errors: FormikErrors<IInvoiceFields>) => {
            if (invoiceFieldsValidationErrors !== errors) {
                dispatch(getInvoiceFieldsValidationErrors(errors));
            }
        },
        [dispatch, invoiceFieldsValidationErrors]
    );

    const onFieldsSubmit = useCallback(
        (fields) => {
            dispatch(getInvoiceFields(fields));
            dispatch(updateInvoiceFieldsAnnotation());
            dispatch(clearEmptyDataAnnotations());
        },
        [dispatch]
    );

    const onFieldFocus = useCallback(
        (fieldId) => {
            dispatch(assignSelectedLayoutItemsToInvoiceField(fieldId));
            dispatch(selectInvoiceFieldDataAnnotation(fieldId));
            dispatch(selectDocumentLayoutItems([]));
        },
        [dispatch]
    );

    const onLineItemFieldFocus = useCallback(
        (fieldType, orderNumber) => {
            dispatch(
                assignSelectedLayoutLineItemsToInvoiceField({
                    fieldType: fieldType,
                    orderNumber: orderNumber
                })
            );
            dispatch(
                selectInvoiceLineItemFieldDataAnnotation({
                    fieldType: fieldType,
                    orderNumber: orderNumber
                })
            );
            dispatch(selectDocumentLayoutItems([]));
        },
        [dispatch]
    );

    const onFieldBlur = useCallback(() => {
        dispatch(clearInvoiceFieldDataAnnotationSelection());
    }, [dispatch]);

    const onFieldsClearAll = useCallback(() => {
        dispatch(clearInvoiceFields());
    }, [dispatch]);

    return {
        invoiceFieldsState: invoiceFieldsState,
        invoiceFieldTypesInFocus: invoiceFieldTypesInFocus,
        lineItemsFieldTypesInFocus: lineItemFieldTypesInFocus,
        onFieldFocus: onFieldFocus,
        onLineItemFieldFocus: onLineItemFieldFocus,
        onFieldBlur: onFieldBlur,
        onFieldsClearAll: onFieldsClearAll,
        onFieldsSubmit: onFieldsSubmit,
        onFormValidationStateChanged: onFormValidationStateChanged
    };
}
