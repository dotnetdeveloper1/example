import { useCallback, useEffect, useState } from "react";
import { useDocumentView } from ".";
import { ReactRefType } from "../../../../helperTypes";
import { DocumentPageViewComponent } from "../../DocumentPageViewComponent";
import { IInvoiceDocumentPage, ILayoutItem } from "../../store/state";
import { BrowserIdentifier } from "./../../../../utils/browserIdentifier";

type PageElementType = ReactRefType<typeof DocumentPageViewComponent>;
type DocumentPagesType = ReturnType<typeof useDocumentView>["pages"];
type DocumentViewPageElementsType = ReturnType<typeof useDocumentView>["pageElements"];
type DocumentViewBoxType = ReturnType<typeof useDocumentView>["documentViewBox"];
type ScrollOffsetType = ReturnType<typeof useDocumentView>["scrollOffset"];

interface IOutOfViewLayoutItemInFocus {
    scrollTo: { x: number; y: number };
}

interface IDocumentLayoutItemFocusTrackingHookState {
    outOfViewItems: IOutOfViewLayoutItemInFocus[];
    itemsInFocus: string[];
    canAutoScroll: boolean;
}

interface IDocumentLayoutItemFocusTrackingHookResult {
    itemTrackingState: IDocumentLayoutItemFocusTrackingHookState;
    scrollToLayoutItemInFocus(pageContainerElement: React.RefObject<HTMLDivElement>): void;
}

const DOCUMENT_PADDING_TOP = 32; // same as css style in DocumentViewComponent.css -> padding: 32px 32px 0px 32px;

export function useDocumentLayoutItemFocusTracking(
    pages: DocumentPagesType,
    documentViewBox: DocumentViewBoxType,
    scrollOffset: ScrollOffsetType,
    pageElements: DocumentViewPageElementsType
): IDocumentLayoutItemFocusTrackingHookResult {
    const [state, setState] = useState<IDocumentLayoutItemFocusTrackingHookState>({
        outOfViewItems: [],
        itemsInFocus: [],
        canAutoScroll: false
    });

    const scrollToLayoutItemInFocus = useCallback(
        (pageContainerElement: React.RefObject<HTMLDivElement>) => {
            const pageContainerDiv = pageContainerElement && pageContainerElement.current;
            if (pageContainerDiv) {
                const closestLayoutItem = findClosestLayoutItem(pageContainerDiv, state.outOfViewItems);

                if (closestLayoutItem) {
                    scrollElement(pageContainerDiv, closestLayoutItem.scrollTo.y, closestLayoutItem.scrollTo.x);
                }

                setState({ ...state, canAutoScroll: false });
            }
        },
        [state]
    );

    useEffect(() => {
        const { changed, nextItemsInFocus } = isLayoutItemsInFocusChanged(pages, state);

        if (changed) {
            const { nextOutOfViewItems, nextCanAutoScroll } = getNextFocusTrackingState(
                pages,
                pageElements,
                nextItemsInFocus,
                scrollOffset,
                documentViewBox
            );

            setState({
                outOfViewItems: nextOutOfViewItems,
                itemsInFocus: nextItemsInFocus,
                canAutoScroll: nextCanAutoScroll
            });
        }
    }, [pages, documentViewBox, scrollOffset, pageElements, state]);

    return {
        itemTrackingState: state,
        scrollToLayoutItemInFocus: scrollToLayoutItemInFocus
    };
}

function scrollElement(element: HTMLDivElement, top: number, left: number): void {
    if (BrowserIdentifier.IsEdge()) {
        element.scrollTop = top;
        element.scrollLeft = left;
    } else {
        element.scroll(left, top);
    }
}

function getNextFocusTrackingState(
    pages: IInvoiceDocumentPage[] | undefined,
    pageElements: DocumentViewPageElementsType | undefined,
    layoutItemIdsInFocus: string[],
    scrollOffset: ScrollOffsetType,
    documentViewBox: DocumentViewBoxType
): {
    nextOutOfViewItems: IOutOfViewLayoutItemInFocus[];
    nextCanAutoScroll: boolean;
} {
    let nextVisibleItemsCount = 0;
    const nextOutOfViewItems: IOutOfViewLayoutItemInFocus[] = [];

    if (pageElements && pages && pages.length > 0 && layoutItemIdsInFocus.length > 0) {
        pages.forEach((page) => {
            const pageElement = page && pageElements[page.number];
            if (pageElement) {
                const { pageOffset, layoutItemsInFocus } = getPageLayoutState(pageElement, layoutItemIdsInFocus);
                if (pageOffset && scrollOffset && layoutItemsInFocus && layoutItemsInFocus.length > 0) {
                    layoutItemsInFocus.forEach((item) => {
                        const scrollPosition = getItemScrollPosition(item, pageOffset, scrollOffset, documentViewBox);

                        if (scrollPosition) {
                            nextOutOfViewItems.push({
                                scrollTo: scrollPosition
                            });
                        } else {
                            nextVisibleItemsCount++;
                        }
                    });
                }
            }
        });
    }

    return {
        nextOutOfViewItems: nextOutOfViewItems,
        nextCanAutoScroll: nextVisibleItemsCount === 0 && nextOutOfViewItems.length > 0
    };
}

function getPageLayoutState(
    pageElement: PageElementType,
    nextItemsInFocus: string[]
): {
    pageOffset: ReturnType<PageElementType["getPageOffset"]>;
    layoutItemsInFocus: ReturnType<PageElementType["getLayoutItem"]>[];
} {
    const pageOffset = pageElement.getPageOffset();
    const layoutItemsInFocus = nextItemsInFocus
        .map((id) => pageElement.getLayoutItem(id))
        .filter((item) => item !== undefined);

    return { pageOffset: pageOffset, layoutItemsInFocus: layoutItemsInFocus };
}

function getItemScrollPosition(
    item: ILayoutItem | undefined,
    pageOffset: { startX: number; startY: number },
    scrollOffset: ScrollOffsetType,
    documentViewBox: DocumentViewBoxType
): { x: number; y: number } | null {
    if (item) {
        const itemLeftOffset = pageOffset.startX + item.topLeft.x;
        const itemTopOffset = pageOffset.startY + item.topLeft.y;
        const itemRightOffset = pageOffset.startX + item.bottomRight.x;
        const itemBottomOffset = pageOffset.startY + item.bottomRight.y;

        const isItemInView =
            scrollOffset.scrollLeft <= itemRightOffset &&
            scrollOffset.scrollLeft + documentViewBox.width >= itemLeftOffset &&
            scrollOffset.scrollTop <= itemBottomOffset &&
            scrollOffset.scrollTop + documentViewBox.height - DOCUMENT_PADDING_TOP >= itemBottomOffset;

        const itemLeftScrollPosition = Math.max(
            itemLeftOffset + (itemRightOffset - itemLeftOffset) / 2 - documentViewBox.width / 2,
            0
        );
        const itemTopScrollPosition = Math.max(
            itemTopOffset + (itemBottomOffset - itemTopOffset) / 2 - documentViewBox.height / 2,
            0
        );

        if (!isItemInView) {
            return {
                x: itemLeftScrollPosition,
                y: itemTopScrollPosition
            };
        }
    }

    return null;
}

function isLayoutItemsInFocusChanged(
    pages: DocumentPagesType | undefined,
    state: IDocumentLayoutItemFocusTrackingHookState
): {
    changed: boolean;
    nextItemsInFocus: string[];
} {
    const layoutItemsInFocus: string[] = [];

    if (pages && pages.length > 0) {
        pages.forEach((page) => {
            if (page && page.pageLayoutItems && page.pageLayoutItems.length > 0) {
                const itemsInFocus = page.pageLayoutItems.filter((item) => item.inFocus) || [];
                layoutItemsInFocus.push(...itemsInFocus.map((item) => item.id));
            }
        });
    }

    return {
        changed: JSON.stringify(state.itemsInFocus) !== JSON.stringify(layoutItemsInFocus),
        nextItemsInFocus: layoutItemsInFocus
    };
}

function findClosestLayoutItem(
    pageContainerDiv: HTMLDivElement,
    outOfViewItems: IOutOfViewLayoutItemInFocus[]
): IOutOfViewLayoutItemInFocus | null {
    if (outOfViewItems.length > 0) {
        let minimalDistance = Number.MAX_SAFE_INTEGER;
        let closestLayoutItem = outOfViewItems[0];

        outOfViewItems.forEach((item) => {
            const itemScrollPositionDistance =
                Math.pow(pageContainerDiv.scrollTop - item.scrollTo.y, 2) +
                Math.pow(pageContainerDiv.scrollLeft - item.scrollTo.x, 2);

            if (minimalDistance > itemScrollPositionDistance) {
                minimalDistance = itemScrollPositionDistance;
                closestLayoutItem = item;
            }
        });

        return closestLayoutItem;
    }

    return null;
}
