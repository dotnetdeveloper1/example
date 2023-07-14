import { mount } from "enzyme";
import React from "react";
import { Provider } from "react-redux";
import { EnzymeWrapperType, ReactPropsType } from "../../../helperTypes";
import { rootStore } from "../../../store/configuration";
import { COMPONENT_NAME, FieldAssignmentMenuComponent } from "./FieldAssignmentMenuComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        onAssignItemsToInvoiceFieldSpy = jest.fn();
        onCloseSpy = jest.fn();

        propsMockObject = { ...props, onAssignItems: onAssignItemsToInvoiceFieldSpy, onClose: onCloseSpy };

        mountFieldAssignmentMenu();
    });

    test("renders component", () => {
        const fieldAssignmentMenuElement = wrapper.find(FieldAssignmentMenuComponent);
        expect(fieldAssignmentMenuElement.exists()).toBeTruthy();
    });

    test("renders empty component when isOpen is false", () => {
        propsMockObject.isOpen = false;

        mountFieldAssignmentMenu();

        const fieldAssignmentMenuElement = wrapper.find(`.${COMPONENT_NAME}`);
        expect(fieldAssignmentMenuElement.exists()).toBeFalsy();
    });

    test("renders root component element when isOpen is true", () => {
        propsMockObject.isOpen = true;
        propsMockObject.top = 10;
        propsMockObject.left = 20;

        mountFieldAssignmentMenu();

        const fieldAssignmentMenuElement = wrapper.find(`.${COMPONENT_NAME}`);
        expect(fieldAssignmentMenuElement.exists()).toBeTruthy();
        expect(fieldAssignmentMenuElement.get(0).props.style.top).toBe(propsMockObject.top);
        expect(fieldAssignmentMenuElement.get(0).props.style.left).toBe(propsMockObject.left);
    });

    test("renders context menu header with selected layout items count and selected plain text", () => {
        propsMockObject.isOpen = true;
        propsMockObject.selectedPlainText = "Sample Plain Text";
        propsMockObject.selectedLayoutItemsCount = 2;

        mountFieldAssignmentMenu();

        const fieldAssignmentMenuHeaderLabelElement = wrapper.find(`.${COMPONENT_NAME}__header-label`).getDOMNode();
        expect(fieldAssignmentMenuHeaderLabelElement).toBeDefined();
        expect(fieldAssignmentMenuHeaderLabelElement.textContent).toContain(
            `${propsMockObject.selectedLayoutItemsCount} CONTEXT_MENU_HEADER->ITEMS`
        );

        const fieldAssignmentMenuHeaderBodyElement = wrapper.find(`.${COMPONENT_NAME}__header-body`).getDOMNode();
        expect(fieldAssignmentMenuHeaderBodyElement).toBeDefined();
        expect(fieldAssignmentMenuHeaderBodyElement.textContent).toBe(propsMockObject.selectedPlainText);
    });

    test("renders context menu with __fields-header", () => {
        propsMockObject.isOpen = true;

        mountFieldAssignmentMenu();

        const fieldAssignmentMenuFieldsHeaderElement = wrapper.find(`.${COMPONENT_NAME}__fields-header`);
        expect(fieldAssignmentMenuFieldsHeaderElement.exists()).toBeTruthy();
        expect(fieldAssignmentMenuFieldsHeaderElement.getDOMNode().textContent).toContain(
            "CONTEXT_MENU_HEADER->ASSIGN_DATA"
        );
    });
    /*
    test("renders context menu with __fields-group and __field items (14 elements)", () => {
        propsMockObject.isOpen = true;

        mountFieldAssignmentMenu();

        const fieldAssignmentMenuFieldsGroupElement = wrapper.find(`.${COMPONENT_NAME}__fields-group`);
        expect(fieldAssignmentMenuFieldsGroupElement.exists()).toBeTruthy();
        expect(fieldAssignmentMenuFieldsGroupElement.find(`.${COMPONENT_NAME}__field`).length).toBe(14);
    });
*/
    test("opens context menu aligned to #root element box bounds when top position from mouse position place menu out of view", () => {
        const rootBoxHeight = 600;
        const contextMenuHeight = 200;
        const topOffsetFromCursor = 500;

        (document as any).getElementById = jest.fn(() => ({
            getBoundingClientRect: jest.fn(() => ({
                x: 0,
                y: 0,
                width: 0,
                height: rootBoxHeight,
                top: 0,
                left: 0
            }))
        }));

        Element.prototype.getBoundingClientRect = jest.fn(() => ({
            x: 0,
            y: 0,
            width: 0,
            height: contextMenuHeight,
            top: 0,
            left: 0
        })) as any;

        propsMockObject.isOpen = true;
        propsMockObject.top = topOffsetFromCursor;

        mountFieldAssignmentMenu();

        const contextMenuElement = wrapper.find(`.${COMPONENT_NAME}`);

        expect(contextMenuElement.exists()).toBeTruthy();
        expect(contextMenuElement.get(0).props.style.top).toBe(rootBoxHeight - contextMenuHeight);
    });

    test("renders context menu with (top,left) computed styles from props when #root element contains necessary space", () => {
        const rootBoxHeight = 600;
        const contextMenuHeight = 200;
        const topOffsetFromCursor = 300;

        (document as any).getElementById = jest.fn(() => ({
            getBoundingClientRect: jest.fn(() => ({
                x: 0,
                y: 0,
                width: 0,
                height: rootBoxHeight,
                top: 0,
                left: 0
            }))
        }));

        Element.prototype.getBoundingClientRect = jest.fn(() => ({
            x: 0,
            y: 0,
            width: 0,
            height: contextMenuHeight,
            top: 0,
            left: 0
        })) as any;

        propsMockObject.isOpen = true;
        propsMockObject.top = topOffsetFromCursor;

        mountFieldAssignmentMenu();

        const contextMenuElement = wrapper.find(`.${COMPONENT_NAME}`);

        expect(contextMenuElement.exists()).toBeTruthy();
        expect(contextMenuElement.get(0).props.style.top).toBe(propsMockObject.top);
        expect(contextMenuElement.get(0).props.style.left).toBe(propsMockObject.left);
    });

    test("close context menu when global click event is received with out of context menu placement coordinates", () => {
        const contextMenuHeight = 200;
        const contextMenuWidth = 300;
        const topOffset = 300;
        const leftOffset = 200;

        const eventMap: { [eventType: string]: any } = {};

        window.addEventListener = jest.fn((eventType, handler) => {
            eventMap[eventType] = handler;
        });

        Element.prototype.getBoundingClientRect = jest.fn(() => ({
            x: leftOffset,
            y: topOffset,
            width: contextMenuWidth,
            height: contextMenuHeight,
            top: topOffset,
            left: leftOffset
        })) as any;

        propsMockObject.isOpen = true;

        mountFieldAssignmentMenu();

        eventMap["mousedown"]({
            pageX: leftOffset - 1,
            pageY: topOffset - 1
        });

        expect(onCloseSpy).toBeCalledTimes(1);
    });
    /*
    test("all available __field items execute field assignment for selected Invoice Field on menu item mouse click", () => {
        onAssignItemsToInvoiceFieldSpy = jest.fn((fieldType: InvoiceFieldTypes) => {
            expect(Array.from(InvoiceFieldTypesMap.values())).toContain(fieldType);
        });

        propsMockObject.isOpen = true;
        propsMockObject.selectedLayoutItemsCount = 1;
        propsMockObject.onAssignItems = onAssignItemsToInvoiceFieldSpy;
        mountFieldAssignmentMenu();

        const fieldAssignmentMenuFieldsElements = wrapper.find(`.${COMPONENT_NAME}__field`);

        fieldAssignmentMenuFieldsElements.forEach((fieldElement) => {
            fieldElement.simulate("click");
        });

        expect(onAssignItemsToInvoiceFieldSpy).toBeCalledTimes(14);
        expect(onCloseSpy).toBeCalledTimes(14);
    });
*/
    let wrapper: EnzymeWrapperType<typeof FieldAssignmentMenuComponent>;

    let onAssignItemsToInvoiceFieldSpy = jest.fn();
    let onCloseSpy = jest.fn();

    const props: ReactPropsType<typeof FieldAssignmentMenuComponent> = {
        selectedPlainText: "",
        selectedLayoutItemsCount: 0,
        isOpen: false,
        left: 0,
        top: 0,
        onAssignItems: jest.fn(),
        onAssignLineItems: jest.fn(),
        onClose: jest.fn()
    };

    let propsMockObject: ReactPropsType<typeof FieldAssignmentMenuComponent> = { ...props };

    const mountFieldAssignmentMenu = () => {
        wrapper = mount(
            <Provider store={rootStore}>
                <FieldAssignmentMenuComponent {...propsMockObject} />
            </Provider>
        );
    };
});
