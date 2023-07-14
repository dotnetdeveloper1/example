import classNames from "classnames";
import { FormikErrors } from "formik";
import React from "react";
import { useTranslation } from "react-i18next";
import addIcon from "../../../images/addIcon.svg";
import { IInvoiceLineItem } from "../../store/state/IInvoiceLineItem";
import { InvoiceStatus, ISelectedLineItemsFieldTypes, LineItemsFieldTypes } from "./../../store/state";
import { useLineItems } from "./hooks/useLineItems";
import { InvoiceLineItemComponent } from "./InvoiceLineItemComponent/InvoiceLineItemComponent";
import "./InvoiceLineItemsComponent.scss";

interface InvoiceLineItemsComponentProps {
    values: IInvoiceLineItem[];
    errors: FormikErrors<IInvoiceLineItem>[] | undefined;
    lineItemsFieldTypesInFocus: ISelectedLineItemsFieldTypes[];
    invoiceStatus: InvoiceStatus;
    isFormFieldsEnabled: boolean;
    onChange(event: React.ChangeEvent): void;
    onBlur(event: React.FocusEvent): void;
    onLineItemFocus(fieldType: LineItemsFieldTypes, orderNumber: number): void;
}

export const COMPONENT_NAME = "InvoiceLineItemsComponent";

export const InvoiceLineItemsComponent: React.FunctionComponent<InvoiceLineItemsComponentProps> = (props) => {
    const { t } = useTranslation();
    const {
        isStateReady,
        values,
        isFormFieldsEnabled,
        onAddInvoiceItem,
        onChange,
        onBlur,
        onLineItemFocus
    } = useLineItems(props);

    const addButtonStyles = [`${COMPONENT_NAME}__add`];
    if (!isFormFieldsEnabled) {
        addButtonStyles.push(`${COMPONENT_NAME}__add--disabled`);
    }
    return (
        <div className={`${COMPONENT_NAME}`}>
            {isStateReady &&
                values.map((item: IInvoiceLineItem, index: number) => (
                    <div key={`invoice-line-item-container-${index}`}>
                        <InvoiceLineItemComponent
                            errors={props.errors ? props.errors[index] : undefined}
                            lineItem={item}
                            onBlur={onBlur}
                            onChange={onChange}
                            lineItemsFieldTypesInFocus={props.lineItemsFieldTypesInFocus}
                            disabled={!isFormFieldsEnabled}
                            index={index}
                            onLineItemFocus={onLineItemFocus}
                        />
                    </div>
                ))}
            <div className={classNames(addButtonStyles)} onClick={onAddInvoiceItem}>
                <img alt={t("ADD_TITLE")} srcSet={`${addIcon} 1x`} />
                {t("ADD_NEW_ITEM_CAPS_TITLE")}
            </div>
        </div>
    );
};
