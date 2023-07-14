import { useCallback } from "react";
import { useDispatch } from "react-redux";
import { BrowserIdentifier } from "../../../../../../utils/browserIdentifier";
import { switchToEditMode } from "../../../../store/InvoiceDataCaptureToolkitStoreSlice";
import { removeNoAssignmentLineItems } from "../../../../store/InvoiceDataCaptureToolkitStoreSlice";
import { tempLineItemsSelector, useToolkitSelector } from "../../../../store/selectors";

interface ITableMenuComponentHookResult {
    isEdge: boolean;
    switchToEditMode: () => void;
    isVisible: boolean;
}

export function useSwitchToEditModeComponent(): ITableMenuComponentHookResult {
    const tempLineItems = useToolkitSelector(tempLineItemsSelector);
    const dispatch = useDispatch();

    const switchToEditModeCallback = useCallback(() => {
        dispatch(switchToEditMode());
        dispatch(removeNoAssignmentLineItems());
    }, [dispatch]);

    return {
        isEdge: BrowserIdentifier.IsEdge(),
        switchToEditMode: switchToEditModeCallback,
        isVisible: tempLineItems && tempLineItems.length > 0
    };
}
