import React from "react";
import { ReactPropsType } from "../../helperTypes";
import { COMPONENT_NAME } from "./InvoiceDataCaptureToolkitComponent";
import { ToolkitHeaderComponent } from "./ToolkitHeaderComponent";

interface HeaderProps extends ReactPropsType<typeof ToolkitHeaderComponent> {}

export const Header: React.FunctionComponent<HeaderProps> = React.memo((props) => {
    return (
        <div className={`${COMPONENT_NAME}__header`}>
            <ToolkitHeaderComponent
                invoiceId={props.invoiceId}
                invoiceStatus={props.invoiceStatus}
                title={props.title}
                invoiceFileName={props.invoiceFileName}
            />
        </div>
    );
});
