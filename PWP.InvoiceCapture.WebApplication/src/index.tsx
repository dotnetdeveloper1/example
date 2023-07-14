import { createBrowserHistory } from "history";
import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import { Router } from "react-router";
import { RootComponent } from "./components/RootComponent";
import "./localization/i18n";
import { settings } from "./settings/SettingsProvider";
import { rootStore } from "./store/configuration";
import "./styles/globals.scss";

const history = createBrowserHistory({ basename: settings.applicationBasePath });

ReactDOM.render(
    <Router history={history}>
        <Provider store={rootStore}>
            <RootComponent />
        </Provider>
    </Router>,
    document.getElementById("root")
);
