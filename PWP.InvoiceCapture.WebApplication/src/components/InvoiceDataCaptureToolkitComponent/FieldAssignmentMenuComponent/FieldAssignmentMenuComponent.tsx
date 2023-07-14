import React from "react";
import { useTranslation } from "react-i18next";
import { RootMountComponent } from "../../common/RootMountComponent";
import { LineItemsFieldTypes } from "../store/state";
import "./FieldAssignmentMenuComponent.scss";
import { FieldSelectionList } from "./FieldSelectionList";
import { useFieldAssignmentMenu } from "./hooks";
import { ItemFieldsMenuComponent } from "./ItemsListAssignmentMenuComponent/ItemFieldsMenuComponent";
import { ItemsListMenuComponent } from "./ItemsListAssignmentMenuComponent/ItemsListMenuComponent";

interface FieldAssignmentMenuComponentProps {
    selectedLayoutItemsCount: number;
    selectedPlainText?: string;
    isOpen: boolean;
    left: number;
    top: number;
    onAssignItems(fieldId: string): void;
    onAssignLineItems(fieldType: LineItemsFieldTypes, orderNumber: number): void;
    onClose(): void;
}

export const COMPONENT_NAME = "FieldAssignmentMenuComponent";

export const FieldAssignmentMenuComponent: React.FunctionComponent<FieldAssignmentMenuComponentProps> = (props) => {
    const { t } = useTranslation();

    const {
        isEdge,
        topOffset,
        componentRef,
        onAssignField,
        onAssignLineItemField,
        onOpenListItems,
        onOpenItemFields,
        onEnterMainMenuField,
        itemFieldsMenuState,
        listItemsMenuState
    } = useFieldAssignmentMenu(props);

    const fieldsComputedStyles: React.CSSProperties = {
        top: topOffset,
        left: props.left
    };

    const headerLabelText = `${t("CONTEXT_MENU_HEADER->EXTRACTED_DATA_FROM")} ${props.selectedLayoutItemsCount} ${t(
        "CONTEXT_MENU_HEADER->ITEMS"
    )}:`;

    const menuHeaderClass = isEdge ? `${COMPONENT_NAME}__edge-header-label` : `${COMPONENT_NAME}__header-label`;

    return props.isOpen ? (
        <RootMountComponent>
            <div ref={componentRef} className={`${COMPONENT_NAME}`} style={fieldsComputedStyles}>
                <div className={`${COMPONENT_NAME}__header`}>
                    <div className={menuHeaderClass}>{headerLabelText}</div>
                    <div className={`${COMPONENT_NAME}__header-body`}>
                        <div className={`${COMPONENT_NAME}__plain-text`}>{props.selectedPlainText}</div>
                    </div>
                </div>
                <div className={`${COMPONENT_NAME}__fields`}>
                    <div className={`${COMPONENT_NAME}__fields-header`}>{t("CONTEXT_MENU_HEADER->ASSIGN_DATA")}</div>
                    <FieldSelectionList
                        listItemsMenuState={listItemsMenuState}
                        onAssignField={onAssignField}
                        onOpenListItems={onOpenListItems}
                        onEnterMainMenuField={onEnterMainMenuField}
                    />
                </div>
            </div>
            <ItemsListMenuComponent
                itemFieldsOpenedOrderNumber={itemFieldsMenuState.orderNumber}
                isOpen={listItemsMenuState.isOpen}
                onOpenListItemFields={onOpenItemFields}
            />
            <ItemFieldsMenuComponent
                orderNumber={itemFieldsMenuState.orderNumber}
                isOpen={itemFieldsMenuState.isOpen}
                onAssignField={onAssignLineItemField}
            />
        </RootMountComponent>
    ) : null;
};
