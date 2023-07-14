import { FormikErrors, getIn } from "formik";
import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import useDynamicRefs from "use-dynamic-refs";
import { IInvoiceFields, InvoiceStatus, ISelectedLineItemsFieldTypes, LineItemsFieldTypes } from "../store/state";
import { useInvoiceFields } from "./hooks";
import { InvoiceFieldComponent } from "./InvoiceFieldComponent/InvoiceFieldComponent";
import "./InvoiceFieldsComponent.scss";
import { InvoiceLineItemsComponent } from "./InvoiceLineItemsComponent/InvoiceLineItemsComponent";

interface InvoiceFieldsComponentProps {
    fieldsState?: IInvoiceFields;
    fieldTypesInFocus: string[];
    lineItemsFieldTypesInFocus: ISelectedLineItemsFieldTypes[];
    invoiceStatus: InvoiceStatus;
    tableSelectionMode: boolean;
    onFieldFocus(fieldId: string): void;
    onLineItemFieldFocus(fieldType: LineItemsFieldTypes, orderNumber: number): void;
    onFieldBlur(): void;
    onFieldsClearAll(): void;
    onFieldsSubmit(fields: IInvoiceFields): void;
    onFormValidationStateChanged(errors: FormikErrors<IInvoiceFields>): void;
}

export const COMPONENT_NAME = "InvoiceFieldsComponent";

export const InvoiceFieldsComponent: React.FunctionComponent<InvoiceFieldsComponentProps> = (props) => {
    const [getRef, setRef] = useDynamicRefs();
    const { t } = useTranslation();

    const {
        isStateReady,
        formState,
        selectedFields,
        onFieldFocus,
        onLineItemFieldFocus,
        isFormFieldsEnabled,
        formFields
    } = useInvoiceFields(props);

    const { values, handleChange, handleBlur, handleSubmit, errors } = formState;

    useEffect(() => {
        if (selectedFields && selectedFields.length > 0) {
            const firstSelectedRef: React.RefObject<HTMLInputElement> | undefined = getRef(selectedFields[0]) as
                | React.RefObject<HTMLInputElement>
                | undefined;
            if (firstSelectedRef) {
                firstSelectedRef.current?.scrollIntoView();
            }
        }
    }, [getRef, selectedFields]);

    const getFieldError = (fieldId: string): string | undefined => {
        return errors && errors.formFields ? errors.formFields[fieldId] : undefined;
    };

    return (
        <>
            {isStateReady && values && (
                <div className={`${COMPONENT_NAME}`}>
                    {formFields && formFields.formFields && (
                        <>
                            <form onSubmit={handleSubmit}>
                                {formFields.formFields.map((group) => (
                                    <div key={group.groupName} className={`${COMPONENT_NAME}__invoice-fields-group`}>
                                        <div className={`${COMPONENT_NAME}__invoice-fields-group-header`}>
                                            {group.groupName}
                                        </div>
                                        {group.fields &&
                                            group.fields.length > 0 &&
                                            group.fields.map((field) => (
                                                <InvoiceFieldComponent
                                                    key={`invoice-field-component-${field.fieldId}`}
                                                    fieldId={field.fieldId}
                                                    inputType="text"
                                                    fieldName={`formFields.${field.fieldId}`}
                                                    displayName={field.fieldName}
                                                    value={values.formFields[field.fieldId]}
                                                    error={getFieldError(field.fieldId)}
                                                    inFocus={selectedFields.indexOf(field.fieldId) > -1}
                                                    onFocus={onFieldFocus}
                                                    onChange={handleChange}
                                                    onBlur={handleBlur}
                                                    disabled={!isFormFieldsEnabled}
                                                    ref={setRef(field.fieldId) as React.RefObject<HTMLInputElement>}
                                                />
                                            ))}
                                    </div>
                                ))}
                                <div className={`${COMPONENT_NAME}__invoice-fields-group`}>
                                    <div className={`${COMPONENT_NAME}__invoice-fields-group-header`}>
                                        {t("INVOICE_FIELDS_TITLE->LINE_ITEMS_HEADER")}
                                    </div>
                                    <InvoiceLineItemsComponent
                                        values={values.lineItems}
                                        isFormFieldsEnabled={isFormFieldsEnabled}
                                        errors={getIn(errors, "lineItems")}
                                        lineItemsFieldTypesInFocus={props.lineItemsFieldTypesInFocus}
                                        invoiceStatus={props.invoiceStatus}
                                        onChange={handleChange}
                                        onLineItemFocus={onLineItemFieldFocus}
                                        onBlur={handleBlur}
                                    />
                                </div>
                            </form>
                        </>
                    )}
                </div>
            )}
        </>
    );
};
