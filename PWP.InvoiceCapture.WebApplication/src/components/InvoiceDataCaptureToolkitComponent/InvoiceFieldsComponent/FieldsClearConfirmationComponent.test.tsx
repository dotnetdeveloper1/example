import { mount } from "enzyme";
import React from "react";
import { EnzymeWrapperType, ReactPropsType } from "../../../helperTypes";
import { COMPONENT_NAME, FieldsClearConfirmationComponent } from "./FieldsClearConfirmationComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        onCancelSpy.mockClear();
        onConfirmSpy.mockClear();

        mountModal();
    });

    test("renders component", () => {
        const componentElement = wrapper.find(FieldsClearConfirmationComponent);
        expect(componentElement.exists()).toBeTruthy();
    });

    test("should not display modal window when isOpen prop is false", () => {
        propsMockObject.isOpen = false;

        mountModal();

        const rootElement = wrapper.find(`.${COMPONENT_NAME}`);
        expect(rootElement.exists()).toBeFalsy();
    });

    test("should display modal window when isOpen prop is true", () => {
        propsMockObject.isOpen = true;

        mountModal();

        const rootElement = wrapper.find(`.${COMPONENT_NAME}`);
        expect(rootElement.exists()).toBeTruthy();
    });

    test("should call onConfirm prop when Confirm button will be pressed", () => {
        propsMockObject.isOpen = true;

        mountModal();

        wrapper.find(`.${COMPONENT_NAME}__confirm`).simulate("click");

        expect(onConfirmSpy).toBeCalledTimes(1);
        expect(onCancelSpy).toBeCalledTimes(0);
    });

    test("should call onCancel prop when Cancel button will be pressed", () => {
        propsMockObject.isOpen = true;

        mountModal();

        wrapper.find(`.${COMPONENT_NAME}__cancel`).simulate("click");

        expect(onConfirmSpy).toBeCalledTimes(0);
        expect(onCancelSpy).toBeCalledTimes(1);
    });

    let wrapper: EnzymeWrapperType<typeof FieldsClearConfirmationComponent>;

    const onCancelSpy = jest.fn();
    const onConfirmSpy = jest.fn();

    const props: ReactPropsType<typeof FieldsClearConfirmationComponent> = {
        isOpen: false,
        onCancel: onCancelSpy,
        onConfirm: onConfirmSpy
    };

    const propsMockObject: ReactPropsType<typeof FieldsClearConfirmationComponent> = {
        ...props
    };

    const mountModal = () => {
        wrapper = mount(
            <FieldsClearConfirmationComponent
                isOpen={propsMockObject.isOpen}
                onCancel={propsMockObject.onCancel}
                onConfirm={propsMockObject.onConfirm}
            />
        );
    };
});
