import classNames from "classnames";
import { FormikErrors, getIn } from "formik";
import React from "react";
import { useTranslation } from "react-i18next";
import { ISelectedLineItemsFieldTypes, LineItemsFieldTypes, LineItemsFieldTypesKeyMap } from "../../../store/state";
import { IInvoiceLineItem } from "../../../store/state/IInvoiceLineItem";
import deleteIcon from "./deleteIcon.svg";
import { useLineItem } from "./hooks/useLineItem";
import "./InvoiceLineItemComponent.scss";

interface InvoiceLineItemComponentProps {
    lineItem: IInvoiceLineItem;
    errors: FormikErrors<IInvoiceLineItem> | undefined;
    lineItemsFieldTypesInFocus: ISelectedLineItemsFieldTypes[];
    disabled: boolean;
    index: number;
    onChange(event: React.ChangeEvent): void;
    onBlur(event: React.FocusEvent): void;
    onLineItemFocus(fieldType: LineItemsFieldTypes, orderNumber: number): void;
}

export const COMPONENT_NAME = "InvoiceLineItemComponent";

export const InvoiceLineItemComponent: React.FunctionComponent<InvoiceLineItemComponentProps> = (props) => {
    const { t } = useTranslation();
    const { lineItem, onDeleteInvoiceItem, onBlur, onChange, onFocus, isFocusedInDocument } = useLineItem(props);

    const getInputFocusStyle = (lineItemsFieldTypes: LineItemsFieldTypes): string => {
        return isFocusedInDocument(lineItemsFieldTypes) ? `${COMPONENT_NAME}__input--in-focus` : "";
    };

    const deleteButtonStyles = [`${COMPONENT_NAME}__delete`];
    const descriptionInputStyles = [
        `${COMPONENT_NAME}__big-input`,
        getInputFocusStyle(LineItemsFieldTypes.description)
    ];
    const itemNumberInputStyles = [`${COMPONENT_NAME}__input`, getInputFocusStyle(LineItemsFieldTypes.number)];
    const totalPriceInputStyles = [`${COMPONENT_NAME}__input`, getInputFocusStyle(LineItemsFieldTypes.lineTotal)];
    const quantityInputStyles = [`${COMPONENT_NAME}__input`, getInputFocusStyle(LineItemsFieldTypes.quantity)];
    const unitPriceInputStyles = [`${COMPONENT_NAME}__input`, getInputFocusStyle(LineItemsFieldTypes.price)];

    const totalPriceErrors = getIn(props.errors, LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.lineTotal)!);
    const quantityErrors = getIn(props.errors, LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.quantity)!);
    const unitPriceErrors = getIn(props.errors, LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.price)!);

    if (props.disabled) {
        deleteButtonStyles.push(`${COMPONENT_NAME}__delete--disabled`);
    }

    if (totalPriceErrors) {
        totalPriceInputStyles.push(`${COMPONENT_NAME}__input--invalid`);
    }
    if (quantityErrors) {
        quantityInputStyles.push(`${COMPONENT_NAME}__input--invalid`);
    }
    if (unitPriceErrors) {
        unitPriceInputStyles.push(`${COMPONENT_NAME}__input--invalid`);
    }

    return (
        <div className={`${COMPONENT_NAME}__container`}>
            <div className={`${COMPONENT_NAME}__line-container`}>
                <div className={`${COMPONENT_NAME}__first-group-container`}>
                    <label className={`${COMPONENT_NAME}__id-label`}>
                        {`${t("ITEM_CAPS_TITLE")} `}
                        {lineItem.orderNumber}
                    </label>
                </div>
                <div className={`${COMPONENT_NAME}__second-group-container`}>
                    <input
                        name={`lineItems[${props.index}].${LineItemsFieldTypesKeyMap.get(
                            LineItemsFieldTypes.description
                        )}`}
                        className={classNames(descriptionInputStyles)}
                        data-lineitemfieldname={LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.description)}
                        onBlur={onBlur}
                        onChange={onChange}
                        type="text"
                        value={props.lineItem.description}
                        onFocus={onFocus}
                        disabled={props.disabled}
                        autoComplete="off"
                        placeholder={t("SELECT_OR_ENTER_DATA_TITLE")}
                    />
                </div>
            </div>
            <div className={`${COMPONENT_NAME}__line-container`}>
                <div className={`${COMPONENT_NAME}__first-group-container`}>
                    <label className={`${COMPONENT_NAME}__label`}>Number#</label>
                    <input
                        data-lineitemfieldname={LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.number)}
                        name={`lineItems[${props.index}].${LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.number)}`}
                        className={classNames(itemNumberInputStyles)}
                        onBlur={onBlur}
                        onChange={onChange}
                        type="text"
                        value={props.lineItem.number}
                        onFocus={onFocus}
                        disabled={props.disabled}
                        autoComplete="off"
                    />
                </div>
                <div className={`${COMPONENT_NAME}__second-group-container`}>
                    <div className={`${COMPONENT_NAME}__regular-input`}>
                        <label className={`${COMPONENT_NAME}__label`}>Qty</label>
                        <input
                            name={`lineItems[${props.index}].${LineItemsFieldTypesKeyMap.get(
                                LineItemsFieldTypes.quantity
                            )}`}
                            data-lineitemfieldname={LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.quantity)}
                            className={classNames(quantityInputStyles)}
                            onBlur={onBlur}
                            onChange={onChange}
                            type="text"
                            value={props.lineItem.quantity}
                            onFocus={onFocus}
                            disabled={props.disabled}
                            autoComplete="off"
                        />
                    </div>
                    <div className={`${COMPONENT_NAME}__regular-input`}>
                        <label className={`${COMPONENT_NAME}__label`}>Unit Price</label>
                        <input
                            name={`lineItems[${props.index}].${LineItemsFieldTypesKeyMap.get(
                                LineItemsFieldTypes.price
                            )}`}
                            data-lineitemfieldname={LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.price)}
                            className={classNames(unitPriceInputStyles)}
                            onBlur={props.onBlur}
                            onChange={props.onChange}
                            type="text"
                            value={props.lineItem.price}
                            onFocus={onFocus}
                            disabled={props.disabled}
                            autoComplete="off"
                        />
                    </div>
                    <div className={`${COMPONENT_NAME}__regular-input`}>
                        <label className={`${COMPONENT_NAME}__label`}>Line Total</label>
                        <input
                            name={`lineItems[${props.index}].${LineItemsFieldTypesKeyMap.get(
                                LineItemsFieldTypes.lineTotal
                            )}`}
                            data-lineitemfieldname={LineItemsFieldTypesKeyMap.get(LineItemsFieldTypes.lineTotal)}
                            className={classNames(totalPriceInputStyles)}
                            onBlur={props.onBlur}
                            onChange={props.onChange}
                            type="text"
                            value={props.lineItem.lineTotal}
                            onFocus={onFocus}
                            disabled={props.disabled}
                            autoComplete="off"
                        />
                    </div>
                    <div className={`${COMPONENT_NAME}__delete_container`}>
                        <div className={classNames(deleteButtonStyles)} onClick={onDeleteInvoiceItem}>
                            <img
                                alt={t("DELETE_TITLE")}
                                srcSet={`${deleteIcon} 1x`}
                                className={`${COMPONENT_NAME}__center`}
                            />
                        </div>
                    </div>
                </div>
            </div>
            <div className={`${COMPONENT_NAME}__last_line_container`}>
                <div className={`${COMPONENT_NAME}__error`}>
                    {[t(quantityErrors), t(unitPriceErrors), t(totalPriceErrors)].filter(Boolean).join("\n")}
                </div>
            </div>
        </div>
    );
};
