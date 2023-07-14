import moment, { Moment } from "moment";
import { useCallback } from "react";
import { useTranslation } from "react-i18next";
import * as Yup from "yup";
import { FieldType } from "../../../../api/models/InvoiceFields/FieldType";
import { invoiceFieldsSelector, useToolkitSelector } from "../../store/selectors";
import { IInvoiceField, IInvoiceFields } from "../../store/state/IInvoiceFields";
import { ReactPropsType } from "./../../../../helperTypes";
import { cultureSelector } from "./../../store/selectors/index";
import { InvoiceFieldsComponent } from "./../InvoiceFieldsComponent";

interface IValidationSchemaHookResult {
    createValidationSchema: (fields: IInvoiceFields | undefined) => Yup.ObjectSchema | undefined;
}

const requiredMessage = "VALIDATION_MESSAGE->REQUIRED";
const fieldTooLongMessage = "VALIDATION_MESSAGE->TOO_LONG";
const fieldTooShortMessage = "VALIDATION_MESSAGE->TOO_SHORT";
const fieldMaxValueMessage = "VALIDATION_MESSAGE->MAX_VALUE";
const fieldMinValueMessage = "VALIDATION_MESSAGE->MIN_VALUE";
const invalidDateFormatMessage = "VALIDATION_MESSAGE->INVALID_DATE_FORMAT";
const invalidNumberFormatMessage = "VALIDATION_MESSAGE->INVALID_NUMBER_FORMAT";
const lineItemTotalValidationErrorMessage = "VALIDATION_MESSAGE->INVALID_LINE_ITEM_TOTAL";
const lineItemTotalTypeErrorMessage = "VALIDATION_MESSAGE->INVALID_TOTAL_FORMAT";
const lineItemQuantityTypeErrorMessage = "VALIDATION_MESSAGE->INVALID_QUANTITY_FORMAT";
const lineItemPriceTypeErrorMessage = "VALIDATION_MESSAGE->INVALID_PRICE_FORMAT";
const lineItemTotalRequiredErrorMessage = "VALIDATION_MESSAGE->TOTAL_REQUIRED";

const customValidationErrorName = "customValidation";
const lineItemTotalErrorName = "invoiceLineTotalValidation";
const quantityErrorName = "invoiceLineQuantityValidation";
const minDate = "1900-01-01";
const minDateErrorMessage = `Must be between 01/01/1900 and 12/31/9999.`;

export function useValidationSchema(props: ReactPropsType<typeof InvoiceFieldsComponent>): IValidationSchemaHookResult {
    const { t } = useTranslation();
    const currentCulture = useToolkitSelector(cultureSelector) ?? "en-US";
    const invoiceFields = useToolkitSelector(invoiceFieldsSelector)?.invoiceFields ?? [];

    const getValidator = (fieldType: FieldType): any | undefined => {
        switch (fieldType) {
            case FieldType.DateTime:
                return Yup.date()
                    .typeError(t(invalidDateFormatMessage))
                    .min(minDate, minDateErrorMessage)
                    .transform(momentDateTransform);
            case FieldType.Decimal:
                return Yup.number().typeError(t(invalidNumberFormatMessage));
            case FieldType.String:
                return Yup.string();
            default:
                return undefined;
        }
    };

    const createFieldValidationSchema = (schema: Yup.ObjectSchema, field: IInvoiceField): Yup.ObjectSchema<object> => {
        let validator = getValidator(field.fieldType);
        if (field && validator) {
            if (field.isRequired) {
                validator = validator.required(t(requiredMessage));
            }

            if (field.maxLength && field.fieldType === FieldType.String) {
                validator = validator.max(field.maxLength, t(fieldTooLongMessage, { max: field.maxLength }));
            }

            if (field.minLength && field.fieldType === FieldType.String) {
                validator = validator.min(field.minLength, t(fieldTooShortMessage, { min: field.minLength }));
            }
            if (field.maxValue && field.fieldType === FieldType.Decimal) {
                validator = validator.max(field.maxValue, t(fieldMaxValueMessage, { max: field.maxValue }));
            }
            if (field.minValue && field.fieldType === FieldType.Decimal) {
                validator = validator.min(field.minValue, t(fieldMinValueMessage, { min: field.minValue }));
            }
            if (field.customValidationFormula && field.customValidationFormula.length > 0) {
                validator = validator.test(
                    customValidationErrorName,
                    getCustomValidationErrorMessage(field.customValidationFormula),
                    customValidation
                );
            }

            (schema as any)[field.fieldId] = validator;
        }
        return schema;
    };

    const createValidationSchema = (fields: IInvoiceFields | undefined): Yup.ObjectSchema | undefined => {
        if (fields && fields.invoiceFields) {
            const plainFieldsValidationSchema = fields.invoiceFields.reduce(createFieldValidationSchema, {} as any);
            return Yup.object().shape({
                formFields: Yup.object().shape(plainFieldsValidationSchema as any),
                lineItems: Yup.array().of(
                    Yup.object().shape({
                        lineTotal: Yup.number()
                            .typeError(lineItemTotalTypeErrorMessage)
                            .required(lineItemTotalRequiredErrorMessage)
                            .test(lineItemTotalErrorName, lineItemTotalValidationErrorMessage, validateLineItemTotal),
                        quantity: Yup.string()
                            .typeError(lineItemQuantityTypeErrorMessage)
                            .test(quantityErrorName, lineItemQuantityTypeErrorMessage, validateQuantity),
                        price: Yup.number().typeError(lineItemPriceTypeErrorMessage)
                    })
                )
            });
        }

        return undefined;
    };

    function momentDateTransform(value: Moment, originalValue: string): Date {
        moment.locale(currentCulture);
        const dateFormatsArray = [
            moment.localeData().longDateFormat("L"),
            moment.localeData().longDateFormat("l"),
            moment.localeData().longDateFormat("LL"),
            moment.localeData().longDateFormat("LL")
        ];

        if (currentCulture.toUpperCase() === "EN-AU") {
            dateFormatsArray.push("D.M.YYYY");
            dateFormatsArray.push("D-M-YYYY");
        }

        if (currentCulture.toUpperCase() === "EN-US") {
            dateFormatsArray.push("M-D-YYYY");
            dateFormatsArray.push("M.D.YYYY");
        }

        if (currentCulture.toUpperCase() === "EN-US" || currentCulture.toUpperCase() === "EN-AU") {
            dateFormatsArray.push("MMM D, YYYY");
            dateFormatsArray.push("MMMM D, YYYY");
            dateFormatsArray.push("D MMM, YYYY");
            dateFormatsArray.push("D MMMM, YYYY");

            dateFormatsArray.push("MMM D YYYY");
            dateFormatsArray.push("MMMM D YYYY");
            dateFormatsArray.push("D MMM YYYY");
            dateFormatsArray.push("D MMMM YYYY");

            dateFormatsArray.push("DMMMYYYY");
            dateFormatsArray.push("D-MMM-YYYY");
        }
        const fullFormatsArray = dateFormatsArray.concat(getTwoDigitsYearFormats(dateFormatsArray));

        value = moment(originalValue, fullFormatsArray, true);

        return value.isValid() ? value.toDate() : new Date("");
    }

    // Do not remove this function. It used by eval function
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    function getFieldValueById(context: Yup.TestContext, fieldId: string): string {
        const currentField = invoiceFields.find((field) => field.fieldId === fieldId.toString());
        if (currentField && context.parent[fieldId] && context.parent[fieldId] !== "") {
            return context.parent[fieldId];
        }
        return "0";
    }

    function customValidation(this: Yup.TestContext): boolean {
        const fieldId = this.path.replace("formFields.", "");
        if (!fieldId || fieldId.length === 0) {
            throw new Error("Error in custom validation schema");
        }
        const invoiceField = invoiceFields.find((field) => field.fieldId === fieldId);
        if (invoiceField === undefined) {
            throw new Error("Error in custom validation schema");
        }
        if (invoiceField.customValidationFormula === undefined) {
            return true;
        }
        const equality: string = `${invoiceField.customValidationFormula}===[${fieldId}]`;
        const expression = equality
            .replace(/\[/g, "Number(parseFloat(getFieldValueById(this,")
            .replace(/\]/g, ")).toFixed(2))");
        // tslint:disable-next-line: no-eval
        return eval(expression);
    }

    const getFieldNameById = useCallback(
        (fieldId: string): string => {
            if (invoiceFields) {
                const currentField = invoiceFields.find((field) => field.fieldId === fieldId);
                if (currentField) {
                    return currentField.fieldName;
                }
            }
            return "";
        },
        [invoiceFields]
    );

    function replaceFieldIdsWithNames(formula: string): string {
        while (formula.indexOf("[") > -1 && formula.indexOf("]")) {
            const startIndex = formula.indexOf("[");
            const endIndex = formula.indexOf("]");
            const fieldId = formula.substring(startIndex + 1, endIndex);
            formula =
                formula.substring(0, startIndex) +
                " " +
                getFieldNameById(fieldId) +
                " " +
                formula.substring(endIndex + 1);
        }
        return formula;
    }

    function getCustomValidationErrorMessage(formula: string | undefined): string {
        if (formula) {
            return t("VALIDATION_MESSAGE->CUSTOM_VALIDATION", { formula: replaceFieldIdsWithNames(formula) });
        }
        return t("VALIDATION_MESSAGE->DEFAULT");
    }

    return {
        createValidationSchema: createValidationSchema
    };
}

function validateLineItemTotal(this: Yup.TestContext): boolean {
    const quantity = convertHourOrNumberToNumber(this.parent.quantity);
    const price = parseFloat(this.parent.price);
    const lineTotal = parseFloat(this.parent.lineTotal);

    if (!isNaN(quantity) && !isNaN(price) && !isNaN(lineTotal)) {
        return Number(lineTotal.toFixed(2)) === Number((quantity * price).toFixed(2));
    }
    return true;
}

function validateQuantity(this: Yup.TestContext): boolean {
    const stringQty = this.parent.quantity as string;
    if (!stringQty) {
        return true;
    }
    if (stringQty.includes(":")) {
        const splitted = stringQty.split(":");
        if (splitted.some(isNullOrWhitespace)) {
            return false;
        }
        if (splitted.length > 3) {
            return false;
        }
        if (!isHoursValid(splitted[0])) {
            return false;
        }
        for (let i = 1; i < splitted.length; i++) {
            if (!isMinutesOrSecondsValid(splitted[i])) {
                return false;
            }
        }
        return true;
    } else {
        return !isNaN(Number(stringQty));
    }
}

function convertHourOrNumberToNumber(value: string): number {
    if (value && value.includes(":")) {
        const splitted = value.split(":");

        const hours = splitted[0];
        const minutes = splitted[1];
        const convertedMinutes = Number(Number(minutes) / 60).toFixed(2);
        let convertedSeconds = "0";
        if (splitted.length > 2) {
            convertedSeconds = Number(Number(splitted[2]) / 3600).toFixed();
        }

        return Number(hours) + Number(convertedMinutes) + Number(convertedSeconds);
    } else {
        return Number(value);
    }
}

function isHoursValid(value: string): boolean {
    const valueAsNumber = Number(value);
    return !isNaN(valueAsNumber) && valueAsNumber >= 0;
}

function isMinutesOrSecondsValid(value: string): boolean {
    const valueAsNumber = Number(value);
    return !isNaN(valueAsNumber) && valueAsNumber >= 0 && valueAsNumber < 60;
}

function isNullOrWhitespace(input: string): boolean {
    return !input || !input.trim();
}

function getTwoDigitsYearFormats(formats: Array<string>): Array<string> {
    return formats.map((format) => format.replace(/YYYY/g, "YY"));
}
