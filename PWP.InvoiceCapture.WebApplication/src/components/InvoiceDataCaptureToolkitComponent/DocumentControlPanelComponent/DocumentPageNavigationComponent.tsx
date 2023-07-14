import React from "react";
import "./DocumentPageNavigationComponent.scss";
import { NextButton } from "./NextButton";
import { PageNavigationView } from "./PageNavigationView";
import { PreviousButton } from "./PreviousButton";

interface DocumentPageNavigationComponentProps {
    pageNumber: number;
    totalPages: number;
    onPageChange(page: number): void;
}

export const COMPONENT_NAME = "DocumentPageNavigationComponent";

export const DocumentPageNavigationComponent: React.FunctionComponent<DocumentPageNavigationComponentProps> = (
    props
) => {
    return (
        <div className={`${COMPONENT_NAME}`}>
            <PreviousButton pageNumber={props.pageNumber} onPageChange={props.onPageChange} />
            <PageNavigationView
                pageNumber={props.pageNumber}
                totalPages={props.totalPages}
                onPageChange={props.onPageChange}
            />
            <NextButton pageNumber={props.pageNumber} totalPages={props.totalPages} onPageChange={props.onPageChange} />
        </div>
    );
};
