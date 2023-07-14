import React, { Suspense } from "react";
import { Redirect, Route, RouteComponentProps, Switch, useRouteMatch } from "react-router";
// import { settings } from "../../settings/SettingsProvider";
import { ApplicationRoutes, getRoutes } from "./routerConfig";

interface RootComponentProps extends Partial<RouteComponentProps<any>> {}

export const COMPONENT_NAME = "RootComponent";

export const RootComponent: React.FunctionComponent<RootComponentProps> = (props) => {
    const match = useRouteMatch();

    const routes = getRoutes();

    return (
        <Suspense fallback={"Loading..."}>
            <Switch>
                <Redirect
                    exact={true}
                    from={match.url}
                    // TODO: uncomment this before release
                    // to={settings.isDevelopment ? `/${routes[0].defaultPath}` : `/${routes[1].defaultPath}`}
                    to={`/${routes[0].defaultPath}`}
                />
                {routes.map((route, i) => (
                    <Route key={i} path={`/${route.path}`} component={route.component} />
                ))}
                <Redirect from="*" to={`/${ApplicationRoutes.NotFound}`} />
            </Switch>
        </Suspense>
    );
};
