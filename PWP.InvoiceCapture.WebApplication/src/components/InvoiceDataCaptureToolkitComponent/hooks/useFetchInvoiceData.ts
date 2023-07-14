import { useCallback, useEffect } from "react";
import { useDispatch } from "react-redux";
import { useHistory, useParams } from "react-router-dom";
import { ApplicationRoutes } from "../../RootComponent/routerConfig";
import { fetchInvoiceDataAsync } from "../store/InvoiceDataCaptureToolkitStoreSlice";
import {
    emptyStateSelector,
    errorConfirmedSelector,
    invoiceStatusSelector,
    isAnyHttpRequestPendingSelector,
    useToolkitSelector
} from "../store/selectors";
import { identityApi } from "./../../../api/IdentityApiEndpoint";
import { TokenResponse } from "./../../../api/models/Identity/TokenResponse";
import { settings } from "./../../../settings/SettingsProvider";
import { InvoiceStatus } from "./../store/state";

interface IFetchInvoiceDataHookResult {
    isStateEmpty: boolean;
    isAnyHttpRequestPending: boolean;
    invoiceStatus: InvoiceStatus;
    isErrorConfirmed: boolean;
    invoiceId: number | undefined;
}

declare global {
    interface Window {
        refreshToken: string;
        accessToken: string;
    }
}

export function useFetchInvoiceData(): IFetchInvoiceDataHookResult {
    const { invoiceId, token } = useParams();

    const dispatch = useDispatch();
    const history = useHistory();

    const isStateEmpty = useToolkitSelector(emptyStateSelector);
    const isAnyHttpRequestPending = useToolkitSelector(isAnyHttpRequestPendingSelector);
    const invoiceStatus = useToolkitSelector(invoiceStatusSelector);
    const isErrorConfirmed = useToolkitSelector(errorConfirmedSelector);

    const getAccessToken = useCallback(
        async (refreshToken: string) => {
            const tokenRequestData = {
                grant_type: "refresh_token",
                client_id: settings.clientId,
                client_secret: "password",
                refresh_token: refreshToken
            };
            const urlEncodedData = Object.entries(tokenRequestData)
                .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
                .join("&");

            let tokenApiResponse: TokenResponse | undefined;
            try {
                tokenApiResponse = await identityApi.token(urlEncodedData);
            } catch (exception) {
                history.push(`/${ApplicationRoutes.Unauthorized}`);
            }
            if (tokenApiResponse) {
                window.accessToken = tokenApiResponse.access_token;
            }
        },
        [history]
    );

    useEffect(() => {
        if (token) {
            window.refreshToken = token;
            const accessToken = window.accessToken;
            if (!accessToken) {
                getAccessToken(token).then((res) => {
                    if (invoiceId) {
                        dispatch(fetchInvoiceDataAsync(parseInt(invoiceId, 10)));
                    }
                });
            } else if (invoiceId) {
                dispatch(fetchInvoiceDataAsync(parseInt(invoiceId, 10)));
            }
        }
    }, [token, invoiceId, getAccessToken, dispatch]);

    return {
        invoiceStatus: invoiceStatus,
        isStateEmpty: isStateEmpty,
        isAnyHttpRequestPending: isAnyHttpRequestPending,
        isErrorConfirmed: isErrorConfirmed,
        invoiceId: invoiceId ? parseInt(invoiceId, 10) : undefined
    };
}
