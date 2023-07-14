import { PageNotFound } from "../common/PageNotFound";
import { RequestNotAuthorized } from "../common/RequestNotAuthorized";
import { InvoiceDataCaptureToolkitComponent } from "../InvoiceDataCaptureToolkitComponent/InvoiceDataCaptureToolkitComponent";
import { InvoicesManagementComponent } from "../InvoicesManagementComponent/InvoicesManagementComponent";

export enum ApplicationRoutes {
    InvoiceDataCaptureToolkit = "invoice-capture/:invoiceId/:token",
    InvoiceManagementTool = "invoices",
    NotFound = "not-found",
    Unauthorized = "not-authorized"
}

export enum ApplicationRouteProperties {
    ReturnUrl = "returnUrl",
    Javascript = "javascript"
}

export interface IApplicationRoute {
    path: ApplicationRoutes;
    name: string;
    component:
        | typeof InvoicesManagementComponent
        | typeof InvoiceDataCaptureToolkitComponent
        | typeof PageNotFound
        | typeof RequestNotAuthorized;
    defaultPath?: string;
}

export function getRoutes(): IApplicationRoute[] {
    return [
        {
            path: ApplicationRoutes.InvoiceManagementTool,
            name: "Invoices",
            component: InvoicesManagementComponent,
            defaultPath: "invoices"
        },
        {
            path: ApplicationRoutes.InvoiceDataCaptureToolkit,
            name: "Invoice Capture & Matching Toolkit",
            component: InvoiceDataCaptureToolkitComponent
        },
        {
            path: ApplicationRoutes.NotFound,
            name: "Page Not Found",
            component: PageNotFound
        },
        {
            path: ApplicationRoutes.Unauthorized,
            name: "Request Not Authorized",
            component: RequestNotAuthorized
        }
    ];
}
