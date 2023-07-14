import React from "react";
import { ReactPropsType } from "../../../helperTypes";
import { DocumentPageViewComponent } from "../DocumentPageViewComponent";
import { IInvoiceDocumentPage } from "../store/state";
import "./DocumentViewComponent.scss";
import { useDocumentView } from "./hooks";
import { NoContentPlaceholder } from "./NoContentPlaceholder";

interface DocumentViewComponentProps
    extends Pick<
        ReactPropsType<typeof DocumentPageViewComponent>,
        | "showCompareBoxes"
        | "selectedPlainText"
        | "onLayoutItemsSelect"
        | "onAssignItems"
        | "onAssignLineItems"
        | "onAssignColumnToLineItems"
        | "invoiceStatus"
    > {
    pages?: IInvoiceDocumentPage[];
    onPageChange(page: number, autoScroll: boolean): void;
    tableSelectionMode: boolean;
}

export const COMPONENT_NAME = "DocumentViewComponent";

export const DocumentViewComponent: React.FunctionComponent<DocumentViewComponentProps> = (props) => {
    const {
        pages,
        pageContainerElement,
        showCompareBoxes,
        selectedPlainText,
        documentViewBox,
        scrollOffset,
        onLayoutItemsSelect,
        onPageContainerScroll,
        onSetPageElements,
        onAssignItems,
        onAssignLineItems,
        onAssignColumnToLineItems,
        selectedTable,
        tablePageNumber,
        onTableSelected,
        onTooltipWasShown,
        tooltipWasShown
    } = useDocumentView(props);

    return (
        <div className={`${COMPONENT_NAME}`}>
            {pages && pages.length > 0 ? (
                <div ref={pageContainerElement} className={`${COMPONENT_NAME}__pages`} onScroll={onPageContainerScroll}>
                    {pages.map((page) => (
                        <DocumentPageViewComponent
                            ref={(ref) => {
                                onSetPageElements(ref, page.number);
                            }}
                            key={`document-page-view-component-${page.id}`}
                            page={page}
                            showCompareBoxes={showCompareBoxes}
                            selectedPlainText={selectedPlainText}
                            onLayoutItemsSelect={onLayoutItemsSelect}
                            onAssignItems={onAssignItems}
                            onAssignLineItems={onAssignLineItems}
                            onAssignColumnToLineItems={onAssignColumnToLineItems}
                            onTooltipWasShown={onTooltipWasShown}
                            tooltipWasShown={tooltipWasShown}
                            containerScrollOffset={{ x: scrollOffset.scrollLeft, y: scrollOffset.scrollTop }}
                            documentViewBox={documentViewBox}
                            invoiceStatus={props.invoiceStatus}
                            tableSelectionMode={props.tableSelectionMode}
                            selectedTable={selectedTable}
                            tablePageNumber={tablePageNumber}
                            onTableSelected={onTableSelected}
                        />
                    ))}
                </div>
            ) : (
                <NoContentPlaceholder />
            )}
        </div>
    );
};
