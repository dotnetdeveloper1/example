import { useCallback, useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useHistory } from "react-router-dom";
import { settings } from "../../../settings/SettingsProvider";
import { useInterval } from "../../InvoiceDataCaptureToolkitComponent/hooks";
import { useToolkitSelector } from "../../InvoiceDataCaptureToolkitComponent/store/selectors";
import { isAnyHttpRequestPendingSelector } from "../../InvoiceDataCaptureToolkitComponent/store/selectors/index";
import { IInvoice } from "../../InvoiceDataCaptureToolkitComponent/store/state/IInvoice";
import { ApplicationRoutes } from "../../RootComponent/routerConfig";
import { identityApi } from "./../../../api/IdentityApiEndpoint";
import { TokenResponse } from "./../../../api/models/Identity/TokenResponse";
import { fetchInvoicesListAsync } from "./../../InvoiceDataCaptureToolkitComponent/store/InvoiceDataCaptureToolkitStoreSlice";
import { invoicesListSelector } from "./../../InvoiceDataCaptureToolkitComponent/store/selectors/index";

interface IUseInvoiceManagementHookResult {
    invoicesList: IInvoice[];
    isAnyHttpRequestPending: boolean;
    refreshToken: string;
    accessToken: string;
}

export function useInvoiceManagement(): IUseInvoiceManagementHookResult {
    const isAnyHttpRequestPending = useToolkitSelector(isAnyHttpRequestPendingSelector);
    const invoicesList = useToolkitSelector(invoicesListSelector);
    const dispatch = useDispatch();
    const history = useHistory();
    const [refreshToken, setRefreshToken] = useState("");
    const [accessToken, setAccessToken] = useState("");

    const getTokens = useCallback(async () => {
        const tokenRequestData = {
            grant_type: "password",
            username: settings.username,
            password: settings.password,
            client_id: settings.clientId,
            scope: "InvoiceManagement offline_access OcrDataAnalysis",
            client_secret: "password",
            tenantId: "1"
        };
        const urlEncodedData = Object.entries(tokenRequestData)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
            .join("&");

        let tokenResponse: TokenResponse | undefined;
        try {
            tokenResponse = await identityApi.token(urlEncodedData);
        } catch (exception) {
            history.push(`/${ApplicationRoutes.Unauthorized}`);
        }

        if (tokenResponse) {
            setAccessToken(tokenResponse.access_token);
            setRefreshToken(tokenResponse.refresh_token);
        }
    }, [history]);

    useEffect(() => {
        if (accessToken) {
            dispatch(fetchInvoicesListAsync(accessToken));
        }
    }, [accessToken, dispatch]);

    useInterval(getTokens, 2000, true);

    return {
        invoicesList: invoicesList,
        isAnyHttpRequestPending: isAnyHttpRequestPending,
        refreshToken: refreshToken,
        accessToken: accessToken
    };
}
