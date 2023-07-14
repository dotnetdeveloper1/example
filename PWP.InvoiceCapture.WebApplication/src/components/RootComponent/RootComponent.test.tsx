import { mount } from "enzyme";
import React from "react";
import { Provider } from "react-redux";
import { MemoryRouter } from "react-router";
import { rootStore } from "../../store/configuration";
import { PageNotFound } from "../common/PageNotFound";
import { RequestNotAuthorized } from "../common/RequestNotAuthorized";
import { COMPONENT_NAME, RootComponent } from "./RootComponent";

describe(`${COMPONENT_NAME}`, () => {
    test("redirect to Page Not Found component when invalid url path received", () => {
        const wrapper = mount(
            <MemoryRouter initialEntries={["/test-invalid-route"]}>
                <Provider store={rootStore}>
                    <RootComponent />
                </Provider>
            </MemoryRouter>
        );

        const pageNotFoundElement = wrapper.find(PageNotFound);
        expect(pageNotFoundElement.exists()).toBeTruthy();
    });

    test("render Request Not Authorized page component for /not-authorized route", () => {
        const wrapper = mount(
            <MemoryRouter initialEntries={["/not-authorized"]}>
                <Provider store={rootStore}>
                    <RootComponent />
                </Provider>
            </MemoryRouter>
        );

        const requestNotAuthorizedElement = wrapper.find(RequestNotAuthorized);
        expect(requestNotAuthorizedElement.exists()).toBeTruthy();
    });
});
