import { useCallback } from "react";
import { useLocation } from "react-router-dom";
import { ApplicationRouteProperties, ApplicationRoutes } from "../../RootComponent/routerConfig";

interface ICloseInvoiceDataAnnotationsHookResult {
    onClose: () => void;
}

export function useCloseInvoiceDataAnnotations(): ICloseInvoiceDataAnnotationsHookResult {
    const location = useLocation();

    const onClose = useCallback(() => {
        const query = new URLSearchParams(location.search);
        const redirectPage = query.get(ApplicationRouteProperties.ReturnUrl);
        if (!redirectPage || redirectPage.length === 0) {
            window.location.replace(`/${ApplicationRoutes.NotFound}`);
            return;
        }
        if (isValidUrl(redirectPage)) {
            window.location.replace(redirectPage);
            return;
        }
        window.location.replace(`/${ApplicationRoutes.NotFound}`);
    }, [location.search]);

    const isValidUrl = (urlText: string) => {
        try {
            const containsJavascript = urlText?.toLowerCase().includes(ApplicationRouteProperties.Javascript);
            if (containsJavascript) {
                return false;
            }

            const url = new URL(urlText);
            return !!url;
        } catch (_) {
            return false;
        }
    };

    return {
        onClose: onClose
    };
}
