import { useFormik } from "formik";
import _ from "lodash";
import { useCallback, useEffect, useMemo } from "react";
import { ReactPropsType, ReactPropType } from "../../../../helperTypes";
import { tempLineItemsSelector, useToolkitSelector } from "../../store/selectors";
import { cultureSelector } from "../../store/selectors/index";
import { IInvoiceFields, InvoiceStatus } from "../../store/state";
import { IInvoiceField } from "../../store/state/IInvoiceFields";
import { IInvoiceLineItem } from "../../store/state/IInvoiceLineItem";
import { InvoiceFieldComponent } from "../InvoiceFieldComponent/InvoiceFieldComponent";
import { InvoiceFieldsComponent } from "../InvoiceFieldsComponent";
import { InvoiceLineItemsComponent } from "../InvoiceLineItemsComponent/InvoiceLineItemsComponent";
import { useValidationSchema } from "./useValidationSchema";

export interface IGroupedField {
    groupName: string;
    groupOrder: number;
    fields: IInvoiceField[];
}

export interface IPlainFormFields {
    formFields: any;
    lineItems: IInvoiceLineItem[];
    tableTemporaryLineItems: IInvoiceLineItem[];
}

export interface IFormFields {
    formFields: IGroupedField[];
    lineItems: IInvoiceLineItem[];
    tableTemporaryLineItems: IInvoiceLineItem[];
}

interface IInvoiceFieldsHookResult {
    isStateReady: boolean;
    formState: {
        values: IPlainFormFields;
        errors: any;
        handleBlur: (event: React.FocusEvent) => void;
        handleChange: (eventOrTextValue: React.ChangeEvent | string) => void;
        handleSubmit: (event?: React.FormEvent<HTMLFormElement> | undefined) => void;
    };
    selectedFields: string[];
    isFormFieldsEnabled: boolean;
    formFields: IFormFields;
    onFieldFocus: ReactPropType<typeof InvoiceFieldComponent, "onFocus">;
    onLineItemFieldFocus: ReactPropType<typeof InvoiceLineItemsComponent, "onLineItemFocus">;
}

export function useInvoiceFields(props: ReactPropsType<typeof InvoiceFieldsComponent>): IInvoiceFieldsHookResult {
    const isFormFieldsEnabled = useMemo(() => props.invoiceStatus === InvoiceStatus.PendingReview, [props]);

    const currentCulture = useToolkitSelector(cultureSelector);
    const { createValidationSchema } = useValidationSchema(props);

    const onSubmit = useCallback(
        (formValues: IPlainFormFields) => {
            props.onFieldsSubmit(mapPlainFormFieldsToInvoiceFields(formValues, props.fieldsState));
        },
        [props]
    );

    const formState = useFormik<IPlainFormFields>({
        initialValues: mapInvoiceFieldsToPlainFormFields(props.fieldsState),
        validationSchema: createValidationSchema(props.fieldsState),
        onSubmit: onSubmit,
        validateOnMount: isFormFieldsEnabled
    });

    useEffect(() => {
        formState.validateForm();
    }, [currentCulture]);

    const onFieldFocus = useCallback(
        (fieldId: string) => {
            if (fieldId) {
                props.onFieldFocus(fieldId);
            }
        },
        [props]
    );

    const onLineItemFieldFocus = useCallback(
        (fieldType, orderNumber) => {
            if (fieldType) {
                props.onLineItemFieldFocus(fieldType, orderNumber);
            }
        },
        [props]
    );

    const onFieldBlur = useCallback(
        (event: React.FocusEvent) => {
            formState.handleBlur(event);
            props.onFieldBlur();
            onSubmit(formState.values);
        },
        [props, formState, onSubmit]
    );

    useEffect(() => {
        const newFormFields = mapInvoiceFieldsToPlainFormFields(props.fieldsState);
        if (
            props.fieldsState &&
            (JSON.stringify(formState.initialValues) !== JSON.stringify(newFormFields) ||
                JSON.stringify(formState.initialValues.lineItems) !== JSON.stringify(newFormFields.lineItems))
        ) {
            formState.setValues(newFormFields);
        }
        props.onFormValidationStateChanged(formState.errors);
    }, [formState, props]);

    const tempLineItems = useToolkitSelector(tempLineItemsSelector);
    const isFormEnabled = useMemo(() => {
        return isFormFieldsEnabled && (!tempLineItems || tempLineItems.length === 0);
    }, [isFormFieldsEnabled, tempLineItems]);

    return {
        isStateReady: props.fieldsState !== undefined,
        formState: {
            ...formState,
            handleBlur: onFieldBlur
        },
        selectedFields: props.fieldTypesInFocus,
        isFormFieldsEnabled: isFormEnabled,
        formFields: mapToGroupFormFields(props.fieldsState),
        onFieldFocus: onFieldFocus,
        onLineItemFieldFocus: onLineItemFieldFocus
    };
}

function mapInvoiceFieldsToPlainFormFields(fields: IInvoiceFields | undefined): IPlainFormFields {
    const plainFields = {};
    if (fields && fields.invoiceFields) {
        fields.invoiceFields.forEach((field) => {
            (plainFields as any)[field.fieldId] = field.value || "";
        });
    }
    return {
        formFields: plainFields,
        lineItems: fields ? fields.lineItems : [],
        tableTemporaryLineItems: fields ? fields.tableTemporaryLineItems : []
    };
}

function mapPlainFormFieldsToInvoiceFields(
    plainFields: IPlainFormFields,
    fields: IInvoiceFields | undefined
): IInvoiceFields {
    return {
        invoiceFields: fields
            ? fields.invoiceFields.map((field) => ({
                  ...field,
                  value: plainFields.formFields[field.fieldId] || ""
              }))
            : [],
        lineItems: plainFields.lineItems,
        tableTemporaryLineItems: plainFields.tableTemporaryLineItems
    };
}

function mapToGroupFormFields(fields: IInvoiceFields | undefined): IFormFields {
    return fields
        ? {
              formFields: mapInvoiceFields(fields.invoiceFields),
              lineItems: fields.lineItems,
              tableTemporaryLineItems: fields.tableTemporaryLineItems
          }
        : {
              formFields: [],
              lineItems: [],
              tableTemporaryLineItems: []
          };
}

function mapInvoiceFields(invoiceFields: IInvoiceField[] | undefined): IGroupedField[] {
    return _(invoiceFields)
        .groupBy((field) => field.groupId)
        .map((value, key) => ({
            groupName: value[0].groupName,
            groupOrder: value.find((field) => field.groupId === key)?.groupOrder || 1,
            fields: _.orderBy(value, (field) => field.fieldOrder)
        }))
        .orderBy((groupedField, key) => groupedField.groupOrder)
        .value();
}
