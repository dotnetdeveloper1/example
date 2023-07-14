import { PayloadAction } from "@reduxjs/toolkit";
import { InvoiceProcessingResult } from "../../../../api/models/InvoiceManagement/InvoiceProcessingResult";
import { IInvoiceDataCaptureToolkitState } from "../InvoiceDataCaptureToolkitState";

export function getCleanProcessingResultsReducer(
    state: IInvoiceDataCaptureToolkitState,
    action: PayloadAction<InvoiceProcessingResult>
): void {
    state.cleanProcessingResults = action.payload;
}
