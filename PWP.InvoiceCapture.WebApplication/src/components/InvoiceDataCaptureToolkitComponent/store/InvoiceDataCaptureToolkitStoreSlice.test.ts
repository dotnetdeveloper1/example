import { createRootStore } from "../../../store/configuration";
import {
    assignSelectedLayoutItemsToInvoiceField,
    changeCurrentPage,
    changeInvoiceFieldDataAnnotation,
    clearInvoiceFieldDataAnnotationSelection,
    clearInvoiceFields,
    confirmError,
    fetchInvoiceDataAsync,
    getCleanProcessingResults,
    getInvoiceDataAnnotation,
    getInvoicePages,
    InvoiceDataCaptureToolkitStoreSlice,
    selectDocumentLayoutItems,
    selectInvoiceFieldDataAnnotation,
    toggleCompareBoxVisibility
} from "./InvoiceDataCaptureToolkitStoreSlice";
import {
    emptyStateSelector,
    errorConfirmedSelector,
    errorSelector,
    isAnyHttpRequestPendingSelector
} from "./selectors";
import { defaultDocumentViewState, IInvoiceDataAnnotation, IInvoiceDocumentPage } from "./state";

describe(InvoiceDataCaptureToolkitStoreSlice.name, () => {
    beforeEach(() => {
        cleanStore();
    });

    test("default state should be empty state", () => {
        expect(emptyStateSelector(rootState())).toBeTruthy();
    });

    describe("FetchInvoiceDataAsync thunk action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("should clean toolkit state when FetchInvoiceDataAsync execution will start", () => {
            expect(rootState().invoiceFields).toBeUndefined();

            rootStore.dispatch(fetchInvoiceDataAsync.pending({} as any, 0));

            expect(rootState().invoiceFields).toBeUndefined();
        });

        test("should change loading state on start of fetching data and then switch off loading state", () => {
            rootStore.dispatch(fetchInvoiceDataAsync.pending({} as any, 0));

            expect(emptyStateSelector(rootState())).toBeFalsy();
            expect(isAnyHttpRequestPendingSelector(rootState())).toBeTruthy();

            rootStore.dispatch(fetchInvoiceDataAsync.fulfilled({} as any, "synthetic_test", 0));

            expect(isAnyHttpRequestPendingSelector(rootState())).toBeFalsy();
        });

        test("should catch data loading error and set error confirmation state", () => {
            const testError = new Error("test error");
            rootStore.dispatch(fetchInvoiceDataAsync.rejected(testError, "synthetic_test", 0));

            expect(errorSelector(rootState())).toBeDefined();
            expect(errorConfirmedSelector(rootState())).toBeFalsy();

            rootStore.dispatch(confirmError());

            expect(errorConfirmedSelector(rootState())).toBeTruthy();
        });
    });

    describe("fetchInvoiceDataAsyncFulfilled action", () => {
        beforeEach(() => {
            cleanStore();
        });
        /*
        test("should synchronize Invoice Fields with provided Invoice Data Annotation", () => {
            const testVendorName = "sample vendor name";
            const testVendorEmail = "sample vendor email";
            const testVendorPhone = "12345678";

            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: 1,
                        fieldValue: testVendorName,
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    },
                    {
                        fieldId: 2,
                        fieldValue: testVendorEmail,
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    },
                    {
                        fieldId: 3,
                        fieldValue: testVendorPhone + "90",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    },
                    {
                        fieldId: "wrong key" as any,
                        fieldValue: "no text",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    }
                ]
            };

            expect(rootState().invoiceFields).toBeUndefined();
            expect(rootState().invoiceDataAnnotation).toBeUndefined();

            const initialInvoiceFields = { ...defaultInvoiceFieldsState, vendorPhone: testVendorPhone };

            rootStore.dispatch(getInvoiceFields(initialInvoiceFields));
            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            expect(rootState().invoiceFields).toStrictEqual(initialInvoiceFields);
            expect(rootState().invoiceDataAnnotation).toStrictEqual(exampleInvoiceDataAnnotation);

            rootStore.dispatch(fetchInvoiceDataAsync.fulfilled({} as any, "synthetic_test", 0));

            expect(rootState().invoiceFields!).toStrictEqual({
                ...initialInvoiceFields,
                vendorName: testVendorName,
                vendorEmail: testVendorEmail
            });
        });
        */
    });

    describe("getInvoicePages action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("does nothing when undefined payload provided", () => {
            const cleanState = rootState();

            rootStore.dispatch(getInvoicePages(null as any));

            expect(rootState()).toStrictEqual(cleanState);
        });

        test("set pages array to document view state when empty page array provided", () => {
            expect(rootState().documentView).toEqual(defaultDocumentViewState);
            expect(rootState().documentView.pages).toBeUndefined();

            rootStore.dispatch(getInvoicePages([]));

            expect(rootState().documentView).not.toEqual(defaultDocumentViewState);
            expect(rootState().documentView.pages).toStrictEqual([]);
        });

        test("set pages, first page number and page count to document view state and update LayoutItem assignment when array with pages and data annotations provided", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["1"]
                    }
                ]
            };

            const examplePages: IInvoiceDocumentPage[] = [
                {
                    id: 1,
                    number: 1,
                    height: 0,
                    width: 0,
                    imageFileId: ""
                },
                {
                    id: 2,
                    number: 2,
                    height: 0,
                    width: 0,
                    imageFileId: "",
                    pageLayoutItems: [
                        {
                            id: "1",
                            assigned: false,
                            selected: false,
                            text: "",
                            value: "",
                            bottomRight: {
                                x: 1,
                                y: 1
                            },
                            topLeft: {
                                x: 1,
                                y: 1
                            }
                        },
                        {
                            id: "2",
                            assigned: false,
                            selected: false,
                            text: "",
                            value: "",
                            bottomRight: {
                                x: 1,
                                y: 1
                            },
                            topLeft: {
                                x: 1,
                                y: 1
                            }
                        }
                    ]
                }
            ];

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages(examplePages));

            const documentViewState = rootState().documentView;
            const secondPage = documentViewState.pages?.find((page) => page.id === 2);

            expect(documentViewState.pages?.map((page) => page.id)).toEqual(examplePages.map((page) => page.id));
            expect(documentViewState.currentPageNumber).toBe(1);
            expect(documentViewState.pageCount).toBe(2);
            expect(secondPage!.pageLayoutItems!.find((item) => item.assigned && item.id === "1")).toBeDefined();
            expect(secondPage!.pageLayoutItems!.find((item) => !item.assigned && item.id === "2")).toBeDefined();
        });
    });

    describe("getCleanProcessingResults action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("set InvoiceProcessingResult object from payload", () => {
            expect(rootState().cleanProcessingResults).toBeUndefined();

            rootStore.dispatch(getCleanProcessingResults({} as any));

            expect(rootState().cleanProcessingResults).toEqual({});
        });
    });

    describe("getInvoiceDataAnnotation action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("set InvoiceDataAnnotation object from payload", () => {
            expect(rootState().invoiceDataAnnotation).toBeUndefined();

            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "sample vendor name",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    }
                ]
            };

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));

            expect(rootState().invoiceDataAnnotation).toStrictEqual(exampleInvoiceDataAnnotation);
        });
    });

    describe("toggleCompareBoxVisibility action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("toggle compareBoxesVisible flag in document view state", () => {
            expect(rootState().documentView.compareBoxesVisible).toBeTruthy();

            rootStore.dispatch(toggleCompareBoxVisibility());

            expect(rootState().documentView.compareBoxesVisible).toBeFalsy();
        });
    });

    describe("changeCurrentPage action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("does nothing when pages are not provided", () => {
            expect(rootState().documentView).toEqual(defaultDocumentViewState);

            rootStore.dispatch(changeCurrentPage({ pageNumber: 1, autoScroll: true }));

            expect(rootState().documentView).toEqual(defaultDocumentViewState);
        });

        test("switch current page when pageNumber is valid", () => {
            expect(rootState().documentView.currentPageNumber).toBe(defaultDocumentViewState.currentPageNumber);

            const nextPageNumber = 2;
            const examplePages: IInvoiceDocumentPage[] = [
                {
                    id: 1,
                    number: 1,
                    height: 0,
                    width: 0,
                    imageFileId: ""
                },
                {
                    id: 2,
                    number: 2,
                    height: 0,
                    width: 0,
                    imageFileId: ""
                }
            ];

            rootStore.dispatch(getInvoicePages(examplePages));
            rootStore.dispatch(changeCurrentPage({ pageNumber: nextPageNumber, autoScroll: false }));

            expect(rootState().documentView.currentPageNumber).toBe(nextPageNumber);
            expect(
                rootState().documentView.pages!.find((page) => page.current && page.number === nextPageNumber)
            ).toBeDefined();
        });

        test("doesn't switch page when pageNumber is invalid", () => {
            expect(rootState().documentView.currentPageNumber).toBe(defaultDocumentViewState.currentPageNumber);

            const nextPageNumber = 3;
            const examplePages: IInvoiceDocumentPage[] = [
                {
                    id: 1,
                    number: 1,
                    height: 0,
                    width: 0,
                    imageFileId: ""
                },
                {
                    id: 2,
                    number: 2,
                    height: 0,
                    width: 0,
                    imageFileId: ""
                }
            ];

            rootStore.dispatch(getInvoicePages(examplePages));
            rootStore.dispatch(changeCurrentPage({ pageNumber: nextPageNumber, autoScroll: false }));

            expect(rootState().documentView.currentPageNumber).toBe(1);
            expect(rootState().documentView.pages!.find((page) => page.current && page.number === 1)).toBeDefined();
        });
    });

    describe("changeInvoiceFieldDataAnnotation action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("should update existing invoice field data annotation", () => {
            const testFieldValue = "sample vendor name";

            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    }
                ]
            };

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));

            expect(
                rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation.find(
                    (annotation) => annotation.fieldValue === ""
                )
            ).toBeDefined();

            rootStore.dispatch(
                changeInvoiceFieldDataAnnotation({
                    ...exampleInvoiceDataAnnotation.invoiceFieldsAnnotation[0],
                    fieldValue: testFieldValue
                })
            );

            expect(
                rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation.find(
                    (annotation) => annotation.fieldValue === testFieldValue
                )
            ).toBeDefined();
        });

        test("should add new invoice field data annotation item if the toolkit state won't have items of the same fieldType", () => {
            const testFieldValue = "sample email";

            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    }
                ]
            };

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation).toHaveLength(1);

            rootStore.dispatch(
                changeInvoiceFieldDataAnnotation({
                    ...exampleInvoiceDataAnnotation.invoiceFieldsAnnotation[0],
                    fieldId: "2",
                    fieldValue: testFieldValue
                })
            );

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation).toHaveLength(2);
            expect(
                rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation.find(
                    (annotation) => annotation.fieldValue === testFieldValue
                )
            ).toBeDefined();
        });

        test("should update page layout items assignment state", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["1"]
                    }
                ]
            };

            const examplePages: IInvoiceDocumentPage[] = [
                {
                    id: 1,
                    number: 1,
                    height: 0,
                    width: 0,
                    imageFileId: "",
                    pageLayoutItems: [
                        {
                            id: "1",
                            assigned: false,
                            selected: false,
                            text: "",
                            value: "",
                            bottomRight: {
                                x: 1,
                                y: 1
                            },
                            topLeft: {
                                x: 1,
                                y: 1
                            }
                        },
                        {
                            id: "2",
                            assigned: false,
                            selected: false,
                            text: "",
                            value: "",
                            bottomRight: {
                                x: 1,
                                y: 1
                            },
                            topLeft: {
                                x: 1,
                                y: 1
                            }
                        }
                    ]
                }
            ];

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages(examplePages));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation).toHaveLength(1);
            expect(
                rootState().documentView.pages!.find(
                    (page) =>
                        page.pageLayoutItems!.find((item) => item.assigned && item.id === "1") !== undefined &&
                        page.pageLayoutItems!.find((item) => !item.assigned && item.id === "2") !== undefined
                )
            ).toBeDefined();

            rootStore.dispatch(
                changeInvoiceFieldDataAnnotation({
                    ...exampleInvoiceDataAnnotation.invoiceFieldsAnnotation[0],
                    documentLayoutItemIds: ["2"]
                })
            );

            expect(
                rootState().documentView.pages!.find(
                    (page) =>
                        page.pageLayoutItems!.find((item) => !item.assigned && item.id === "1") !== undefined &&
                        page.pageLayoutItems!.find((item) => item.assigned && item.id === "2") !== undefined
                )
            ).toBeDefined();
        });
    });

    describe("selectInvoiceFieldDataAnnotation action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("should do nothing when invoice field data annotation is missing", () => {
            const testFieldId = "1";

            rootStore.dispatch(getInvoiceDataAnnotation(undefined as any));

            expect(rootState().invoiceDataAnnotation).toBeUndefined();

            rootStore.dispatch(selectInvoiceFieldDataAnnotation(testFieldId));

            expect(rootState().invoiceDataAnnotation).toBeUndefined();
        });
        test("should update invoice field data annotation selection state", () => {
            const testFieldType = "1";

            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: testFieldType,
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    }
                ]
            };

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeFalsy();

            rootStore.dispatch(selectInvoiceFieldDataAnnotation(testFieldType));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeTruthy();
        });

        test("should update page layout items focus state", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["1"]
                    }
                ]
            };

            const examplePages: IInvoiceDocumentPage[] = [
                {
                    id: 1,
                    number: 1,
                    height: 0,
                    width: 0,
                    imageFileId: "",
                    pageLayoutItems: [
                        {
                            id: "1",
                            assigned: false,
                            selected: false,
                            text: "",
                            value: "",
                            bottomRight: {
                                x: 1,
                                y: 1
                            },
                            topLeft: {
                                x: 1,
                                y: 1
                            }
                        }
                    ]
                }
            ];

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages(examplePages));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeFalsy();
            expect(rootState().documentView.pages![0].pageLayoutItems![0].inFocus).toBeUndefined();

            rootStore.dispatch(selectInvoiceFieldDataAnnotation("1"));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeTruthy();
            expect(rootState().documentView.pages![0].pageLayoutItems![0].inFocus).toBeTruthy();
        });
    });

    describe("clearInvoiceFieldDataAnnotationSelection action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("should clear invoice field data annotation selection state", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: true,
                        userCreated: false,
                        documentLayoutItemIds: []
                    }
                ]
            };

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeTruthy();

            rootStore.dispatch(clearInvoiceFieldDataAnnotationSelection());

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeFalsy();
        });

        test("should clear page layout items focus state", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["1"]
                    }
                ]
            };

            const examplePages: IInvoiceDocumentPage[] = [
                {
                    id: 1,
                    number: 1,
                    height: 0,
                    width: 0,
                    imageFileId: "",
                    pageLayoutItems: [
                        {
                            id: "1",
                            assigned: false,
                            selected: false,
                            text: "",
                            value: "",
                            bottomRight: {
                                x: 1,
                                y: 1
                            },
                            topLeft: {
                                x: 1,
                                y: 1
                            }
                        }
                    ]
                }
            ];

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages(examplePages));

            rootStore.dispatch(selectInvoiceFieldDataAnnotation("1"));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeTruthy();
            expect(rootState().documentView.pages![0].pageLayoutItems![0].inFocus).toBeTruthy();

            rootStore.dispatch(clearInvoiceFieldDataAnnotationSelection());

            expect(rootState().documentView.pages![0].pageLayoutItems![0].inFocus).toBeUndefined();
        });
    });

    describe("confirmError action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("change error confirmation state when error exists", () => {
            const testError = new Error("test error");
            rootStore.dispatch(fetchInvoiceDataAsync.rejected(testError, "synthetic_test", 0));

            expect(rootState().error).toBeDefined();
            expect(rootState().error!.confirm).toBeFalsy();

            rootStore.dispatch(confirmError());

            expect(rootState().error!.confirm).toBeTruthy();
        });

        test("does nothing when error state is empty", () => {
            const cleanState = rootState();

            rootStore.dispatch(confirmError());

            expect(rootState()).toEqual(cleanState);
        });
    });

    describe("clearInvoiceFields action", () => {
        beforeEach(() => {
            cleanStore();
        });
        /*
        test("clear all Invoice Fields values", () => {
            const sampleInvoiceFields: IInvoiceFields = {
                ...defaultInvoiceFieldsState,
                vendorName: "sample vendor name",
                vendorEmail: "sample vendor email"
            };

            rootStore.dispatch(getInvoiceFields(sampleInvoiceFields));

            expect(rootState().invoiceFields).toEqual(sampleInvoiceFields);

            rootStore.dispatch(clearInvoiceFields());

            expect(rootState().invoiceFields).toEqual(defaultInvoiceFieldsState);
        });
*/
        test("clear invoice field data annotation items and document layout items assignment", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["1"]
                    }
                ]
            };

            const examplePages: IInvoiceDocumentPage[] = [
                {
                    id: 1,
                    number: 1,
                    height: 0,
                    width: 0,
                    imageFileId: "",
                    pageLayoutItems: [
                        {
                            id: "1",
                            assigned: false,
                            selected: false,
                            text: "",
                            value: "",
                            bottomRight: {
                                x: 1,
                                y: 1
                            },
                            topLeft: {
                                x: 1,
                                y: 1
                            }
                        }
                    ]
                }
            ];

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages(examplePages));
            rootStore.dispatch(selectInvoiceFieldDataAnnotation("1"));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation[0].selected).toBeTruthy();
            expect(
                rootState().documentView.pages!.find(
                    (page) =>
                        page.pageLayoutItems!.find((item) => item.assigned && item.inFocus && item.id === "1") !==
                        undefined
                )
            ).toBeDefined();

            rootStore.dispatch(clearInvoiceFields());

            expect(
                rootState().documentView.pages!.find(
                    (page) =>
                        page.pageLayoutItems!.find(
                            (item) => !item.assigned && item.inFocus === undefined && item.id === "1"
                        ) !== undefined
                )
            ).toBeDefined();
        });
    });

    describe("selectDocumentLayoutItems action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("does nothing when document view pages state is empty", () => {
            const cleanState = rootState();

            rootStore.dispatch(selectDocumentLayoutItems([]));

            expect(rootState()).toEqual(cleanState);
        });

        test("change document layout items selection state by specified array of item id's and calculate selection plain text value", () => {
            const examplePage: IInvoiceDocumentPage = {
                id: 1,
                number: 1,
                height: 0,
                width: 0,
                imageFileId: "",
                pageLayoutItems: [
                    {
                        id: "1",
                        assigned: false,
                        selected: false,
                        text: "sample",
                        value: "sample",
                        bottomRight: {
                            x: 1,
                            y: 100
                        },
                        topLeft: {
                            x: 1,
                            y: 100
                        }
                    },
                    {
                        id: "2",
                        assigned: false,
                        selected: false,
                        text: "text",
                        value: "text",
                        bottomRight: {
                            x: 1,
                            y: 1
                        },
                        topLeft: {
                            x: 1,
                            y: 1
                        }
                    }
                ]
            };

            rootStore.dispatch(getInvoicePages([examplePage]));
            rootStore.dispatch(selectDocumentLayoutItems([...examplePage.pageLayoutItems!.map((item) => item.id)]));

            expect(rootState().documentView.pages![0].pageLayoutItems!.filter((item) => item.selected)).toHaveLength(
                examplePage.pageLayoutItems!.length
            );

            expect(rootState().documentView.selectedPlainTextValue).toStrictEqual("sample text");
        });

        test("clear selection when empty array of selected id's provided", () => {
            const examplePage: IInvoiceDocumentPage = {
                id: 1,
                number: 1,
                height: 0,
                width: 0,
                imageFileId: "",
                pageLayoutItems: [
                    {
                        id: "1",
                        assigned: false,
                        selected: true,
                        text: "",
                        value: "",
                        bottomRight: {
                            x: 1,
                            y: 1
                        },
                        topLeft: {
                            x: 1,
                            y: 1
                        }
                    },
                    {
                        id: "2",
                        assigned: false,
                        selected: true,
                        text: "",
                        value: "",
                        bottomRight: {
                            x: 1,
                            y: 1
                        },
                        topLeft: {
                            x: 1,
                            y: 1
                        }
                    }
                ]
            };

            rootStore.dispatch(getInvoicePages([examplePage]));

            expect(rootState().documentView.pages![0].pageLayoutItems!.filter((item) => item.selected)).toHaveLength(
                examplePage.pageLayoutItems!.length
            );

            rootStore.dispatch(selectDocumentLayoutItems([]));

            expect(rootState().documentView.pages![0].pageLayoutItems!.filter((item) => item.selected)).toHaveLength(0);
            expect(rootState().documentView.selectedPlainTextValue).toBeUndefined();
        });
    });

    describe("assignSelectedLayoutItemsToInvoiceField action", () => {
        beforeEach(() => {
            cleanStore();
        });

        test("should do nothing when data annotation and invoice fields states are empty", () => {
            const cleanState = rootState();

            rootStore.dispatch(assignSelectedLayoutItemsToInvoiceField("1"));

            expect(rootState()).toEqual(cleanState);
        });

        test("should do nothing when document layout item selection is empty", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "1",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: []
                    }
                ]
            };

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));

            const cleanState = rootState();

            rootStore.dispatch(assignSelectedLayoutItemsToInvoiceField("1"));

            expect(rootState()).toEqual(cleanState);
        });

        test("should do nothing when fieldType from action payload is undefined", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldId: "0",
                        fieldValue: "",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["1", "2"]
                    }
                ]
            };

            const examplePage: IInvoiceDocumentPage = {
                id: 1,
                number: 1,
                height: 0,
                width: 0,
                imageFileId: "",
                pageLayoutItems: [
                    {
                        id: "1",
                        assigned: false,
                        selected: false,
                        text: "sample",
                        value: "sample",
                        bottomRight: {
                            x: 1,
                            y: 100
                        },
                        topLeft: {
                            x: 1,
                            y: 100
                        }
                    },
                    {
                        id: "2",
                        assigned: false,
                        selected: false,
                        text: "text",
                        value: "value",
                        bottomRight: {
                            x: 1,
                            y: 1
                        },
                        topLeft: {
                            x: 1,
                            y: 1
                        }
                    }
                ]
            };

            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages([examplePage]));
            rootStore.dispatch(selectDocumentLayoutItems(["1", "2"]));

            const cleanState = rootState();

            rootStore.dispatch(assignSelectedLayoutItemsToInvoiceField("0"));

            expect(rootState()).toEqual(cleanState);
        });

        // test(`should set computed plain text from selection to field data annotation with specified fieldType
        //         and clean field value and assignment data from old data annotation when selection contains already assigned layout items`, () => {
        //     const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
        //         plainDocumentText: "",
        //         invoiceLineItemAnnotation: [],
        //         invoiceFieldsAnnotation: [
        //             {
        //                 fieldType: InvoiceFieldTypes.vendorEmail,
        //                 fieldValue: "sample vendor email",
        //                 selected: false,
        //                 userCreated: false,
        //                 documentLayoutItemIds: ["1"]
        //             },
        //             {
        //                 fieldType: InvoiceFieldTypes.vendorName,
        //                 fieldValue: "sample vendor name",
        //                 selected: false,
        //                 userCreated: false,
        //                 documentLayoutItemIds: ["2"]
        //             }
        //         ]
        //     };

        //     const examplePage: IInvoiceDocumentPage = {
        //         id: 1,
        //         number: 1,
        //         height: 0,
        //         width: 0,
        //         imageFileId: "",
        //         pageLayoutItems: [
        //             {
        //                 id: "1",
        //                 assigned: false,
        //                 selected: false,
        //                 text: "sample",
        //                 value: "sample",
        //                 bottomRight: {
        //                     x: 1,
        //                     y: 100
        //                 },
        //                 topLeft: {
        //                     x: 1,
        //                     y: 100
        //                 }
        //             },
        //             {
        //                 id: "2",
        //                 assigned: false,
        //                 selected: false,
        //                 text: "text",
        //                 value: "text",
        //                 bottomRight: {
        //                     x: 1,
        //                     y: 1
        //                 },
        //                 topLeft: {
        //                     x: 1,
        //                     y: 1
        //                 }
        //             }
        //         ]
        //     };

        //     rootStore.dispatch(getInvoiceFields(defaultInvoiceFieldsState));
        //     rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
        //     rootStore.dispatch(getInvoicePages([examplePage]));
        //     rootStore.dispatch(selectDocumentLayoutItems(["1", "2"]));

        //     rootStore.dispatch(assignSelectedLayoutItemsToInvoiceField({ fieldType: InvoiceFieldTypes.vendorName }));

        //     const emailFieldAnnotation = rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation.find(
        //         (annotation) => annotation.fieldType === InvoiceFieldTypes.vendorEmail
        //     )!;

        //     const vendorNameFieldAnnotation = rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation.find(
        //         (annotation) => annotation.fieldType === InvoiceFieldTypes.vendorName
        //     )!;

        //     expect(emailFieldAnnotation.fieldValue).toBe("");
        //     expect(emailFieldAnnotation.documentLayoutItemIds).toHaveLength(0);
        //     expect(emailFieldAnnotation.userCreated).toBeTruthy();
        //     expect(rootState().invoiceFields!.vendorEmail).toBe("");

        //     expect(vendorNameFieldAnnotation.fieldValue).toBe("sample text");
        //     expect(vendorNameFieldAnnotation.documentLayoutItemIds).toStrictEqual(["1", "2"]);
        //     expect(vendorNameFieldAnnotation.userCreated).toBeTruthy();
        //     expect(rootState().invoiceFields!.vendorName).toBe("sample text");
        // });
        /*
        test("should not change other field data annotations which won't have assignments to selected layout items", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceLineItemAnnotation: [],
                invoiceFieldsAnnotation: [
                    {
                        fieldType: InvoiceFieldTypes.vendorEmail,
                        fieldValue: "sample vendor email",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["1"]
                    },
                    {
                        fieldType: InvoiceFieldTypes.vendorName,
                        fieldValue: "sample vendor name",
                        selected: false,
                        userCreated: false,
                        documentLayoutItemIds: ["2"]
                    }
                ]
            };

            const examplePage: IInvoiceDocumentPage = {
                id: 1,
                number: 1,
                height: 0,
                width: 0,
                imageFileId: "",
                pageLayoutItems: [
                    {
                        id: "1",
                        assigned: false,
                        selected: false,
                        text: "sample",
                        value: "sample",
                        bottomRight: {
                            x: 1,
                            y: 100
                        },
                        topLeft: {
                            x: 1,
                            y: 100
                        }
                    },
                    {
                        id: "2",
                        assigned: false,
                        selected: false,
                        text: "text",
                        value: "text",
                        bottomRight: {
                            x: 1,
                            y: 1
                        },
                        topLeft: {
                            x: 1,
                            y: 1
                        }
                    }
                ]
            };

            rootStore.dispatch(getInvoiceFields(defaultInvoiceFieldsState));
            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages([examplePage]));
            rootStore.dispatch(selectDocumentLayoutItems(["2"]));

            rootStore.dispatch(assignSelectedLayoutItemsToInvoiceField({ fieldType: InvoiceFieldTypes.vendorName }));

            const emailFieldAnnotation = rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation.find(
                (annotation) => annotation.fieldType === InvoiceFieldTypes.vendorEmail
            )!;

            expect(emailFieldAnnotation).toStrictEqual(exampleInvoiceDataAnnotation.invoiceFieldsAnnotation[0]);
        });
*/
        /*
        test("should add new field data annotation when initial invoiceFieldsAnnotation array doesn't contain it", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceFieldsAnnotation: [],
                invoiceLineItemAnnotation: []
            };

            const examplePage: IInvoiceDocumentPage = {
                id: 1,
                number: 1,
                height: 0,
                width: 0,
                imageFileId: "",
                pageLayoutItems: [
                    {
                        id: "1",
                        assigned: false,
                        selected: false,
                        text: "sample",
                        value: "sample",
                        bottomRight: {
                            x: 1,
                            y: 100
                        },
                        topLeft: {
                            x: 1,
                            y: 100
                        }
                    },
                    {
                        id: "2",
                        assigned: false,
                        selected: false,
                        text: "text",
                        value: "text",
                        bottomRight: {
                            x: 1,
                            y: 1
                        },
                        topLeft: {
                            x: 1,
                            y: 1
                        }
                    }
                ]
            };

            rootStore.dispatch(getInvoiceFields(defaultInvoiceFieldsState));
            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages([examplePage]));
            rootStore.dispatch(selectDocumentLayoutItems(["1", "2"]));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation).toHaveLength(0);

            rootStore.dispatch(assignSelectedLayoutItemsToInvoiceField({ fieldType: InvoiceFieldTypes.vendorName }));

            expect(rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation).toHaveLength(1);

            const vendorNameAnnotation = rootState().invoiceDataAnnotation!.invoiceFieldsAnnotation.find(
                (annotation) => annotation.fieldType === InvoiceFieldTypes.vendorName
            )!;

            expect(vendorNameAnnotation.userCreated).toBeTruthy();
            expect(vendorNameAnnotation.selected).toBeFalsy();
            expect(vendorNameAnnotation.fieldValue).toBe("sample text");
            expect(vendorNameAnnotation.documentLayoutItemIds).toStrictEqual(["1", "2"]);
        });
*/
        /*
        test("should update document layout items assignment state", () => {
            const exampleInvoiceDataAnnotation: IInvoiceDataAnnotation = {
                plainDocumentText: "",
                invoiceFieldsAnnotation: [],
                invoiceLineItemAnnotation: []
            };

            const examplePage: IInvoiceDocumentPage = {
                id: 1,
                number: 1,
                height: 0,
                width: 0,
                imageFileId: "",
                pageLayoutItems: [
                    {
                        id: "1",
                        assigned: false,
                        selected: false,
                        text: "sample",
                        value: "sample",
                        bottomRight: {
                            x: 1,
                            y: 100
                        },
                        topLeft: {
                            x: 1,
                            y: 100
                        }
                    },
                    {
                        id: "2",
                        assigned: false,
                        selected: false,
                        text: "text",
                        value: "text",
                        bottomRight: {
                            x: 1,
                            y: 1
                        },
                        topLeft: {
                            x: 1,
                            y: 1
                        }
                    }
                ]
            };

            rootStore.dispatch(getInvoiceFields(defaultInvoiceFieldsState));
            rootStore.dispatch(getInvoiceDataAnnotation(exampleInvoiceDataAnnotation));
            rootStore.dispatch(getInvoicePages([examplePage]));
            rootStore.dispatch(selectDocumentLayoutItems(["1", "2"]));

            expect(
                rootState()
                    .documentView.pages![0].pageLayoutItems!.filter((item) => item.assigned)
                    .map((item) => item.id)
            ).toHaveLength(0);

            rootStore.dispatch(assignSelectedLayoutItemsToInvoiceField({ fieldType: InvoiceFieldTypes.vendorName }));

            expect(
                rootState()
                    .documentView.pages![0].pageLayoutItems!.filter((item) => item.assigned)
                    .map((item) => item.id)
            ).toStrictEqual(["1", "2"]);
        });
        */
    });

    let rootStore: ReturnType<typeof createRootStore>;

    const cleanStore = () => {
        rootStore = createRootStore();

        rootStore.dispatch(fetchInvoiceDataAsync.pending({} as any, 0));
        rootStore.dispatch(fetchInvoiceDataAsync.fulfilled({} as any, "synthetic_test", 0));
    };

    const rootState = () => {
        return rootStore.getState().invoiceDataCaptureToolkit;
    };
});
