import { mount } from "enzyme";
import React from "react";
import { EnzymeWrapperType, ReactPropsType } from "../../../helperTypes";
import { COMPONENT_NAME, DocumentPageImagePaneComponent } from "./DocumentPageImagePaneComponent";

describe(`${COMPONENT_NAME}`, () => {
    beforeEach(() => {
        mountDocumentPageImagePane();
    });

    test("renders component and display img tag", () => {
        const documentPageImagePaneImgElement = wrapper.find(DocumentPageImagePaneComponent).find("img");

        expect(documentPageImagePaneImgElement.exists()).toBeTruthy();
        expect(documentPageImagePaneImgElement.props().width).toBe(propsMockObject.width);
    });

    let wrapper: EnzymeWrapperType<typeof DocumentPageImagePaneComponent>;

    const propsMockObject: ReactPropsType<typeof DocumentPageImagePaneComponent> = {
        pageNumber: 1,
        pageImageLink: "",
        width: 500,
        height: 500
    };

    const mountDocumentPageImagePane = () => {
        wrapper = mount(
            <DocumentPageImagePaneComponent
                pageNumber={propsMockObject.pageNumber}
                pageImageLink={propsMockObject.pageImageLink}
                width={propsMockObject.width}
                height={propsMockObject.height}
            />
        );
    };
});
