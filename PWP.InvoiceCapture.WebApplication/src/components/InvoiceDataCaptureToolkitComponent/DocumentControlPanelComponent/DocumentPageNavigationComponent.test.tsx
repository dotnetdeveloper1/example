import { mount } from "enzyme";
import React from "react";
import { EnzymeWrapperType, ReactPropType } from "../../../helperTypes";
import { COMPONENT_NAME, DocumentPageNavigationComponent } from "./DocumentPageNavigationComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        // set second page to enable both nav buttons
        currentPageNumber = 2;

        onChangeSpy = jest.fn((pageNumber: number) => {
            currentPageNumber = pageNumber;
        });

        wrapper = mount(
            <DocumentPageNavigationComponent pageNumber={currentPageNumber} totalPages={3} onPageChange={onChangeSpy} />
        );
    });

    test("renders component", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);
        expect(documentPageNavigationElement.exists()).toBeTruthy();
    });

    test("click on next button should change current page to 3", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);
        documentPageNavigationElement.find(`.${COMPONENT_NAME}__next-button`).simulate("click");
        expect(onChangeSpy).toBeCalledTimes(1);
        expect(currentPageNumber).toBe(3);
    });

    test("click on previous button should change current page to 1", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);
        documentPageNavigationElement.find(`.${COMPONENT_NAME}__previous-button`).simulate("click");
        expect(onChangeSpy).toBeCalledTimes(1);
        expect(currentPageNumber).toBe(1);
    });

    test("double click on previous button should not change current page to 0", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);
        documentPageNavigationElement
            .find(`.${COMPONENT_NAME}__previous-button`)
            .simulate("click")
            .simulate("click");
        expect(onChangeSpy).toBeCalledTimes(2);
        expect(currentPageNumber).toBe(1);
    });

    test("input blur event should change page number from 2 to 3", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);

        documentPageNavigationElement.find(`.${COMPONENT_NAME}__page-navigation-view > input`).simulate("blur", {
            target: {
                value: "3"
            }
        });

        expect(onChangeSpy).toBeCalledTimes(1);
        expect(currentPageNumber).toBe(3);
    });

    test("input blur event should not change page number from 2 to 42", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);

        documentPageNavigationElement.find(`.${COMPONENT_NAME}__page-navigation-view > input`).simulate("blur", {
            target: {
                value: "42"
            }
        });

        expect(onChangeSpy).not.toBeCalled();
        expect(currentPageNumber).toBe(2);
    });

    test("input change event should change page number from 2 to 1", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);

        documentPageNavigationElement.find(`.${COMPONENT_NAME}__page-navigation-view > input`).simulate("change", {
            target: {
                value: "1"
            }
        });

        expect(onChangeSpy).toBeCalledTimes(1);
        expect(currentPageNumber).toBe(1);
    });

    let wrapper: EnzymeWrapperType<typeof DocumentPageNavigationComponent>;
    let onChangeSpy: jest.Mock<ReturnType<ReactPropType<typeof DocumentPageNavigationComponent, "onPageChange">>>;
    let currentPageNumber: number = 0;
});
