import { mount } from "enzyme";
import React from "react";
import { Provider } from "react-redux";
import "../../../mocks/react-router";
import { rootStore } from "../../../store/configuration";
import { InvoiceStatus } from "../store/state";
import { ReactPropsType } from "./../../../helperTypes";
import { COMPONENT_NAME, ToolkitHeaderComponent } from "./ToolkitHeaderComponent";

describe(`${COMPONENT_NAME}`, () => {
    test("renders component", () => {
        const wrapper = mount(
            <Provider store={rootStore}>
                <ToolkitHeaderComponent
                    invoiceId={propsMockObject.invoiceId}
                    invoiceStatus={propsMockObject.invoiceStatus}
                    title={propsMockObject.title}
                    invoiceFileName={propsMockObject.invoiceFileName}
                />
            </Provider>
        );
        const toolkitHeaderComponentElement = wrapper.find(ToolkitHeaderComponent);
        expect(toolkitHeaderComponentElement.exists()).toBeTruthy();
    });
});

const propsMockObject: ReactPropsType<typeof ToolkitHeaderComponent> = {
    invoiceId: 1,
    invoiceStatus: InvoiceStatus.PendingReview,
    title: "title",
    invoiceFileName: "invoice.pdf"
};
