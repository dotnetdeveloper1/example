import { mount } from "enzyme";
import React from "react";
import "../../../mocks/react-router";
import { ReactPropsType } from "./../../../helperTypes";
import { COMPONENT_NAME, ToolkitControlPanelComponent } from "./ToolkitControlPanelComponent";

describe(`${COMPONENT_NAME}`, () => {
    test("renders component", () => {
        const wrapper = mount(
            <ToolkitControlPanelComponent
                isSaveEnabled={propsMockObject.isSaveEnabled}
                isSubmitEnabled={propsMockObject.isSubmitEnabled}
                onSaveProcessingResult={propsMockObject.onSaveProcessingResult}
                onSubmitProcessingResult={propsMockObject.onSubmitProcessingResult}
                onClose={propsMockObject.onClose}
            />
        );
        const toolkitControlPanelElement = wrapper.find(ToolkitControlPanelComponent);
        expect(toolkitControlPanelElement.exists()).toBeTruthy();
    });
});

const propsMockObject: ReactPropsType<typeof ToolkitControlPanelComponent> = {
    isSaveEnabled: true,
    isSubmitEnabled: true,
    onSaveProcessingResult: jest.fn(),
    onSubmitProcessingResult: jest.fn(),
    onClose: jest.fn()
};
