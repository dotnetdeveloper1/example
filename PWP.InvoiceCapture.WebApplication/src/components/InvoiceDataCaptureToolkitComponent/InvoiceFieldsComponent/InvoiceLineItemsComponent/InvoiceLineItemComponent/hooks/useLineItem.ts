import { useCallback } from "react";
import { useDispatch } from "react-redux";
import { LineItemsFieldTypes, LineItemsFieldTypesMap } from "../../../../store/state";
import { IInvoiceLineItem } from "../../../../store/state/IInvoiceLineItem";
import { ReactPropsType, ReactPropType } from "./../../../../../../helperTypes";
import { clearEmptyDataAnnotations, deleteInvoiceItems } from "./../../../../store/InvoiceDataCaptureToolkitStoreSlice";
import { InvoiceLineItemComponent } from "./../InvoiceLineItemComponent";

interface ILineItemHookResult {
    lineItem: IInvoiceLineItem;
    isFocusedInDocument: (fieldType: LineItemsFieldTypes) => boolean;
    onDeleteInvoiceItem: (event: React.MouseEvent) => void;
    onChange: ReactPropType<typeof InvoiceLineItemComponent, "onChange">;
    onBlur: ReactPropType<typeof InvoiceLineItemComponent, "onBlur">;
    onFocus(event: React.FocusEvent): void;
}

export function useLineItem(props: ReactPropsType<typeof InvoiceLineItemComponent>): ILineItemHookResult {
    const { lineItem, lineItemsFieldTypesInFocus } = props;

    const dispatch = useDispatch();

    const onDeleteInvoiceItem = useCallback(
        (event: React.MouseEvent) => {
            dispatch(deleteInvoiceItems({ orderNumber: lineItem.orderNumber }));
            dispatch(clearEmptyDataAnnotations());
        },
        [dispatch, lineItem]
    );
    const onChange = useCallback(
        (event) => {
            props.onChange(event);
        },
        [props]
    );

    const onBlur = useCallback(
        (event) => {
            props.onBlur(event);
        },
        [props]
    );

    const onFocus = useCallback(
        (event: React.FocusEvent) => {
            const inputElement = event.target as HTMLInputElement;
            const lineItemFieldName = inputElement.getAttribute("data-lineitemfieldname");
            if (lineItemFieldName) {
                const lineItemsFieldType = LineItemsFieldTypesMap.get(lineItemFieldName);
                if (lineItemsFieldType) {
                    props.onLineItemFocus(lineItemsFieldType, lineItem.orderNumber);
                }
            }
        },
        [props, lineItem.orderNumber]
    );

    const isFocusedInDocument = (fieldType: LineItemsFieldTypes): boolean => {
        if (!lineItemsFieldTypesInFocus) {
            return false;
        }
        const selectedLineItemFieldType = lineItemsFieldTypesInFocus.find(
            (focusedFieldType) => focusedFieldType.orderNumber === lineItem.orderNumber
        );
        if (selectedLineItemFieldType) {
            const isFieldTypeSelected = selectedLineItemFieldType?.lineItemsFieldTypes?.find(
                (type) => type === fieldType
            );
            return isFieldTypeSelected ? true : false;
        } else {
            return false;
        }
    };

    return {
        lineItem: lineItem,
        isFocusedInDocument: isFocusedInDocument,
        onDeleteInvoiceItem: onDeleteInvoiceItem,
        onChange: onChange,
        onBlur: onBlur,
        onFocus: onFocus
    };
}
