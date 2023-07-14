import React, { forwardRef, useCallback, useEffect, useRef, useState } from "react";
import { ReactPropsType } from "../../../helperTypes";
import { FieldAssignmentMenuComponent, useFieldAssignmentMenuControl } from "../FieldAssignmentMenuComponent";
import { IBox, IPoint } from "../store/state";
import { IInvoiceDocumentPage, ILayoutItem, InvoiceStatus } from "../store/state";
import { DocumentLayoutItemComponent } from "./DocumentLayoutItemComponent";
import { DocumentPageImagePaneComponent } from "./DocumentPageImagePaneComponent";
import "./DocumentPageViewComponent.scss";
import { useDocumentPageView } from "./hooks";
import { MultiSelectionPaneComponent } from "./MultiSelectionPaneComponent";
import { TableAssignmentComponent } from "./TableAssignmentComponent/TableAssignmentComponent";

interface DocumentPageViewComponentProps
    extends Pick<
            ReactPropsType<typeof FieldAssignmentMenuComponent>,
            "onAssignLineItems" | "onAssignItems" | "selectedPlainText"
        >,
        Pick<ReactPropsType<typeof MultiSelectionPaneComponent>, "onLayoutItemsSelect" | "containerScrollOffset">,
        Pick<ReactPropsType<typeof TableAssignmentComponent>, "onAssignColumnToLineItems"> {
    page?: IInvoiceDocumentPage;
    invoiceStatus: InvoiceStatus;
    showCompareBoxes: boolean;
    documentViewBox: {
        top: number;
        left: number;
        width: number;
        height: number;
    };
    tableSelectionMode: boolean;
    selectedTable?: IBox;
    tablePageNumber: number;
    onTableSelected: (table: IBox | undefined, pageNumber: number) => void;
    onTooltipWasShown: () => void;
    tooltipWasShown: boolean;
}

interface DocumentPageViewComponentRef {
    getPageOffset(): { startY: number; endY: number; startX: number; endX: number } | null;
    getLayoutItem(id: string): ILayoutItem | undefined;
    scrollIntoView(): void;
}

export const COMPONENT_NAME = "DocumentPageViewComponent";

const DocumentPageViewComponentCore: React.ForwardRefRenderFunction<
    DocumentPageViewComponentRef,
    DocumentPageViewComponentProps
> = (props, ref) => {
    const {
        page,
        size,
        layoutItems,
        selectedLayoutItemIds,
        componentElementRef,
        isAssignmentMenuEnabled,
        scaleRatio
    } = useDocumentPageView(props, ref);
    const { contextMenuState, onContextMenuOpen, onContextMenuClose } = useFieldAssignmentMenuControl(props);
    const documentPageContentRef = useRef<HTMLDivElement>(null);
    const [initialScaleRatio, setInitialScaleRatio] = useState<number>(1);

    const [currentPageOffset, setCurrentPageOffset] = useState<IPoint>({
        x: documentPageContentRef.current?.getBoundingClientRect().x || 0,
        y: documentPageContentRef.current?.getBoundingClientRect().y || 0
    });

    const onTableSelected = useCallback(
        (table?: IBox) => {
            props.onTableSelected(table, page ? page.number : 0);
            setInitialScaleRatio(scaleRatio);
        },
        [page, props, scaleRatio]
    );

    useEffect(() => {
        setCurrentPageOffset({
            x: documentPageContentRef.current?.getBoundingClientRect().x || 0,
            y: documentPageContentRef.current?.getBoundingClientRect().y || 0
        });
    }, [scaleRatio, props.containerScrollOffset]);

    return (
        <>
            {page && (
                <div ref={componentElementRef} className={`${COMPONENT_NAME}`}>
                    <div ref={documentPageContentRef} className={`${COMPONENT_NAME}__content`}>
                        <DocumentPageImagePaneComponent
                            pageNumber={page.number}
                            pageImageLink={page.pageImageLink || ""}
                            width={size.width}
                            height={size.height}
                        />
                        {layoutItems && layoutItems.length > 0 && (
                            <MultiSelectionPaneComponent
                                selectedIds={selectedLayoutItemIds}
                                showCompareBoxes={props.showCompareBoxes}
                                containerScrollOffset={props.containerScrollOffset}
                                onLayoutItemsSelect={props.onLayoutItemsSelect}
                                tableSelectionMode={props.tableSelectionMode}
                                onTableSelected={onTableSelected}>
                                {layoutItems.map((layoutItem) => (
                                    <DocumentLayoutItemComponent
                                        key={`layout-item-${layoutItem.id}`}
                                        id={layoutItem.id}
                                        topLeft={layoutItem.topLeft}
                                        bottomRight={layoutItem.bottomRight}
                                        text={layoutItem.text}
                                        value={layoutItem.value}
                                        displayed={props.showCompareBoxes}
                                        assigned={layoutItem.assigned}
                                        inFocus={layoutItem.inFocus}
                                        selected={props.tableSelectionMode ? false : layoutItem.selected}
                                        onSelectedItemContextMenu={onContextMenuOpen}
                                    />
                                ))}
                            </MultiSelectionPaneComponent>
                        )}
                        {props.tableSelectionMode &&
                            props.selectedTable &&
                            props.page &&
                            props.page.number === props.tablePageNumber && (
                                <TableAssignmentComponent
                                    containerScrollOffset={props.containerScrollOffset}
                                    layoutItems={layoutItems}
                                    selectedTable={props.selectedTable}
                                    onAssignColumnToLineItems={props.onAssignColumnToLineItems}
                                    onTooltipWasShown={props.onTooltipWasShown}
                                    tooltipWasShown={props.tooltipWasShown}
                                    scaleRatio={scaleRatio}
                                    initialScaleRatio={initialScaleRatio}
                                    currentPageOffset={currentPageOffset}
                                />
                            )}
                        {selectedLayoutItemIds &&
                            selectedLayoutItemIds.length > 0 &&
                            props.showCompareBoxes &&
                            isAssignmentMenuEnabled && (
                                <FieldAssignmentMenuComponent
                                    selectedPlainText={props.selectedPlainText}
                                    selectedLayoutItemsCount={selectedLayoutItemIds.length}
                                    isOpen={contextMenuState.isOpen}
                                    left={contextMenuState.left}
                                    top={contextMenuState.top}
                                    onAssignItems={props.onAssignItems}
                                    onAssignLineItems={props.onAssignLineItems}
                                    onClose={onContextMenuClose}
                                />
                            )}
                    </div>
                </div>
            )}
        </>
    );
};

export const DocumentPageViewComponent = forwardRef(DocumentPageViewComponentCore);
