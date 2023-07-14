import { useCallback, useMemo } from "react";
import { useDispatch } from "react-redux";
import { invoiceItemsSelector, tempLineItemsSelector, useToolkitSelector } from "../../../store/selectors";
import { IInvoiceLineItem } from "../../../store/state/IInvoiceLineItem";
import { ReactPropsType, ReactPropType } from "./../../../../../helperTypes";
import { assignInvoiceItems } from "./../../../store/InvoiceDataCaptureToolkitStoreSlice";
import { InvoiceStatus } from "./../../../store/state";
import { LineItemsFieldTypes } from "./../../../store/state";
import { InvoiceLineItemsComponent } from "./../InvoiceLineItemsComponent";

interface ILineItemsHookResult {
    values: IInvoiceLineItem[];
    isStateReady: boolean;
    isFormFieldsEnabled: boolean;
    onAddInvoiceItem: (event: React.MouseEvent) => void;
    onChange: ReactPropType<typeof InvoiceLineItemsComponent, "onChange">;
    onBlur: ReactPropType<typeof InvoiceLineItemsComponent, "onBlur">;
    onLineItemFocus: ReactPropType<typeof InvoiceLineItemsComponent, "onLineItemFocus">;
}

export function useLineItems(props: ReactPropsType<typeof InvoiceLineItemsComponent>): ILineItemsHookResult {
    const dispatch = useDispatch();
    const { values } = props;

    const isPendingReview = useMemo(() => props.invoiceStatus === InvoiceStatus.PendingReview, [props]);

    const invoiceItemsState = useToolkitSelector(invoiceItemsSelector);

    const onAddInvoiceItem = useCallback(
        (event: React.MouseEvent) => {
            const itemsCount = invoiceItemsState?.length ?? 0;
            dispatch(assignInvoiceItems({ orderNumber: itemsCount + 1 }));
        },
        [dispatch, invoiceItemsState]
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

    const onLineItemFocus = useCallback(
        (fieldType: LineItemsFieldTypes, orderNumber: number) => {
            props.onLineItemFocus(fieldType, orderNumber);
        },
        [props]
    );

    const tempLineItems = useToolkitSelector(tempLineItemsSelector);
    const isFormEnabled = useMemo(() => {
        return !tempLineItems || tempLineItems?.length === 0;
    }, [tempLineItems]);

    return {
        values: values,
        isFormFieldsEnabled: isFormEnabled && isPendingReview,
        isStateReady: values !== undefined,
        onAddInvoiceItem: onAddInvoiceItem,
        onChange: onChange,
        onBlur: onBlur,
        onLineItemFocus: onLineItemFocus
    };
}
