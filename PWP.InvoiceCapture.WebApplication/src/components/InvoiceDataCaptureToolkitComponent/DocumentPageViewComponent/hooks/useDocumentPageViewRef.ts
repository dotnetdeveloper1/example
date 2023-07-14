import { useImperativeHandle, useRef } from "react";
import { ILayoutItem } from "../../store/state";
import { ReactRefType } from "./../../../../helperTypes";
import { DocumentPageViewComponent } from "./../DocumentPageViewComponent";

type DocumentPageViewRefType = ReactRefType<typeof DocumentPageViewComponent>;

interface IDocumentPageViewRefHookResult {
    componentElementRef: React.RefObject<HTMLDivElement>;
}

export function useDocumentPageViewRef(
    ref: React.Ref<DocumentPageViewRefType>,
    layoutItems: ILayoutItem[]
): IDocumentPageViewRefHookResult {
    const componentElementRef = useRef<HTMLDivElement>(null);

    useImperativeHandle<DocumentPageViewRefType, DocumentPageViewRefType>(
        ref,
        () => ({
            scrollIntoView: () => {
                if (componentElementRef && componentElementRef.current) {
                    componentElementRef.current.scrollIntoView({ behavior: "auto", block: "start" });
                }
            },
            getPageOffset: () => {
                if (componentElementRef && componentElementRef.current) {
                    return {
                        startY: componentElementRef.current.offsetTop,
                        endY: componentElementRef.current.offsetTop + componentElementRef.current.offsetHeight,
                        startX: componentElementRef.current.offsetLeft,
                        endX: componentElementRef.current.offsetLeft + componentElementRef.current.offsetWidth
                    };
                }

                return null;
            },
            getLayoutItem: (id) => {
                return layoutItems && layoutItems.find((item) => item.id === id);
            }
        }),
        [layoutItems]
    );

    return {
        componentElementRef: componentElementRef
    };
}
