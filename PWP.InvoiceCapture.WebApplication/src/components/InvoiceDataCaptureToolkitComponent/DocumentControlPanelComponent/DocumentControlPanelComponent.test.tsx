import { mount } from "enzyme";
import React from "react";
import { Provider } from "react-redux";
import { EnzymeWrapperType, ReactPropsType, ReactPropType } from "../../../helperTypes";
import { rootStore } from "../../../store/configuration";
import { InvoiceStatus } from "../store/state";
import { COMPONENT_NAME, DocumentControlPanelComponent } from "./DocumentControlPanelComponent";
import { DocumentPageNavigationComponent } from "./DocumentPageNavigationComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        onToggleCompareBoxesSpy.mockClear();
        propsMockObject.onToggleCompareBoxes = onToggleCompareBoxesSpy;
        onPageChangeSpy.mockClear();
        propsMockObject.onPageChange = onPageChangeSpy;

        propsMockObject.compareBoxes = false;
        propsMockObject.currentPageNumber = 1;
        propsMockObject.tableSelectionMode = false;

        mountDocumentControlPanel();
    });

    test("renders component", () => {
        const documentControlPanelElement = wrapper.find(DocumentControlPanelComponent);
        expect(documentControlPanelElement.exists()).toBeTruthy();
    });

    test("renders ToggleCompareBoxesButton with disabled state and Show Compare Boxes text when compareBoxes prop is false", () => {
        const toggleCompareBoxesButtonElement = wrapper.find(
            `.${COMPONENT_NAME}__show-annotations.${COMPONENT_NAME}__show-annotations--disabled`
        );
        expect(toggleCompareBoxesButtonElement.exists()).toBeTruthy();
        expect(toggleCompareBoxesButtonElement.text()).toContain("BUTTON_TITLE->SHOW_COMPARE_BOXES");
    });

    test("renders ToggleCompareBoxesButton with enabled state and Hide Compare Boxes text when compareBoxes prop is true", () => {
        propsMockObject.compareBoxes = true;

        mountDocumentControlPanel();

        const toggleCompareBoxesButtonElement = wrapper.find(
            `.${COMPONENT_NAME}__show-annotations.${COMPONENT_NAME}__show-annotations--enabled`
        );
        expect(toggleCompareBoxesButtonElement.exists()).toBeTruthy();
        expect(toggleCompareBoxesButtonElement.text()).toContain("BUTTON_TITLE->HIDE_COMPARE_BOXES");
    });

    test("ToggleCompareBoxesButton state should switch when the user click on the button", () => {
        const toggleCompareBoxesButtonElement = wrapper.find(`.${COMPONENT_NAME}__show-annotations`);

        toggleCompareBoxesButtonElement.simulate("click");

        expect(onToggleCompareBoxesSpy).toBeCalled();
        expect(propsMockObject.compareBoxes).toBeTruthy();

        mountDocumentControlPanel();

        expect(
            wrapper.find(`.${COMPONENT_NAME}__show-annotations.${COMPONENT_NAME}__show-annotations--enabled`).exists()
        ).toBeTruthy();
    });

    test("move the current page number to the second one when the user click on next button", () => {
        const documentPageNavigationElement = wrapper.find(DocumentPageNavigationComponent);
        expect(documentPageNavigationElement.exists()).toBeTruthy();

        const nextButton = documentPageNavigationElement.find(".DocumentPageNavigationComponent__next-button");
        nextButton.simulate("click");

        expect(onPageChangeSpy).toBeCalled();
        expect(propsMockObject.currentPageNumber).toBe(2);
    });

    let wrapper: EnzymeWrapperType<typeof DocumentControlPanelComponent>;

    const onPageChangeSpy: jest.Mock<ReturnType<
        ReactPropType<typeof DocumentControlPanelComponent, "onPageChange">
    >> = jest.fn((pageNumber) => {
        propsMockObject.currentPageNumber = pageNumber;
    });

    const onToggleCompareBoxesSpy: jest.Mock<ReturnType<
        ReactPropType<typeof DocumentControlPanelComponent, "onToggleCompareBoxes">
    >> = jest.fn(() => {
        propsMockObject.compareBoxes = !propsMockObject.compareBoxes;
    });

    const propsMockObject: ReactPropsType<typeof DocumentControlPanelComponent> = {
        currentPageNumber: 1,
        pageCount: 2,
        compareBoxes: false,
        onToggleCompareBoxes: onToggleCompareBoxesSpy,
        onPageChange: onPageChangeSpy,
        tableSelectionMode: false,
        invoiceStatus: InvoiceStatus.PendingReview
    };

    const mountDocumentControlPanel = () => {
        wrapper = mount(
            <Provider store={rootStore}>
                <DocumentControlPanelComponent
                    currentPageNumber={propsMockObject.currentPageNumber}
                    pageCount={propsMockObject.pageCount}
                    compareBoxes={propsMockObject.compareBoxes}
                    onToggleCompareBoxes={propsMockObject.onToggleCompareBoxes}
                    onPageChange={propsMockObject.onPageChange}
                    tableSelectionMode={propsMockObject.tableSelectionMode}
                    invoiceStatus={propsMockObject.invoiceStatus}
                />
            </Provider>
        );
    };
});
