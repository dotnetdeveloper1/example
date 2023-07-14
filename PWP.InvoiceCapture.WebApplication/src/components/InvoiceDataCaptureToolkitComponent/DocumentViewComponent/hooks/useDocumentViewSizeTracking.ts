import { useCallback, useEffect, useState } from "react";
import { ReactPropType } from "./../../../../helperTypes";
import { DocumentPageViewComponent } from "./../../DocumentPageViewComponent";

type DocumentViewBoxType = ReactPropType<typeof DocumentPageViewComponent, "documentViewBox">;

interface IDocumentViewSizeTrackingHookResult {
    documentViewBox: DocumentViewBoxType;
}

export function useDocumentViewSizeTracking(
    pageContainerElement: React.RefObject<HTMLDivElement>
): IDocumentViewSizeTrackingHookResult {
    const [documentViewBox, setDocumentViewBoxState] = useState<DocumentViewBoxType>({
        top: 0,
        left: 0,
        height: 0,
        width: 0
    });

    const onPageContainerResize = useCallback(() => {
        const pageContainerBoundingBox =
            pageContainerElement.current && pageContainerElement.current.getBoundingClientRect();

        if (pageContainerBoundingBox) {
            setDocumentViewBoxState({
                top: pageContainerBoundingBox.y || pageContainerBoundingBox.top,
                left: pageContainerBoundingBox.x || pageContainerBoundingBox.left,
                height: pageContainerBoundingBox.height,
                width: pageContainerBoundingBox.width
            });
        }
    }, [pageContainerElement, setDocumentViewBoxState]);

    useEffect(() => {
        onPageContainerResize();

        window.addEventListener("resize", onPageContainerResize);

        return () => {
            window.removeEventListener("resize", onPageContainerResize);
        };
    }, [pageContainerElement, setDocumentViewBoxState, onPageContainerResize]);

    return {
        documentViewBox: documentViewBox
    };
}
