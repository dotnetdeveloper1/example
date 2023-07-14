import { useEffect, useMemo, useState } from "react";
import { ReactPropsType, ReactPropType, ReactRefType } from "./../../../../helperTypes";
import { IInvoiceDocumentPage, ILayoutItem, InvoiceStatus } from "./../../store/state";
import { DocumentPageViewComponent } from "./../DocumentPageViewComponent";
import { useDocumentPageViewRef } from "./useDocumentPageViewRef";

interface IDocumentPageViewHookResult extends ReturnType<typeof useDocumentPageViewRef> {
    page?: Pick<IInvoiceDocumentPage, "id" | "number" | "pageImageLink" | "current" | "autoScroll">;
    size: Pick<IInvoiceDocumentPage, "height" | "width">;
    layoutItems: ILayoutItem[];
    selectedLayoutItemIds: string[];
    isAssignmentMenuEnabled: boolean;
    scaleRatio: number;
}

export const DOCUMENT_VIEW_PAGE_PADDING_WIDTH_INTERVAL = 96;

export function useDocumentPageView(
    props: ReactPropsType<typeof DocumentPageViewComponent>,
    ref: React.Ref<ReactRefType<typeof DocumentPageViewComponent>>
): IDocumentPageViewHookResult {
    const { page, documentViewBox, invoiceStatus } = props;

    const selectedLayoutItemIds = getSelectedLayoutItemIds(page);

    const isAssignmentMenuEnabled = useMemo(() => invoiceStatus === InvoiceStatus.PendingReview, [invoiceStatus]);

    const [size, setSize] = useState<IDocumentPageViewHookResult["size"]>({
        width: (page && page.width) || 0,
        height: (page && page.height) || 0
    });
    const [layoutItems, setLayoutItems] = useState<IDocumentPageViewHookResult["layoutItems"]>(
        (page && page.pageLayoutItems) || []
    );
    const [scaleRatio, setScaleRatio] = useState<IDocumentPageViewHookResult["scaleRatio"]>(1);

    const { componentElementRef } = useDocumentPageViewRef(ref, layoutItems);

    useEffect(() => {
        if (page) {
            const { pageLayoutItems } = page;

            const { size: nextSize, layoutItems: nextLayoutItems, scaleRatio: currentScaleRatio } = nextPageLayout(
                page,
                pageLayoutItems || [],
                documentViewBox
            );

            setSize(nextSize);
            setLayoutItems(nextLayoutItems);
            setScaleRatio(currentScaleRatio);
        }
    }, [page, documentViewBox]);

    return {
        page: page && {
            id: page.id,
            number: page.number,
            pageImageLink: page.pageImageLink,
            current: page.current,
            autoScroll: page.autoScroll
        },
        size: size,
        layoutItems: layoutItems,
        selectedLayoutItemIds: selectedLayoutItemIds,
        componentElementRef: componentElementRef,
        isAssignmentMenuEnabled: isAssignmentMenuEnabled,
        scaleRatio: scaleRatio
    };
}

function nextPageLayout(
    page: IInvoiceDocumentPage,
    pageLayoutItems: ILayoutItem[],
    documentViewBox: ReactPropType<typeof DocumentPageViewComponent, "documentViewBox">
): Pick<IDocumentPageViewHookResult, "size" | "layoutItems" | "scaleRatio"> {
    // NOTE Scaling logic is used only for scaling down page size and page elements, scale ratio should be limited to 100%
    const scaleRatio = Math.min((documentViewBox.width - DOCUMENT_VIEW_PAGE_PADDING_WIDTH_INTERVAL) / page.width, 1);

    const nextPageSize = { width: scaleRatio * page.width, height: scaleRatio * page.height };

    const nextLayoutItems = pageLayoutItems.map((item) => ({
        ...item,
        topLeft: { x: scaleRatio * item.topLeft.x, y: scaleRatio * item.topLeft.y },
        bottomRight: { x: scaleRatio * item.bottomRight.x, y: scaleRatio * item.bottomRight.y }
    }));

    return { size: nextPageSize, layoutItems: nextLayoutItems, scaleRatio: scaleRatio };
}

function getSelectedLayoutItemIds(page: IInvoiceDocumentPage | undefined): string[] {
    const selectedLayoutItems =
        page && page.pageLayoutItems && page.pageLayoutItems.length > 0
            ? page.pageLayoutItems.filter((item) => item.selected)
            : [];

    return selectedLayoutItems.map((item) => item.id);
}
