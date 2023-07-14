import { useCallback } from "react";
import { ReactPropsType, ReactPropType } from "./../../../../../helperTypes";
import { InvoiceFieldComponent } from "./../InvoiceFieldComponent";

interface IInvoiceFieldHookResult {
    labelId: string;
    inputType: ReactPropType<typeof InvoiceFieldComponent, "inputType">;
    fieldName: ReactPropType<typeof InvoiceFieldComponent, "fieldName">;
    value: ReactPropType<typeof InvoiceFieldComponent, "value">;
    error: ReactPropType<typeof InvoiceFieldComponent, "error">;
    inFocus: ReactPropType<typeof InvoiceFieldComponent, "inFocus">;
    tooltipText: ReactPropType<typeof InvoiceFieldComponent, "tooltipText">;
    placeholder: ReactPropType<typeof InvoiceFieldComponent, "placeholder">;
    displayName: ReactPropType<typeof InvoiceFieldComponent, "displayName">;
    onChange: ReactPropType<typeof InvoiceFieldComponent, "onChange">;
    onFocus(): void;
    onBlur: ReactPropType<typeof InvoiceFieldComponent, "onBlur">;
}

export function useInvoiceField(props: ReactPropsType<typeof InvoiceFieldComponent>): IInvoiceFieldHookResult {
    const randomLabelId = `invoice-field-${Math.round(Math.random() * 100)}`;

    const { fieldName, fieldId, tooltipText, placeholder, inputType, value, error, inFocus, displayName } = props;

    const onFocus = useCallback(() => {
        props.onFocus(fieldId);
    }, [fieldId, props]);

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

    return {
        labelId: randomLabelId,
        inputType: inputType,
        fieldName: fieldName,
        value: value,
        error: error,
        inFocus: inFocus,
        tooltipText: tooltipText,
        placeholder: placeholder,
        displayName: displayName,
        onFocus: onFocus,
        onChange: onChange,
        onBlur: onBlur
    };
}
