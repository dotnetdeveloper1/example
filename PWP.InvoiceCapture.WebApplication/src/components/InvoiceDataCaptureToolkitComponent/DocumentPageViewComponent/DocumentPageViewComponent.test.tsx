import { mount } from "enzyme";
import React from "react";
import { Provider } from "react-redux";
import { EnzymeWrapperType, ReactPropsType, ReactPropType } from "../../../helperTypes";
import { rootStore } from "../../../store/configuration";
import {
    COMPONENT_NAME as FIELD_ASSIGNMENT_MENU_COMPONENT_NAME,
    FieldAssignmentMenuComponent
} from "../FieldAssignmentMenuComponent";
import { InvoiceStatus } from "../store/state";
import { COMPONENT_NAME as DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME } from "./DocumentLayoutItemComponent";
import { DocumentPageImagePaneComponent } from "./DocumentPageImagePaneComponent";
import { COMPONENT_NAME, DocumentPageViewComponent } from "./DocumentPageViewComponent";
import { DOCUMENT_VIEW_PAGE_PADDING_WIDTH_INTERVAL } from "./hooks/useDocumentPageView";
import { MultiSelectionPaneComponent } from "./MultiSelectionPaneComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        propsMockObject = { ...props };

        elementRefSpy.mockClear();

        mountDocumentPageView();
    });

    test("renders component", () => {
        const documentPageViewComponentElement = wrapper.find(DocumentPageViewComponent);
        expect(documentPageViewComponentElement.exists()).toBeTruthy();
    });

    test(`doesn't render ${COMPONENT_NAME} DIV with empty page prop`, () => {
        const documentPageViewComponentRootElement = wrapper.find(`.${COMPONENT_NAME}`);
        expect(documentPageViewComponentRootElement.exists()).toBeFalsy();
        expect(rootElementRef && rootElementRef.current).toBeNull();
    });

    test(`renders ${COMPONENT_NAME} DIV with valid page in props`, () => {
        propsMockObject.page = { ...pageMock! };

        mountDocumentPageView();

        const documentPageViewComponentRootElement = wrapper.find(`.${COMPONENT_NAME}`);
        expect(documentPageViewComponentRootElement.exists()).toBeTruthy();
        expect(rootElementRef && rootElementRef.current).toBeDefined();
    });

    test("doesn't render MultiSelectionPaneComponent when empty layout items array in props", () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.page!.pageLayoutItems = [];

        mountDocumentPageView();

        const multiSelectionPaneComponentElement = wrapper.find(MultiSelectionPaneComponent);
        expect(multiSelectionPaneComponentElement.exists()).toBeFalsy();
    });

    test("renders MultiSelectionPaneComponent when layout items array is specified in props", () => {
        propsMockObject.page = { ...pageMock! };

        mountDocumentPageView();

        const multiSelectionPaneComponentElement = wrapper.find(MultiSelectionPaneComponent);
        expect(multiSelectionPaneComponentElement.exists()).toBeTruthy();
    });

    test("doesn't render FieldAssignmentMenuComponent when all layout items are unselected in props", () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.page!.pageLayoutItems = [{ ...pageMock!.pageLayoutItems![0], selected: false }];

        mountDocumentPageView();

        const fieldAssignmentMenuComponentElement = wrapper.find(FieldAssignmentMenuComponent);
        expect(fieldAssignmentMenuComponentElement.exists()).toBeFalsy();
    });

    test("doesn't render FieldAssignmentMenuComponent when some layout items are selected in props, but showCompareBoxes is disabled", () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.page!.pageLayoutItems = [{ ...pageMock!.pageLayoutItems![0], selected: true }];
        propsMockObject.showCompareBoxes = false;

        mountDocumentPageView();

        const fieldAssignmentMenuComponentElement = wrapper.find(FieldAssignmentMenuComponent);
        expect(fieldAssignmentMenuComponentElement.exists()).toBeFalsy();
    });

    test("renders FieldAssignmentMenuComponent when some layout items are selected in props", () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.page!.pageLayoutItems = [{ ...pageMock!.pageLayoutItems![0], selected: true }];
        propsMockObject.showCompareBoxes = true;

        mountDocumentPageView();

        const fieldAssignmentMenuComponentElement = wrapper.find(FieldAssignmentMenuComponent);
        expect(fieldAssignmentMenuComponentElement.exists()).toBeTruthy();
    });

    test("renders DocumentPageImagePaneComponent with (width, height) from page props when documentViewBox width larger than page width", () => {
        propsMockObject.page = { ...pageMock! };

        mountDocumentPageView();

        const documentPageImagePaneComponentElement = wrapper.find(DocumentPageImagePaneComponent);
        expect(documentPageImagePaneComponentElement.exists()).toBeTruthy();
        expect(documentPageImagePaneComponentElement.props().width).toBe(propsMockObject.page!.width);
        expect(documentPageImagePaneComponentElement.props().height).toBe(propsMockObject.page!.height);
    });

    test("renders DocumentPageImagePaneComponent with calculated (width, height) when documentViewBox width smaller than page width", () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.documentViewBox = { ...props.documentViewBox, width: 100 };

        mountDocumentPageView();

        const documentPageImagePaneComponentElement = wrapper.find(DocumentPageImagePaneComponent);
        expect(documentPageImagePaneComponentElement.exists()).toBeTruthy();

        expect(documentPageImagePaneComponentElement.props().width).toBe(
            propsMockObject.documentViewBox.width - DOCUMENT_VIEW_PAGE_PADDING_WIDTH_INTERVAL
        );
        expect(documentPageImagePaneComponentElement.props().height).toBe(
            ((propsMockObject.documentViewBox.width - DOCUMENT_VIEW_PAGE_PADDING_WIDTH_INTERVAL) /
                propsMockObject.page!.width) *
                propsMockObject.page!.height
        );
    });

    test(`renders ${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME} entries from page object with disabled state when showCompareBoxes is disabled`, () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.showCompareBoxes = false;

        mountDocumentPageView();

        const disabledDocumentLayoutItemElements = wrapper.find(`.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}--disabled`);
        expect(disabledDocumentLayoutItemElements.exists()).toBeTruthy();
        expect(disabledDocumentLayoutItemElements.length).toBe(pageMock!.pageLayoutItems!.length);
    });

    test("renders one enabled and one assigned layout item from page object when showCompareBoxes is enabled and assignedId list is provided", () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.showCompareBoxes = true;
        propsMockObject.page!.pageLayoutItems![0] = { ...pageMock!.pageLayoutItems![0], assigned: true };

        mountDocumentPageView();

        const enabledDocumentLayoutItemElements = wrapper.find(`.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}--enabled`);
        expect(enabledDocumentLayoutItemElements.exists()).toBeTruthy();
        expect(enabledDocumentLayoutItemElements.length).toBe(1);

        const assignedDocumentLayoutItemElements = wrapper.find(`.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}--assigned`);
        expect(assignedDocumentLayoutItemElements.exists()).toBeTruthy();
        expect(assignedDocumentLayoutItemElements.length).toBe(1);
    });

    test("renders one enabled in focus and one assigned out of focus layout item from page object when showCompareBoxes is enabled and assignedId list is provided", () => {
        const pageLayoutItemsWithFocus = [
            {
                ...pageMock!.pageLayoutItems![0],
                assigned: true,
                inFocus: false
            },
            {
                ...pageMock!.pageLayoutItems![1],
                assigned: false,
                inFocus: true
            }
        ];

        propsMockObject.page = { ...pageMock! };
        propsMockObject.showCompareBoxes = true;
        propsMockObject.page!.pageLayoutItems = pageLayoutItemsWithFocus;

        mountDocumentPageView();

        const enabledDocumentLayoutItemElements = wrapper.find(
            `.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}--enabled-focus-in`
        );
        expect(enabledDocumentLayoutItemElements.exists()).toBeTruthy();
        expect(enabledDocumentLayoutItemElements.length).toBe(1);

        const assignedDocumentLayoutItemElements = wrapper.find(
            `.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}--assigned-focus-out`
        );
        expect(assignedDocumentLayoutItemElements.exists()).toBeTruthy();
        expect(assignedDocumentLayoutItemElements.length).toBe(1);
    });

    test(`${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME} computed styles should use pageLayoutItems coordinates`, () => {
        const testDocumentLayoutItem = pageMock!.pageLayoutItems![0];
        propsMockObject.page = { ...pageMock!, pageLayoutItems: [testDocumentLayoutItem] };

        mountDocumentPageView();

        const testDocumentLayoutItemElement: React.ReactElement<HTMLDivElement> = wrapper
            .find(`.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}`)
            .get(0);
        expect(testDocumentLayoutItemElement).toBeDefined();

        expect(testDocumentLayoutItemElement.props.style.top).toBe(testDocumentLayoutItem.topLeft.y);
        expect(testDocumentLayoutItemElement.props.style.left).toBe(testDocumentLayoutItem.topLeft.x);
        expect(testDocumentLayoutItemElement.props.style.width).toBe(
            testDocumentLayoutItem.bottomRight.x - testDocumentLayoutItem.topLeft.x
        );
        expect(testDocumentLayoutItemElement.props.style.height).toBe(
            testDocumentLayoutItem.bottomRight.y - testDocumentLayoutItem.topLeft.y
        );
    });

    test("renders context menu for selected Document Layout Item", () => {
        propsMockObject.page = { ...pageMock! };
        propsMockObject.showCompareBoxes = true;
        propsMockObject.page!.pageLayoutItems![0] = { ...pageMock!.pageLayoutItems![0], selected: true };

        mountDocumentPageView();

        const selectedDocumentLayoutItemElements = wrapper.find(`.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}--selected`);
        expect(selectedDocumentLayoutItemElements.exists()).toBeTruthy();

        expect(wrapper.find(`.${FIELD_ASSIGNMENT_MENU_COMPONENT_NAME}`).exists()).toBeFalsy();

        expect(selectedDocumentLayoutItemElements.exists()).toBeTruthy();

        const ev = document.createEvent("HTMLEvents") as MouseEvent;
        ev.initEvent("contextmenu", true, false);
        selectedDocumentLayoutItemElements.getDOMNode().dispatchEvent(ev);
        selectedDocumentLayoutItemElements.simulate("contextmenu", {
            pageX: 100,
            pageY: 200
        });

        expect(wrapper.find(`.${FIELD_ASSIGNMENT_MENU_COMPONENT_NAME}`).exists()).toBeTruthy();
    });

    test(`${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME} computed styles should use computed coordinates when document view width lower than page width`, () => {
        const testDocumentLayoutItem = pageMock!.pageLayoutItems![0];
        propsMockObject.page = { ...pageMock!, pageLayoutItems: [testDocumentLayoutItem] };
        propsMockObject.documentViewBox = { ...props.documentViewBox, width: 100 };

        mountDocumentPageView();

        const testDocumentLayoutItemElement: React.ReactElement<HTMLDivElement> = wrapper
            .find(`.${DOCUMENT_LAYOUT_ITEM_COMPONENT_NAME}`)
            .get(0);
        expect(testDocumentLayoutItemElement).toBeDefined();

        const scaleRatio =
            (propsMockObject.documentViewBox.width - DOCUMENT_VIEW_PAGE_PADDING_WIDTH_INTERVAL) /
            propsMockObject.page!.width;

        expect(testDocumentLayoutItemElement.props.style.top).toBe(scaleRatio * testDocumentLayoutItem.topLeft.y);
        expect(testDocumentLayoutItemElement.props.style.left).toBe(scaleRatio * testDocumentLayoutItem.topLeft.x);
        expect(
            Math.abs(
                Number(testDocumentLayoutItemElement.props.style.width) -
                    (testDocumentLayoutItem.bottomRight.x - testDocumentLayoutItem.topLeft.x) * scaleRatio
            )
        ).toBeLessThan(1);
        expect(
            Math.abs(
                Number(testDocumentLayoutItemElement.props.style.height) -
                    (testDocumentLayoutItem.bottomRight.y - testDocumentLayoutItem.topLeft.y) * scaleRatio
            )
        ).toBeLessThan(1);
    });

    const pageMock: ReactPropType<typeof DocumentPageViewComponent, "page"> = {
        id: 1,
        number: 1,
        height: 1200,
        width: 750,
        imageFileId: "",
        autoScroll: false,
        current: true,
        pageLayoutItems: [
            {
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
                assigned: false
            },
            {
                id: "4",
                text: "06/05/2019",
                value: "06/05/2019",
                topLeft: {
                    x: 490,
                    y: 90
                },
                bottomRight: {
                    x: 561,
                    y: 105
                },
                inFocus: undefined,
                selected: false,
                assigned: false
            }
        ]
    };

    let wrapper: EnzymeWrapperType<typeof DocumentPageViewComponent>;
    let rootElementRef: React.RefObject<HTMLDivElement> | null = null;

    const elementRefSpy = jest.fn((ref) => (rootElementRef = ref));

    const props: ReactPropsType<typeof DocumentPageViewComponent> = {
        page: undefined,
        showCompareBoxes: false,
        selectedPlainText: "",
        containerScrollOffset: { x: 0, y: 0 },
        invoiceStatus: InvoiceStatus.PendingReview,
        tableSelectionMode: false,
        documentViewBox: {
            top: 0,
            left: 0,
            height: pageMock.height,
            width: pageMock.width + DOCUMENT_VIEW_PAGE_PADDING_WIDTH_INTERVAL
        },
        tablePageNumber: 0,
        selectedTable: {
            top: 0,
            left: 0,
            height: 300,
            width: 500
        },
        onLayoutItemsSelect: jest.fn(),
        onAssignItems: jest.fn(),
        onAssignLineItems: jest.fn(),
        onAssignColumnToLineItems: jest.fn(),
        onTableSelected: jest.fn(),
        tooltipWasShown: false,
        onTooltipWasShown: jest.fn()
    };

    let propsMockObject = { ...props };

    const mountDocumentPageView = () => {
        wrapper = mount(
            <Provider store={rootStore}>
                <DocumentPageViewComponent
                    page={propsMockObject.page}
                    showCompareBoxes={propsMockObject.showCompareBoxes}
                    onLayoutItemsSelect={propsMockObject.onLayoutItemsSelect}
                    selectedPlainText={propsMockObject.selectedPlainText}
                    onAssignItems={propsMockObject.onAssignItems}
                    onAssignLineItems={propsMockObject.onAssignLineItems}
                    onAssignColumnToLineItems={propsMockObject.onAssignColumnToLineItems}
                    containerScrollOffset={propsMockObject.containerScrollOffset}
                    documentViewBox={propsMockObject.documentViewBox}
                    invoiceStatus={propsMockObject.invoiceStatus}
                    tableSelectionMode={propsMockObject.tableSelectionMode}
                    onTableSelected={propsMockObject.onTableSelected}
                    tablePageNumber={propsMockObject.tablePageNumber}
                    selectedTable={propsMockObject.selectedTable}
                    tooltipWasShown={propsMockObject.tableSelectionMode}
                    onTooltipWasShown={propsMockObject.onTooltipWasShown}
                />
            </Provider>
        );
    };
});
