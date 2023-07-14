import { Action, configureStore, getDefaultMiddleware, ThunkAction } from "@reduxjs/toolkit";
import { enableES5 } from "immer";
import { load, save } from "redux-localstorage-simple";
import reduxLogger from "redux-logger";
import { InvoiceDataCaptureToolkitReducer } from "../components/InvoiceDataCaptureToolkitComponent/store/InvoiceDataCaptureToolkitStoreSlice";
import { settings } from "../settings/SettingsProvider";
import { POST_INVOICE_FILE_ASYNC_ACTION } from "./../components/InvoiceDataCaptureToolkitComponent/store/actions/Actions";

enableES5();

const serializableCheck = {
    ignoredActions: [POST_INVOICE_FILE_ASYNC_ACTION + "/pending", POST_INVOICE_FILE_ASYNC_ACTION + "/fulfilled"]
};

const middlewareCollection = settings.isDevelopment
    ? [
          reduxLogger,
          ...getDefaultMiddleware({
              serializableCheck: serializableCheck
          })
      ]
    : getDefaultMiddleware({
          serializableCheck: serializableCheck
      });

const localStoragePath = `PWP-IDC-${settings.version}`;

// TODO remove when local storage will be required
if (window && window.localStorage && window.localStorage.removeItem) {
    window.localStorage.removeItem(localStoragePath);
}

export const createRootStore = () => {
    return configureStore({
        preloadedState: load({
            namespace: localStoragePath
        }),
        reducer: {
            invoiceDataCaptureToolkit: InvoiceDataCaptureToolkitReducer
        },
        middleware: [
            ...middlewareCollection,
            save({
                namespace: localStoragePath
            })
        ],
        devTools: settings.isDevelopment
    });
};

export const rootStore = createRootStore();

export type RootState = ReturnType<typeof rootStore.getState>;
export type AppThunk<R, A extends Action<any>> = ThunkAction<R, RootState, void, A>;
