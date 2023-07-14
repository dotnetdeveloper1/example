import { faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React, { forwardRef } from "react";
import { useTranslation } from "react-i18next";
import { UncontrolledTooltip } from "reactstrap";
import { useInvoiceField } from "./hooks";
import "./InvoiceFieldComponent.scss";

interface InvoiceFieldComponentProps {
    inputType: string;
    fieldName: string;
    fieldId: string;
    displayName: string;
    value: any;
    placeholder?: string;
    tooltipText?: string;
    error?: any;
    inFocus: boolean;
    disabled: boolean;
    onFocus(fieldId: string): void;
    onChange(event: React.ChangeEvent): void;
    onBlur(event: React.FocusEvent): void;
}

export const COMPONENT_NAME = "InvoiceFieldComponent";

const InvoiceFieldComponentCore: React.ForwardRefRenderFunction<HTMLInputElement, InvoiceFieldComponentProps> = (
    props,
    ref
) => {
    const { t } = useTranslation();

    const {
        labelId,
        inputType,
        fieldName,
        value,
        error,
        inFocus,
        tooltipText,
        placeholder,
        displayName,
        onFocus,
        onChange,
        onBlur
    } = useInvoiceField(props);

    const inputClassNames = [
        `${COMPONENT_NAME}__input`,
        error ? `${COMPONENT_NAME}__input--invalid` : "",
        inFocus ? `${COMPONENT_NAME}__input--in-focus` : ""
    ];
    return (
        <>
            <div className={`${COMPONENT_NAME}`}>
                <label className={`${COMPONENT_NAME}__label`}>
                    <>{displayName}</>
                    <>{tooltipText && <FontAwesomeIcon icon={faInfoCircle} id={labelId} />}</>
                </label>
                {tooltipText && (
                    <UncontrolledTooltip placement="right" target={labelId}>
                        {tooltipText}
                    </UncontrolledTooltip>
                )}
                <div className={`${COMPONENT_NAME}__input-container`}>
                    <input
                        ref={ref}
                        placeholder={placeholder || t("SELECT_OR_ENTER_DATA_TITLE")}
                        type={inputType}
                        name={fieldName}
                        value={value || ""}
                        className={classNames(inputClassNames)}
                        onFocus={onFocus}
                        onChange={onChange}
                        onBlur={onBlur}
                        disabled={props.disabled}
                        autoComplete="off"
                    />
                </div>
                <div className={`${COMPONENT_NAME}__error`}>{error ? t(error) : ""}</div>
            </div>
        </>
    );
};

export const InvoiceFieldComponent = forwardRef(InvoiceFieldComponentCore);
