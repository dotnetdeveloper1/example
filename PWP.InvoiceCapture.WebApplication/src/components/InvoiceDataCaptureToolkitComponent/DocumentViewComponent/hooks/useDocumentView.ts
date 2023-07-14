import { debounce } from "lodash";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { ReactPropsType, ReactRefType } from "../../../../helperTypes";
import { DocumentPageViewComponent } from "../../DocumentPageViewComponent";
import { removeNoAssignmentLineItems } from "../../store/InvoiceDataCaptureToolkitStoreSlice";
import { IBox, IInvoiceDocumentPage } from "../../store/state";
import { DocumentViewComponent } from "../DocumentViewComponent";
import { useDocumentLayoutItemFocusTracking } from "./useDocumentLayoutItemFocusTracking";
import { useDocumentViewSizeTracking } from "./useDocumentViewSizeTracking";

type PageElementType = ReactRefType<typeof DocumentPageViewComponent> | null;

interface IPageElements {
    [pageNumber: number]: PageElementType;
}

interface IScrollOffset {
    scrollTop: number;
    scrollLeft: number;
}

interface IDocumentViewHookResult
    extends ReactPropsType<typeof DocumentViewComponent>,
        ReturnType<typeof useDocumentViewSizeTracking> {
    pageContainerElement: React.RefObject<HTMLDivElement>;
    scrollOffset: IScrollOffset;
    pageElements: IPageElements;
    onPageContainerScroll(event: React.UIEvent<HTMLDivElement>): void;
    onSetPageElements(ref: PageElementType, pageNumber: number): void;
    selectedTable?: IBox;
    tablePageNumber: number;
    onTableSelected: (table: IBox | undefined, pageNumber: number) => void;
    onTooltipWasShown: () => void;
    tooltipWasShown: boolean;
}

export const CHANGE_CURRENT_PAGE_DELAY = 250;
export const TABLE_MIN_SIZE = 5;

export function useDocumentView(props: ReactPropsType<typeof DocumentViewComponent>): IDocumentViewHookResult {
    const pageContainerElement = useRef<HTMLDivElement>(null);

    const [pageElements, setPageElements] = useState<IPageElements>({});
    const [scrollOffset, setScrollOffset] = useState<IScrollOffset>({
        scrollTop: 0,
        scrollLeft: 0
    });
    const [selectedTable, setSelectedTable] = useState<IBox | undefined>();
    const [tablePageNumber, setTablePageNumber] = useState<number>(0);
    const [tooltipWasShown, setTooltipWasShown] = useState<boolean>(false);

    const onTooltipWasShown = useCallback(() => {
        setTooltipWasShown(true);
    }, [setTooltipWasShown]);

    const onSetPageElements = useCallback(
        (ref: PageElementType, pageNumber: number) => {
            pageElements[pageNumber] = ref;
            setPageElements(pageElements);
        },
        [pageElements, setPageElements]
    );

    const onPageContainerScroll = useMemo(
        () => (event: React.UIEvent<HTMLDivElement>) => {
            setScrollOffset({ scrollTop: event.currentTarget.scrollTop, scrollLeft: event.currentTarget.scrollLeft });
            debounce(changeCurrentPageNumberOnScroll(props, pageElements), CHANGE_CURRENT_PAGE_DELAY)(
                event.currentTarget.scrollTop,
                event.currentTarget.offsetHeight
            );
        },
        [props, pageElements]
    );

    const onTableSelected = (table: IBox | undefined, pageNumber: number) => {
        const isSmallTable = table && table.height <= TABLE_MIN_SIZE && table.width <= TABLE_MIN_SIZE;
        if (isSmallTable) {
            setTablePageNumber(0);
            setSelectedTable(undefined);
        } else {
            setTablePageNumber(table ? pageNumber : 0);
            setSelectedTable(table);
        }
    };

    const { documentViewBox } = useDocumentViewSizeTracking(pageContainerElement);

    const { itemTrackingState, scrollToLayoutItemInFocus } = useDocumentLayoutItemFocusTracking(
        props.pages,
        documentViewBox,
        scrollOffset,
        pageElements
    );

    useEffect(() => {
        scrollCurrentPageIntoView(props, pageElements);
    }, [props, pageElements]);

    const dispatch = useDispatch();

    useEffect(() => {
        if (!props.tableSelectionMode) {
            setSelectedTable(undefined);
            dispatch(removeNoAssignmentLineItems());
        }
    }, [dispatch, props.tableSelectionMode]);

    useEffect(() => {
        if (itemTrackingState.canAutoScroll && props.showCompareBoxes) {
            scrollToLayoutItemInFocus(pageContainerElement);
        }
    }, [props, pageContainerElement, itemTrackingState, scrollToLayoutItemInFocus]);

    return {
        ...props,
        pageContainerElement: pageContainerElement,
        documentViewBox: documentViewBox,
        pageElements: pageElements,
        scrollOffset: scrollOffset,
        onPageContainerScroll: onPageContainerScroll,
        onSetPageElements: onSetPageElements,
        selectedTable: selectedTable,
        tablePageNumber: tablePageNumber,
        onTableSelected: onTableSelected,
        onTooltipWasShown: onTooltipWasShown,
        tooltipWasShown: tooltipWasShown
    };
}

function scrollCurrentPageIntoView(
    props: ReactPropsType<typeof DocumentViewComponent>,
    pageElements: IPageElements
): void {
    if (props.pages) {
        const currentPage = findCurrentPage(props.pages);

        if (currentPage && currentPage.autoScroll) {
            const currentPageElement = pageElements[currentPage.number];

            if (currentPageElement) {
                currentPageElement.scrollIntoView();
            }

            props.onPageChange(currentPage.number, false);
        }
    }
}

function changeCurrentPageNumberOnScroll(
    props: ReactPropsType<typeof DocumentViewComponent>,
    pageElements: IPageElements
): (scrollTop: number, scrollHeight: number) => void {
    return (scrollTopPosition: number, pageContainerHeight: number) => {
        if (props.pages) {
            const currentPage = findCurrentPage(props.pages);

            if (currentPage) {
                const currentPageElement = pageElements[currentPage.number];

                if (currentPageElement) {
                    const pageInContainerViewNumber = findClosestPageInContainerView(
                        props.pages,
                        pageElements,
                        currentPage.number,
                        scrollTopPosition,
                        pageContainerHeight
                    );

                    if (currentPage.number !== pageInContainerViewNumber) {
                        props.onPageChange(pageInContainerViewNumber, false);
                    }
                }
            }
        }
    };
}

function findClosestPageInContainerView(
    pages: IInvoiceDocumentPage[],
    pageElements: IPageElements,
    currentPageNumber: number,
    scrollStart: number,
    pageContainerHeight: number
): number {
    const scrollEndPosition = scrollStart + pageContainerHeight;

    let pageInViewNumber = currentPageNumber;
    // NOTE Select the most visible page in the page container
    let maxPageHeightInContainerView = Number.MIN_SAFE_INTEGER;

    pages.forEach((page) => {
        const pageElement = pageElements[page.number];
        if (pageElement) {
            const pageOffset = pageElement.getPageOffset();

            if (pageOffset && !isPageOutOfContainerView(pageElement, scrollStart, scrollEndPosition)) {
                const pageViewArea = findPageHeightInContainerView(
                    pageOffset.startY,
                    scrollStart,
                    pageOffset.endY,
                    scrollEndPosition
                );

                if (pageViewArea > maxPageHeightInContainerView) {
                    maxPageHeightInContainerView = pageViewArea;
                    pageInViewNumber = page.number;
                }
            }
        }
    });

    return pageInViewNumber;
}

function isPageOutOfContainerView(
    pageElement: ReactRefType<typeof DocumentPageViewComponent>,
    scrollStart: number,
    pageContainerHeight: number
): boolean {
    const scrollEnd = scrollStart + pageContainerHeight;
    const pageOffset = pageElement.getPageOffset();

    return (
        pageOffset !== null &&
        // NOTE: If visible page area lower than 10% of container area
        // then the current page is out of container view
        findPageHeightInContainerView(pageOffset.startY, scrollStart, pageOffset.endY, scrollEnd) /
            pageContainerHeight <
            0.1
    );
}

function findPageHeightInContainerView(
    pageOffsetStart: number,
    scrollTopPosition: number,
    pageOffsetEnd: number,
    scrollEndPosition: number
): number {
    return Math.min(pageOffsetEnd, scrollEndPosition) - Math.max(pageOffsetStart, scrollTopPosition);
}

function findCurrentPage(pages: IInvoiceDocumentPage[]): IInvoiceDocumentPage | undefined {
    return pages.find((page) => page.current);
}
