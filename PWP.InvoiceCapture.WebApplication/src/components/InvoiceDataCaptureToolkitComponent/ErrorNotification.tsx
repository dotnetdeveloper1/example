import React, { useCallback } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { InformationModal } from "../common/InformationModal";
import { useFetchInvoiceData } from "./hooks";
import { confirmError } from "./store/InvoiceDataCaptureToolkitStoreSlice";
import { errorSelector, useToolkitSelector } from "./store/selectors";

interface ErrorNotificationProps extends Pick<ReturnType<typeof useFetchInvoiceData>, "isErrorConfirmed"> {}

export const ErrorNotification: React.FunctionComponent<ErrorNotificationProps> = React.memo((props) => {
    const dispatch = useDispatch();

    const error = useToolkitSelector(errorSelector);

    const onModalClose = useCallback(() => dispatch(confirmError()), [dispatch]);

    const { t } = useTranslation();

    return !props.isErrorConfirmed && error ? (
        <InformationModal
            isOpen={!props.isErrorConfirmed}
            title={t("INVOICE_LOADING_ERROR")}
            description={`Error: ${error.message || JSON.stringify({ ...error, confirm: undefined })}`}
            onClose={onModalClose}
        />
    ) : null;
});
