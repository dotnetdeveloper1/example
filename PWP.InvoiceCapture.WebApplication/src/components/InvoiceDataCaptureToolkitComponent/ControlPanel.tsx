import React, { useCallback } from "react";
import { useDispatch } from "react-redux";
import { ReactPropsType } from "../../helperTypes";
import { DocumentControlPanelComponent } from "./DocumentControlPanelComponent";
import { COMPONENT_NAME } from "./InvoiceDataCaptureToolkitComponent";
import { selectDocumentLayoutItems, toggleCompareBoxVisibility } from "./store/InvoiceDataCaptureToolkitStoreSlice";
import { ToolkitControlPanelComponent } from "./ToolkitControlPanelComponent";

interface ControlPanelPropsType
    extends Pick<
            ReactPropsType<typeof DocumentControlPanelComponent>,
            "compareBoxes" | "currentPageNumber" | "onPageChange" | "pageCount" | "tableSelectionMode" | "invoiceStatus"
        >,
        ReactPropsType<typeof ToolkitControlPanelComponent> {}

export const ControlPanel: React.FunctionComponent<ControlPanelPropsType> = React.memo((props) => {
    const dispatch = useDispatch();

    const onToggleCompareBoxes = useCallback(() => {
        dispatch(toggleCompareBoxVisibility());
        dispatch(selectDocumentLayoutItems([]));
    }, [dispatch]);

    return (
        <div className={`${COMPONENT_NAME}__footer`}>
            <div className={`${COMPONENT_NAME}__document-control-panel-container`}>
                <DocumentControlPanelComponent
                    compareBoxes={props.compareBoxes}
                    currentPageNumber={props.currentPageNumber}
                    pageCount={props.pageCount}
                    onToggleCompareBoxes={onToggleCompareBoxes}
                    onPageChange={props.onPageChange}
                    tableSelectionMode={props.tableSelectionMode}
                    invoiceStatus={props.invoiceStatus}
                />
            </div>
            <div className={`${COMPONENT_NAME}__toolkit-control-panel-container`}>
                <ToolkitControlPanelComponent
                    isSaveEnabled={props.isSaveEnabled}
                    isSubmitEnabled={props.isSubmitEnabled}
                    onSaveProcessingResult={props.onSaveProcessingResult}
                    onSubmitProcessingResult={props.onSubmitProcessingResult}
                    onClose={props.onClose}
                />
            </div>
        </div>
    );
});
