import { mount } from "enzyme";
import React from "react";
import { EnzymeWrapperType, ReactPropsType } from "../../../helperTypes";
import { COMPONENT_NAME, DocumentLayoutItemComponent } from "./DocumentLayoutItemComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        onSelectedItemContextMenuSpy.mockClear();
        propsMockObject = { ...props };

        mountDocumentLayoutItem();
    });

    test("renders component", () => {
        const documentLayoutItemComponentElement = wrapper.find(DocumentLayoutItemComponent);
        expect(documentLayoutItemComponentElement.exists()).toBeTruthy();
    });

    test("renders disabled layout item when layout item is assigned, but not displayed", () => {
        propsMockObject.displayed = false;
        propsMockObject.assigned = true;

        mountDocumentLayoutItem();

        const disabledDocumentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--disabled`);
        expect(disabledDocumentLayoutItemElement.exists()).toBeTruthy();
    });

    test("renders disabled layout item when layout item is selected, but not displayed", () => {
        propsMockObject.displayed = false;
        propsMockObject.selected = true;

        mountDocumentLayoutItem();

        const disabledDocumentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--disabled`);
        expect(disabledDocumentLayoutItemElement.exists()).toBeTruthy();
    });

    test("renders enabled layout item when layout item is not assigned, but displayed", () => {
        propsMockObject.displayed = true;
        propsMockObject.assigned = false;

        mountDocumentLayoutItem();

        const disabledDocumentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--enabled`);
        expect(disabledDocumentLayoutItemElement.exists()).toBeTruthy();
    });

    test("renders assigned layout item when layout item is assigned and displayed", () => {
        propsMockObject.displayed = true;
        propsMockObject.assigned = true;

        mountDocumentLayoutItem();

        const disabledDocumentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--assigned`);
        expect(disabledDocumentLayoutItemElement.exists()).toBeTruthy();
    });

    test("renders selected layout item when layout item is assigned, selected and displayed", () => {
        propsMockObject.displayed = true;
        propsMockObject.assigned = true;
        propsMockObject.selected = true;

        mountDocumentLayoutItem();

        const disabledDocumentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--selected`);
        expect(disabledDocumentLayoutItemElement.exists()).toBeTruthy();
    });

    test("computed styles should use coordinates", () => {
        const testDocumentLayoutItemElement: React.ReactElement<HTMLDivElement> = wrapper
            .find(`.${COMPONENT_NAME}`)
            .get(0);
        expect(testDocumentLayoutItemElement).toBeDefined();

        expect(testDocumentLayoutItemElement.props.style.top).toBe(propsMockObject.topLeft.y);
        expect(testDocumentLayoutItemElement.props.style.left).toBe(propsMockObject.topLeft.x);
        expect(testDocumentLayoutItemElement.props.style.width).toBe(
            propsMockObject.bottomRight.x - propsMockObject.topLeft.x
        );
        expect(testDocumentLayoutItemElement.props.style.height).toBe(
            propsMockObject.bottomRight.y - propsMockObject.topLeft.y
        );
    });

    test("doesn't call context menu when layout item is not displayed", () => {
        propsMockObject.displayed = false;

        mountDocumentLayoutItem();

        const disabledDocumentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--disabled`);
        expect(disabledDocumentLayoutItemElement.exists()).toBeTruthy();

        disabledDocumentLayoutItemElement.simulate("contextmenu");

        expect(onSelectedItemContextMenuSpy).not.toBeCalled();
    });

    test("doesn't call context menu when layout item is displayed, but not selected", () => {
        propsMockObject.displayed = true;
        propsMockObject.selected = false;

        mountDocumentLayoutItem();

        const disabledDocumentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--enabled`);
        expect(disabledDocumentLayoutItemElement.exists()).toBeTruthy();

        disabledDocumentLayoutItemElement.simulate("contextmenu");

        expect(onSelectedItemContextMenuSpy).not.toBeCalled();
    });

    test("call context menu when layout item is displayed and selected", () => {
        propsMockObject.displayed = true;
        propsMockObject.selected = true;

        mountDocumentLayoutItem();

        const documentLayoutItemElement = wrapper.find(`.${COMPONENT_NAME}--selected`);
        expect(documentLayoutItemElement.exists()).toBeTruthy();

        const ev = document.createEvent("HTMLEvents");
        ev.initEvent("contextmenu", false, false);
        documentLayoutItemElement.getDOMNode().dispatchEvent(ev);

        expect(onSelectedItemContextMenuSpy).toBeCalledTimes(1);
    });

    test("assigned layout item should be with -focus-in modifier when inFocus props is true", () => {
        propsMockObject.displayed = true;
        propsMockObject.assigned = true;
        propsMockObject.inFocus = true;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--assigned`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--assigned-focus-in`).exists()).toBeTruthy();
    });

    test("assigned layout item should be with -focus-out modifier when inFocus props is false", () => {
        propsMockObject.displayed = true;
        propsMockObject.assigned = true;
        propsMockObject.inFocus = false;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--assigned`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--assigned-focus-out`).exists()).toBeTruthy();
    });

    test("assigned layout item should be without (-focus-in or -focus-out) modifiers when inFocus props is undefined", () => {
        propsMockObject.displayed = true;
        propsMockObject.assigned = true;
        propsMockObject.inFocus = undefined;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--assigned`).exists()).toBeTruthy();
        expect(wrapper.find(`.${COMPONENT_NAME}--assigned-focus-in`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--assigned-focus-out`).exists()).toBeFalsy();
    });

    test("enabled layout item should be with -focus-in modifier when inFocus props is true", () => {
        propsMockObject.displayed = true;
        propsMockObject.inFocus = true;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--enabled`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--enabled-focus-in`).exists()).toBeTruthy();
    });

    test("enabled layout item should be with -focus-out modifier when inFocus props is false", () => {
        propsMockObject.displayed = true;
        propsMockObject.inFocus = false;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--enabled`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--enabled-focus-out`).exists()).toBeTruthy();
    });

    test("enabled layout item should be without (-focus-in or -focus-out) modifiers when inFocus props is undefined", () => {
        propsMockObject.displayed = true;
        propsMockObject.inFocus = undefined;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--enabled`).exists()).toBeTruthy();
        expect(wrapper.find(`.${COMPONENT_NAME}--enabled-focus-in`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--enabled-focus-out`).exists()).toBeFalsy();
    });

    test("selected layout item should be with -focus-in modifier when inFocus props is true", () => {
        propsMockObject.displayed = true;
        propsMockObject.selected = true;
        propsMockObject.inFocus = true;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--selected`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--selected-focus-in`).exists()).toBeTruthy();
    });

    test("selected layout item should be with -focus-out modifier when inFocus props is false", () => {
        propsMockObject.displayed = true;
        propsMockObject.selected = true;
        propsMockObject.inFocus = false;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--selected`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--selected-focus-out`).exists()).toBeTruthy();
    });

    test("selected layout item should be without (-focus-in or -focus-out) modifiers when inFocus props is undefined", () => {
        propsMockObject.displayed = true;
        propsMockObject.selected = true;
        propsMockObject.inFocus = undefined;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--selected`).exists()).toBeTruthy();
        expect(wrapper.find(`.${COMPONENT_NAME}--selected-focus-in`).exists()).toBeFalsy();
        expect(wrapper.find(`.${COMPONENT_NAME}--selected-focus-out`).exists()).toBeFalsy();
    });

    test("disabled layout item should be without -focus-in modifier when inFocus props is true", () => {
        propsMockObject.displayed = false;
        propsMockObject.inFocus = true;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--disabled`).exists()).toBeTruthy();
        expect(wrapper.find(`.${COMPONENT_NAME}--disabled-focus-in`).exists()).toBeFalsy();
    });

    test("disabled layout item should be without -focus-out modifier when inFocus props is false", () => {
        propsMockObject.displayed = false;
        propsMockObject.inFocus = false;

        mountDocumentLayoutItem();

        expect(wrapper.find(`.${COMPONENT_NAME}--disabled`).exists()).toBeTruthy();
        expect(wrapper.find(`.${COMPONENT_NAME}--disabled-focus-out`).exists()).toBeFalsy();
    });

    let wrapper: EnzymeWrapperType<typeof DocumentLayoutItemComponent>;

    const onSelectedItemContextMenuSpy = jest.fn();

    const props: ReactPropsType<typeof DocumentLayoutItemComponent> = {
        id: "7",
        text: "56-2445503",
        value: "56-2445503",
        topLeft: {
            x: 815,
            y: 437
        },
        bottomRight: {
            x: 891,
            y: 452
        },
        inFocus: undefined,
        selected: false,
        assigned: false,
        displayed: false,
        onSelectedItemContextMenu: onSelectedItemContextMenuSpy
    };

    let propsMockObject: ReactPropsType<typeof DocumentLayoutItemComponent> = { ...props };

    const mountDocumentLayoutItem = () => {
        wrapper = mount(<DocumentLayoutItemComponent {...propsMockObject} />);
    };
});
