import { mount } from "enzyme";
import React from "react";
import { Provider } from "react-redux";
import { EnzymeWrapperType, ReactPropsType } from "../../../helperTypes";
import { rootStore } from "../../../store/configuration";
import { DocumentLayoutItemComponent } from "./DocumentLayoutItemComponent";
import { COMPONENT_NAME, MultiSelectionPaneComponent } from "./MultiSelectionPaneComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        preSelectedIds = [];
        showCompareBoxes = true;
        onLayoutItemsSelectSpy = jest.fn();
        mountMultiSelectionPane();
    });

    test("renders component", () => {
        const multiSelectionPaneComponentElement = wrapper.find(MultiSelectionPaneComponent);
        expect(multiSelectionPaneComponentElement.exists()).toBeTruthy();
    });

    test("renders __selection-box when mouse down on the component area with minimal computed width, hight - (1,1)", () => {
        wrapper.find(MultiSelectionPaneComponent).simulate("mousedown", {
            button: 0,
            pageX: mockContainerBox.x,
            pageY: mockContainerBox.y
        });

        const selectionBoxElement = wrapper.find(`.${COMPONENT_NAME}__selection-box`);
        expect(selectionBoxElement.exists()).toBeTruthy();
        expect(selectionBoxElement.get(0).props.style.top).toBe(mockContainerBox.y);
        expect(selectionBoxElement.get(0).props.style.left).toBe(mockContainerBox.x);
        expect(selectionBoxElement.get(0).props.style.height).toBe(1);
        expect(selectionBoxElement.get(0).props.style.width).toBe(1);
    });

    test("doesn't render __selection-box when mouse down and move over the component area with computed rectangle from mouse coordinates and disabled showCompareBoxes", () => {
        showCompareBoxes = false;

        mountMultiSelectionPane();

        const startMousePosition = {
            x: 100,
            y: 200
        };

        const moveMousePosition = {
            x: 300,
            y: 400
        };

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: startMousePosition.x,
                pageY: startMousePosition.y
            })
            .simulate("mousemove", {
                button: 0,
                buttons: 1,
                pageX: moveMousePosition.x,
                pageY: moveMousePosition.y
            })
            .simulate("mouseup", {
                button: 0,
                pageX: startMousePosition.x,
                pageY: startMousePosition.y
            });

        const selectionBoxElement = wrapper.find(`.${COMPONENT_NAME}__selection-box`);
        expect(selectionBoxElement.exists()).toBeFalsy();
    });

    test("renders __selection-box when mouse down and move over the component area with computed rectangle from mouse coordinates", () => {
        const startMousePosition = {
            x: 100,
            y: 200
        };

        const moveMousePosition = {
            x: 300,
            y: 400
        };

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: startMousePosition.x,
                pageY: startMousePosition.y
            })
            .simulate("mousemove", {
                button: 0,
                buttons: 1,
                pageX: moveMousePosition.x,
                pageY: moveMousePosition.y
            });

        const selectionBoxElement = wrapper.find(`.${COMPONENT_NAME}__selection-box`);
        expect(selectionBoxElement.exists()).toBeTruthy();
        expect(selectionBoxElement.get(0).props.style.top).toBe(startMousePosition.y);
        expect(selectionBoxElement.get(0).props.style.left).toBe(startMousePosition.x);
        expect(selectionBoxElement.get(0).props.style.height).toBe(moveMousePosition.y - startMousePosition.y);
        expect(selectionBoxElement.get(0).props.style.width).toBe(moveMousePosition.x - startMousePosition.x);
    });

    test("doesn't render __selection-box when MMB down", () => {
        const mousePosition = {
            x: 100,
            y: 200
        };

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 1,
                pageX: mousePosition.x,
                pageY: mousePosition.y
            })
            .simulate("mousemove", {
                button: 1,
                buttons: 1,
                pageX: mousePosition.x + 100,
                pageY: mousePosition.y - 100
            });

        const selectionBoxElement = wrapper.find(`.${COMPONENT_NAME}__selection-box`);
        expect(selectionBoxElement.exists()).toBeFalsy();
    });

    test("doesn't render __selection-box when RMB down and props contain selected elements", () => {
        preSelectedIds = [layoutItems[0].id, layoutItems[1].id];

        mountMultiSelectionPane();

        const mousePosition = {
            x: 100,
            y: 200
        };

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 2,
                pageX: mousePosition.x,
                pageY: mousePosition.y
            })
            .simulate("mousemove", {
                button: 2,
                buttons: 2,
                pageX: mousePosition.x + 100,
                pageY: mousePosition.y - 100
            });

        const selectionBoxElement = wrapper.find(`.${COMPONENT_NAME}__selection-box`);
        expect(selectionBoxElement.exists()).toBeFalsy();
    });

    test("render __selection-box when RMB down and props contain one preselected element, but doesn't resize on move over the component area", () => {
        preSelectedIds = [layoutItems[1].id];

        mountMultiSelectionPane();

        const mousePosition = {
            x: 100,
            y: 200
        };

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 2,
                pageX: mousePosition.x,
                pageY: mousePosition.y
            })
            .simulate("mousemove", {
                button: 2,
                buttons: 2,
                pageX: mousePosition.x + 100,
                pageY: mousePosition.y - 100
            });

        const selectionBoxElement = wrapper.find(`.${COMPONENT_NAME}__selection-box`);
        expect(selectionBoxElement.exists()).toBeTruthy();

        expect(selectionBoxElement.get(0).props.style.width).toBe(1);
        expect(selectionBoxElement.get(0).props.style.height).toBe(1);
    });

    test("modify DocumentLayoutItemComponent selected prop when __selection-box intersects with layout item", () => {
        wrapper.find(MultiSelectionPaneComponent).simulate("mousedown", {
            button: 0,
            pageX: layoutItems[0].topLeft.x,
            pageY: layoutItems[0].topLeft.y
        });

        const documentLayoutItemElements = wrapper.find(DocumentLayoutItemComponent);
        expect(documentLayoutItemElements.length).toBe(layoutItems.length);
        expect(documentLayoutItemElements.get(0).props.selected).toBeTruthy();
        expect(documentLayoutItemElements.get(1).props.selected).toBeFalsy();
    });

    test("remove selection when mouse click on empty area position", () => {
        preSelectedIds = [layoutItems[0].id];

        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).not.toContain(layoutItems[0].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: mockContainerBox.x,
                pageY: mockContainerBox.y
            })
            .simulate("mouseup", {
                button: 0,
                pageX: mockContainerBox.x,
                pageY: mockContainerBox.y
            });

        expect(onLayoutItemsSelectSpy).toBeCalled();
    });

    test("doesn't select the first layout item when mouseup event occurs without previous mousedown event on item area position", () => {
        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mouseup", {
                button: 0,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            })
            .simulate("mouseup", {
                button: 2,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            });

        expect(onLayoutItemsSelectSpy).not.toBeCalled();
    });

    test("select the first layout item when mouse click on item area position", () => {
        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).toContain(layoutItems[0].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            })
            .simulate("mouseup", {
                button: 0,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            });

        expect(onLayoutItemsSelectSpy).toBeCalled();
    });

    test("select the first layout item when RMB mouse click on the first item area position and move to the second one", () => {
        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).toContain(layoutItems[0].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 2,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            })
            .simulate("mousemove", {
                button: 2,
                buttons: 2,
                pageX: layoutItems[1].topLeft.x,
                pageY: layoutItems[1].topLeft.y
            })
            .simulate("mouseup", {
                button: 2
            });

        expect(onLayoutItemsSelectSpy).toBeCalled();
    });

    test("select the first layout and the second layout items when click on the first item area position and ctrl+click on the second one", () => {
        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            preSelectedIds = [...preSelectedIds, ...ids];
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            })
            .simulate("mouseup", {
                button: 0,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            })
            .simulate("mousedown", {
                button: 0,
                ctrlKey: true,
                pageX: layoutItems[1].topLeft.x,
                pageY: layoutItems[1].topLeft.y
            })
            .simulate("mouseup", {
                button: 0,
                ctrlKey: true,
                pageX: layoutItems[1].topLeft.x,
                pageY: layoutItems[1].topLeft.y
            });

        expect(onLayoutItemsSelectSpy).toBeCalledTimes(2);
        expect(preSelectedIds.length).toBe(2);
        expect(preSelectedIds).toContain(layoutItems[0].id);
        expect(preSelectedIds).toContain(layoutItems[1].id);
    });

    test("select only the first layout item when click on the first item area position with the second preselected item", () => {
        preSelectedIds = [layoutItems[1].id];

        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).toContain(layoutItems[0].id);
            expect(ids).not.toContain(layoutItems[1].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            })
            .simulate("mouseup", {
                button: 0,
                pageX: layoutItems[0].topLeft.x,
                pageY: layoutItems[0].topLeft.y
            });

        expect(onLayoutItemsSelectSpy).toBeCalledTimes(1);
    });

    test("select only the second layout item when click on empty area position with the second preselected item", () => {
        preSelectedIds = [layoutItems[1].id];

        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).not.toContain(layoutItems[0].id);
            expect(ids).toContain(layoutItems[1].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                ctrlKey: true,
                pageX: mockContainerBox.width - 10,
                pageY: mockContainerBox.height - 10
            })
            .simulate("mousemove", {
                button: 0,
                buttons: 1,
                ctrlKey: true,
                pageX: 0,
                pageY: 0
            })
            .simulate("mousemove", {
                button: 0,
                buttons: 1,
                ctrlKey: true,
                pageX: mockContainerBox.width,
                pageY: mockContainerBox.height
            })
            .simulate("mouseup", {
                button: 0,
                ctrlKey: true,
                pageX: mockContainerBox.width,
                pageY: mockContainerBox.height
            });

        expect(onLayoutItemsSelectSpy).toBeCalledTimes(1);
    });

    test("select all test layout items when the first item is pre-selected and the second is clicked with ctrl key", () => {
        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).toContain(layoutItems[0].id);
            expect(ids).toContain(layoutItems[1].id);
        });

        preSelectedIds = [layoutItems[0].id];

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                ctrlKey: true,
                pageX: layoutItems[1].topLeft.x,
                pageY: layoutItems[1].topLeft.y
            })
            .simulate("mouseup", {
                button: 0,
                ctrlKey: true,
                pageX: layoutItems[1].topLeft.x,
                pageY: layoutItems[1].topLeft.y
            });

        expect(onLayoutItemsSelectSpy).toBeCalled();
    });

    test("select all test layout items when mouse selection area covers all elements", () => {
        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).toContain(layoutItems[0].id);
            expect(ids).toContain(layoutItems[1].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: mockContainerBox.x,
                pageY: mockContainerBox.y
            })
            .simulate("mousemove", {
                button: 0,
                buttons: 1,
                pageX: mockContainerBox.x + mockContainerBox.width,
                pageY: mockContainerBox.y + mockContainerBox.height
            })
            .simulate("mouseup", {
                button: 0,
                pageX: mockContainerBox.x + mockContainerBox.width,
                pageY: mockContainerBox.y + mockContainerBox.height
            });

        expect(onLayoutItemsSelectSpy).toBeCalled();
    });

    test("select the second layout item when mouse selection area covers the second item and the first item is preselected", () => {
        preSelectedIds = [layoutItems[0].id];

        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).not.toContain(layoutItems[0].id);
            expect(ids).toContain(layoutItems[1].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                pageX: layoutItems[1].topLeft.x,
                pageY: layoutItems[1].topLeft.y
            })
            .simulate("mousemove", {
                button: 0,
                buttons: 1,
                pageX: layoutItems[1].bottomRight.x,
                pageY: layoutItems[1].bottomRight.y
            })
            .simulate("mouseup", {
                button: 0,
                pageX: layoutItems[1].bottomRight.x,
                pageY: layoutItems[1].bottomRight.y
            });

        expect(onLayoutItemsSelectSpy).toBeCalled();
    });

    test("select all test layout items when mouse selection area with ctrl key covers the second item and the first item is preselected", () => {
        preSelectedIds = [layoutItems[0].id];

        onLayoutItemsSelectSpy = jest.fn((ids: string[]) => {
            expect(ids).toContain(layoutItems[0].id);
            expect(ids).toContain(layoutItems[1].id);
        });

        mountMultiSelectionPane();

        wrapper
            .find(MultiSelectionPaneComponent)
            .simulate("mousedown", {
                button: 0,
                ctrlKey: true,
                pageX: layoutItems[1].topLeft.x,
                pageY: layoutItems[1].topLeft.y
            })
            .simulate("mousemove", {
                button: 0,
                buttons: 1,
                ctrlKey: true,
                pageX: layoutItems[1].bottomRight.x,
                pageY: layoutItems[1].bottomRight.y
            })
            .simulate("mouseup", {
                button: 0,
                pageX: layoutItems[1].bottomRight.x,
                pageY: layoutItems[1].bottomRight.y
            });

        expect(onLayoutItemsSelectSpy).toBeCalled();
    });

    let wrapper: EnzymeWrapperType<typeof React.Component>;

    const mockContainerBox = {
        x: 0,
        y: 0,
        width: 900,
        height: 1200
    };

    const layoutItems: ReactPropsType<typeof DocumentLayoutItemComponent>[] = [
        {
            id: "1",
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
            displayed: true,
            onSelectedItemContextMenu: jest.fn()
        },
        {
            id: "2",
            text: "56-2445503",
            value: "56-2445503",
            topLeft: {
                x: 715,
                y: 437
            },
            bottomRight: {
                x: 791,
                y: 452
            },
            inFocus: undefined,
            selected: false,
            assigned: false,
            displayed: true,
            onSelectedItemContextMenu: jest.fn()
        }
    ];

    let onLayoutItemsSelectSpy = jest.fn();
    let preSelectedIds: string[] = [];
    let showCompareBoxes: boolean = true;

    const mountMultiSelectionPane = () => {
        wrapper = mount(
            <Provider store={rootStore}>
                <div style={{ position: "absolute", width: mockContainerBox.width, height: mockContainerBox.height }}>
                    <MultiSelectionPaneComponent
                        selectedIds={preSelectedIds}
                        showCompareBoxes={showCompareBoxes}
                        onLayoutItemsSelect={onLayoutItemsSelectSpy}
                        tableSelectionMode={false}
                        onTableSelected={() => 0}
                        containerScrollOffset={{ x: 0, y: 0 }}>
                        {layoutItems.map((item) => (
                            <DocumentLayoutItemComponent key={`layout-item-component-${item.id}`} {...item} />
                        ))}
                    </MultiSelectionPaneComponent>
                </div>
            </Provider>
        );
    };

    Element.prototype.getBoundingClientRect = jest.fn(() => ({
        x: mockContainerBox.x,
        y: mockContainerBox.y,
        width: mockContainerBox.width,
        height: mockContainerBox.height,
        top: mockContainerBox.y,
        left: mockContainerBox.x
    })) as any;
});
