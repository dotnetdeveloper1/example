import { mount } from "enzyme";
import React from "react";
import { act } from "react-dom/test-utils";
import { Provider } from "react-redux";
import { EnzymeWrapperType, ReactPropsType, ReactPropType } from "../../../helperTypes";
import { rootStore } from "../../../store/configuration";
import { DocumentPageViewComponent } from "../DocumentPageViewComponent";
import { InvoiceStatus } from "../store/state";
import { DocumentViewComponent } from "./DocumentViewComponent";
import { COMPONENT_NAME } from "./DocumentViewComponent";
import { CHANGE_CURRENT_PAGE_DELAY } from "./hooks/useDocumentView";
describe(`${COMPONENT_NAME}`, () => {
    // NOTE! DocumentViewComponent test cases are simplified compare to real browser experience.
    //
    // They don't include calculation or verification of documentViewBox size, pageOffset size, scrollOffset size, block positions, etc.
    // Jest unit test environment doesn't provide instruments of real browser viewport simulation.
    //
    // Automated testing of Document View interaction functionality should be implemented as a part of E2E UI testing.
    // For example, with help of Puppeteer, Protractor or Selenium Web Driver.

    describe(`${COMPONENT_NAME}`, () => {
        beforeEach(() => {
            onPageChangeSpy.mockClear();
            propsMockObject.onPageChange = onPageChangeSpy;

            scrollIntoViewSpy.mockClear();
            Element.prototype.scrollIntoView = scrollIntoViewSpy;

            currentPageNumber = 0;

            mountDocumentView();
        });

        test("renders component", () => {
            const documentViewComponentElement = wrapper.find(DocumentViewComponent);
            expect(documentViewComponentElement.exists()).toBeTruthy();
        });

        test("renders NoContentPlaceholder when pages prop is empty", () => {
            const noContentPlaceholderElement = wrapper.find(`.${COMPONENT_NAME}__no-content`);
            expect(noContentPlaceholderElement.exists()).toBeTruthy();
        });

        test(`renders __pages element with 1 page element when pages prop contains 1 page`, () => {
            propsMockObject = {
                ...propsMockObject,
                pages: [pagesMock![0]]
            };

            mountDocumentView();

            const documentViewPagesElement = wrapper.find(`.${COMPONENT_NAME}__pages`);
            expect(documentViewPagesElement.exists()).toBeTruthy();

            const documentPageViewElements = documentViewPagesElement.find(DocumentPageViewComponent);

            expect(documentPageViewElements.exists()).toBeTruthy();
            expect(documentPageViewElements.length).toBe(1);
        });

        test(`renders __pages element with 3 page elements when pages prop contains 3 pages`, () => {
            propsMockObject = {
                ...propsMockObject,
                pages: [pagesMock![0], pagesMock![1], pagesMock![2]]
            };

            mountDocumentView();

            const documentPageViewElements = wrapper.find(DocumentPageViewComponent);

            expect(documentPageViewElements.exists()).toBeTruthy();
            expect(documentPageViewElements.length).toBe(3);
        });

        test(`no scroll when the current page is the first`, () => {
            propsMockObject = {
                ...propsMockObject,
                pages: [...pagesMock!]
            };

            mountDocumentView();

            const documentViewPagesElement = wrapper.find(`.${COMPONENT_NAME}__pages`);

            expect(documentViewPagesElement.exists()).toBeTruthy();
            expect(currentPageNumber).toBe(0);
            expect(scrollIntoViewSpy).not.toBeCalled();
        });

        test(`auto scroll when the current page is the second and has force auto scroll flag`, async (done) => {
            propsMockObject = {
                ...propsMockObject,
                pages: [...pagesMock!]
            };

            propsMockObject.pages![0].current = false;
            propsMockObject.pages![1].current = true;
            propsMockObject.pages![1].autoScroll = true;

            mountDocumentView();

            // setTimeout required to pass debounce delay
            setTimeout(() => {
                expect(currentPageNumber).toBe(propsMockObject.pages![1].number);
                expect(scrollIntoViewSpy).toBeCalled();
                currentPageNumber = 0;
                done();
            }, CHANGE_CURRENT_PAGE_DELAY * 2);
        });

        test("set the current page from the second one to the first one according to scroll position", async (done) => {
            currentPageNumber = 0;

            propsMockObject = {
                ...propsMockObject,
                pages: [...pagesMock!]
            };

            propsMockObject.pages![0].current = false;
            propsMockObject.pages![1].current = true;
            propsMockObject.pages![1].autoScroll = false;

            mountDocumentView();

            const documentViewPagesElement = wrapper.find(`.${COMPONENT_NAME}__pages`);
            documentViewPagesElement.simulate("scroll");

            expect(currentPageNumber).toBe(0);

            // setTimeout required to pass debounce delay
            setTimeout(() => {
                expect(currentPageNumber).toBe(1);
                expect(propsMockObject.onPageChange).toBeCalled();
                done();
            }, CHANGE_CURRENT_PAGE_DELAY * 2);
        });

        test("should notify DocumentPageViewComponent about DocumentView resize", () => {
            propsMockObject = {
                ...propsMockObject,
                pages: [pagesMock![0]]
            };

            const { eventMap, documentViewBox, getDocumentViewBoxSpy } = setDocumentViewSizingEventMock();

            mountDocumentView();

            act(() => {
                eventMap["resize"]();
            });

            const documentViewPageElement = wrapper.find(DocumentPageViewComponent);
            expect(documentViewPageElement.props().documentViewBox).toStrictEqual(documentViewBox);

            // NOTE should be called on component mount and on resize
            expect(getDocumentViewBoxSpy).toBeCalledTimes(20);
        });

        test("should auto scroll visible area when layout item with focus state exists and showCompareBoxes is enabled", () => {
            propsMockObject = {
                ...propsMockObject,
                pages: [{ ...pagesMock![0], pageLayoutItems: testLayoutItems }],
                showCompareBoxes: true
            };

            let scrollPosition: { x: number; y: number } | undefined;

            const autoScrollSpy = jest.fn((x, y) => {
                scrollPosition = {
                    x: x,
                    y: y
                };
            });

            Element.prototype.scroll = autoScrollSpy as any;

            mountDocumentView();

            // NOTE Scroll position calculation result is based on assumption that document view box has (0,0,0,0) size
            // and the current scroll position at (0,0), because of missing real browser viewport.
            // In this case, scroll movement should be done right into the middle of layout element box.
            // See getItemScrollPosition function in useDocumentLayoutItemFocusTracking hook for details.

            const expectedScrollPositionX =
                testLayoutItems[0].topLeft.x + (testLayoutItems[0].bottomRight.x - testLayoutItems[0].topLeft.x) / 2;
            const expectedScrollPositionY =
                testLayoutItems[0].topLeft.y + (testLayoutItems[0].bottomRight.y - testLayoutItems[0].topLeft.y) / 2;

            expect(autoScrollSpy).toBeCalledTimes(1);
            expect(scrollPosition).toBeDefined();
            expect(scrollPosition!.x).toBe(expectedScrollPositionX);
            expect(scrollPosition!.y).toBe(expectedScrollPositionY);
        });

        const testLayoutItems = [
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
                inFocus: true,
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
                inFocus: false,
                selected: false,
                assigned: false
            }
        ];

        const pagesMock: ReactPropType<typeof DocumentViewComponent, "pages"> = [
            {
                id: 1,
                number: 1,
                height: 1200,
                width: 750,
                imageFileId: "",
                autoScroll: false,
                current: true,
                pageLayoutItems: []
            },
            {
                id: 2,
                number: 2,
                height: 1200,
                width: 750,
                imageFileId: "",
                autoScroll: false,
                current: false,
                pageLayoutItems: []
            },
            {
                id: 3,
                number: 3,
                height: 1200,
                width: 750,
                imageFileId: "",
                autoScroll: false,
                current: false,
                pageLayoutItems: []
            }
        ];

        let wrapper: EnzymeWrapperType<typeof DocumentViewComponent>;

        let propsMockObject: ReactPropsType<typeof DocumentViewComponent> = {
            pages: [],
            showCompareBoxes: false,
            selectedPlainText: "",
            invoiceStatus: InvoiceStatus.PendingReview,
            onPageChange: jest.fn(),
            onLayoutItemsSelect: jest.fn(),
            onAssignItems: jest.fn(),
            onAssignLineItems: jest.fn(),
            onAssignColumnToLineItems: jest.fn(),
            tableSelectionMode: false
        };
        let currentPageNumber: number = 0;

        const onPageChangeSpy: jest.Mock<ReturnType<
            ReactPropType<typeof DocumentViewComponent, "onPageChange">
        >> = jest.fn((pageNumber: number, autoScroll: number) => {
            // ensure that document view doesn't use force auto scroll to selected page
            expect(autoScroll).toBeFalsy();

            currentPageNumber = pageNumber;
        });

        const scrollIntoViewSpy = jest.fn((scrollOptions) =>
            expect(scrollOptions).toStrictEqual({ behavior: "auto", block: "start" })
        );

        const mountDocumentView = () => {
            wrapper = mount(
                <Provider store={rootStore}>
                    <DocumentViewComponent
                        pages={propsMockObject.pages}
                        showCompareBoxes={propsMockObject.showCompareBoxes}
                        onPageChange={onPageChangeSpy}
                        onLayoutItemsSelect={propsMockObject.onLayoutItemsSelect}
                        selectedPlainText={propsMockObject.selectedPlainText}
                        onAssignItems={propsMockObject.onAssignItems}
                        onAssignLineItems={propsMockObject.onAssignLineItems}
                        onAssignColumnToLineItems={propsMockObject.onAssignColumnToLineItems}
                        invoiceStatus={propsMockObject.invoiceStatus}
                        tableSelectionMode={propsMockObject.tableSelectionMode}
                    />
                </Provider>
            );
        };

        const setDocumentViewSizingEventMock = (documentViewBox?: {
            top: number;
            left: number;
            width: number;
            height: number;
        }): {
            eventMap: { [eventType: string]: any };
            getDocumentViewBoxSpy: jest.Mock<any>;
            documentViewBox: {
                top: number;
                left: number;
                width: number;
                height: number;
            };
        } => {
            const box = documentViewBox || {
                top: 0,
                left: 0,
                width: 100,
                height: 200
            };

            const eventMap: { [eventType: string]: any } = {};

            window.addEventListener = jest.fn((eventType, handler) => {
                eventMap[eventType] = handler;
            });

            const documentViewBoundingBoxSpy = jest.fn(() => box);

            Element.prototype.getBoundingClientRect = documentViewBoundingBoxSpy as any;

            return {
                eventMap: eventMap,
                documentViewBox: box,
                getDocumentViewBoxSpy: documentViewBoundingBoxSpy
            };
        };
    });
});
