import { mount } from "enzyme";
import React from "react";
import { EnzymeWrapperType, ReactPropsType, ReactPropType } from "../../../../helperTypes";
import { COMPONENT_NAME, InvoiceFieldComponent } from "./InvoiceFieldComponent";

jest.mock("reactstrap");

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        propsMockObject = { ...props };
        currentInputValue = propsMockObject.value;

        onFocusSpy = jest.fn();
        onBlurSpy = jest.fn();
        onChangeSpy = jest.fn((event) => {
            currentInputValue = event.target.value;
        });

        propsMockObject.onFocus = onFocusSpy;
        propsMockObject.onBlur = onBlurSpy;
        propsMockObject.onChange = onChangeSpy;

        mountInvoiceField();
    });

    test("renders component", () => {
        const invoiceFieldsElement = wrapper.find(InvoiceFieldComponent);
        expect(invoiceFieldsElement.exists()).toBeTruthy();
    });

    test("input should be displayed with initial test value from props", () => {
        const inputValue = wrapper.find(`.${COMPONENT_NAME}__input`).get(0).props.value;

        expect(inputValue).toBe(propsMockObject.value);
    });

    test("input should be displayed with force focus when inFocus props is specified", () => {
        propsMockObject.inFocus = true;

        mountInvoiceField();

        const element = wrapper.find(`.${COMPONENT_NAME}__input--in-focus`);

        expect(element.exists()).toBeTruthy();
    });

    test("input focus event should call onFocus prop when fieldType is valid", () => {
        wrapper.find(`.${COMPONENT_NAME}__input`).simulate("focus");

        expect(onFocusSpy).toBeCalledTimes(1);
    });

    test("input change event should call onChange prop when fieldType is valid", () => {
        wrapper.find(`.${COMPONENT_NAME}__input`).simulate("change", { target: { value: "" } });

        expect(onChangeSpy).toBeCalledTimes(1);
        expect(currentInputValue).toBe("");
    });

    test("input blur event should call onFocus prop when fieldType is valid", () => {
        wrapper.find(`.${COMPONENT_NAME}__input`).simulate("blur");

        expect(onBlurSpy).toBeCalledTimes(1);
    });

    test("input blur event shouldn't change value", () => {
        wrapper.find(`.${COMPONENT_NAME}__input`).simulate("blur", {
            target: {
                value: "123"
            }
        });

        expect(onChangeSpy).not.toBeCalled();
        expect(onBlurSpy).toBeCalled();
        expect(currentInputValue).toBe(propsMockObject.value);
    });
    test("when props don't have validation error, no --invalid modifier should be existed on input", () => {
        propsMockObject.error = "";

        mountInvoiceField();

        const inputElement = wrapper.find(`.${COMPONENT_NAME}__input--invalid`);
        expect(inputElement.exists()).toBeFalsy();
    });

    test("when props don't have validation error, no error message should be displayed", () => {
        expect(wrapper.find(`.${COMPONENT_NAME}__error`).text()).toBe("");
    });

    test("when there is validation error - error message should be visible and input has --invalid class modifier", () => {
        propsMockObject.error = "ERROR";

        mountInvoiceField();

        const labelText = wrapper.find(`.${COMPONENT_NAME}__error`).getDOMNode().textContent;

        expect(wrapper.find(`.${COMPONENT_NAME}__input--invalid`).exists()).toBeTruthy();
        expect(labelText).toBe("ERROR");
    });

    let wrapper: EnzymeWrapperType<typeof InvoiceFieldComponent>;

    let onBlurSpy: jest.Mock<ReturnType<ReactPropType<typeof InvoiceFieldComponent, "onBlur">>> = jest.fn();
    let onChangeSpy: jest.Mock<ReturnType<ReactPropType<typeof InvoiceFieldComponent, "onChange">>> = jest.fn();
    let onFocusSpy: jest.Mock<ReturnType<ReactPropType<typeof InvoiceFieldComponent, "onFocus">>> = jest.fn();

    const props: ReactPropsType<typeof InvoiceFieldComponent> = {
        inputType: "text",
        fieldName: "1",
        fieldId: "1",
        displayName: "name",
        value: "initial test input value",
        error: null,
        tooltipText: undefined,
        inFocus: false,
        onFocus: onFocusSpy,
        onChange: onChangeSpy,
        onBlur: onBlurSpy,
        disabled: false
    };

    let propsMockObject: ReactPropsType<typeof InvoiceFieldComponent> = { ...props };
    let currentInputValue: string = "";

    const mountInvoiceField = () => {
        wrapper = mount(
            <InvoiceFieldComponent
                inputType={propsMockObject.inputType}
                displayName={propsMockObject.displayName}
                fieldId={propsMockObject.fieldId}
                fieldName={propsMockObject.fieldName}
                value={propsMockObject.value}
                error={propsMockObject.error}
                tooltipText={propsMockObject.tooltipText}
                disabled={propsMockObject.disabled}
                inFocus={propsMockObject.inFocus}
                onFocus={propsMockObject.onFocus}
                onChange={propsMockObject.onChange}
                onBlur={propsMockObject.onBlur}
            />
        );
    };
});
