import React from "react";
import ReactDOM from "react-dom";

export class RootMountComponent extends React.Component<React.PropsWithChildren<any>> {
    private rootElement: HTMLDivElement;

    constructor(props: React.PropsWithChildren<any>) {
        super(props);
        this.rootElement = document.createElement("div");
    }

    public componentDidMount(): void {
        document.body.appendChild(this.rootElement);
    }

    public componentWillUnmount(): void {
        document.body.removeChild(this.rootElement);
    }

    public render(): React.ReactNode {
        return ReactDOM.createPortal(this.props.children, this.rootElement);
    }
}
