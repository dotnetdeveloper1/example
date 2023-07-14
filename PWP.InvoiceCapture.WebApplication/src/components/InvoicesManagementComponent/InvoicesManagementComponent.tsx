import React from "react";
import { useTranslation } from "react-i18next";
import { TranslationUtils } from "../../utils/translationUtils";
import { Header } from "../InvoiceDataCaptureToolkitComponent/Header";
import { invoiceFileNameSelector, useToolkitSelector } from "../InvoiceDataCaptureToolkitComponent/store/selectors";
import { ApplicationRouteProperties } from "../RootComponent/routerConfig";
import { useInvoiceManagement } from "./hooks/useInvoicesManagement";
import "./InvoicesManagementComponent.scss";
import { UploadInvoiceComponent } from "./UploadInvoiceComponent/UploadInvoiceComponent";

export const COMPONENT_NAME = "InvoicesManagementComponent";

export const InvoicesManagementComponent: React.FunctionComponent = React.memo(() => {
    const { t } = useTranslation();
    const { invoicesList, refreshToken, accessToken } = useInvoiceManagement();
    const invoiceFileName = useToolkitSelector(invoiceFileNameSelector);

    return (
        <div className={`${COMPONENT_NAME}`}>
            <Header title={t("INVOICES_TITLE")} invoiceFileName={invoiceFileName} />
            <div className={`${COMPONENT_NAME}__body`}>
                {invoicesList && invoicesList.length > 0 && (
                    <div className={`${COMPONENT_NAME}__table_container`}>
                        <table className={`${COMPONENT_NAME}__table`}>
                            <thead>
                                <tr>
                                    <th>{t("INVOICES_TABLE_TITLE->ID")}</th>
                                    <th>{t("INVOICES_TABLE_TITLE->INVOICE_NAME")}</th>
                                    <th>{t("INVOICES_TABLE_TITLE->INVOICE_STATUS")}</th>
                                    <th>{t("INVOICES_TABLE_TITLE->CREATED_DATE")}</th>
                                    <th>{t("INVOICES_TABLE_TITLE->MODIFIED_DATE")}</th>
                                </tr>
                            </thead>
                            <tbody>
                                {invoicesList.map((invoice) => (
                                    <tr
                                        key={`invoice-table-row-${invoice.id}`}
                                        onClick={() => {
                                            const windowUrl =
                                                `/invoice-capture/${invoice.id}/${refreshToken}` +
                                                `?${ApplicationRouteProperties.ReturnUrl}=${window.location.href}`;
                                            const newWindow = window.open(windowUrl, "_blank", "noopener,noreferrer");
                                            if (newWindow) {
                                                newWindow.opener = null;
                                            }
                                        }}>
                                        <td>{invoice.id}</td>
                                        <td>{invoice.name}</td>
                                        <td>
                                            {t(
                                                TranslationUtils.getInvoiceStatusLocalizationKeyByString(invoice.status)
                                            )}
                                        </td>
                                        <td>{invoice.createdDate}</td>
                                        <td>{invoice.modifiedDate}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
                <UploadInvoiceComponent accessToken={accessToken} />
            </div>
        </div>
    );
});
