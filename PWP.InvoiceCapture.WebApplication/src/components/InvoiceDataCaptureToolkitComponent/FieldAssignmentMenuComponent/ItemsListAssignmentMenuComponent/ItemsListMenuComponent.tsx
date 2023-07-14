import React from "react";
import { useTranslation } from "react-i18next";
import addNewItemImage from "../../../images/addIcon.svg";
import { COMPONENT_NAME as FIELD_ASSIGNMENT_COMPONENT_NAME } from "../FieldAssignmentMenuComponent";
import { useListItemsMenu } from "./hooks";
import { ListItem } from "./ListItem";

interface ItemsListMenuComponentProps {
    isOpen: boolean;
    itemFieldsOpenedOrderNumber: number;
    onOpenListItemFields: (event: React.MouseEvent, orderNumber: number) => void;
}

export const COMPONENT_NAME = "ItemsListMenuComponent";
export const LIST_ITEMS_MENU_CONTAINER_ID = "listItemsContainer";

export const ItemsListMenuComponent: React.FunctionComponent<ItemsListMenuComponentProps> = (props) => {
    const { t } = useTranslation();

    const {
        isItemHighlighted,
        componentRef,
        itemsListState,
        invoiceItemsState,
        onAddInvoiceListItem
    } = useListItemsMenu(props);

    const computedStyles: React.CSSProperties = {
        top: itemsListState.top,
        left: itemsListState.left
    };

    return props.isOpen ? (
        <div
            id={LIST_ITEMS_MENU_CONTAINER_ID}
            ref={componentRef}
            className={`${FIELD_ASSIGNMENT_COMPONENT_NAME} ${FIELD_ASSIGNMENT_COMPONENT_NAME}__item-fields-menu`}
            style={computedStyles}>
            <div className={`${FIELD_ASSIGNMENT_COMPONENT_NAME}__fields-header`}>{t("ITEMS_LIST_TITLE")}</div>
            <div className={`${FIELD_ASSIGNMENT_COMPONENT_NAME}__fields-group`}>
                {invoiceItemsState.map((item, index) => {
                    return (
                        <ListItem
                            key={`${COMPONENT_NAME}__${item.orderNumber}`}
                            orderNumber={item.orderNumber}
                            onOpenListItem={props.onOpenListItemFields}
                            isItemFieldsMenuOpened={isItemHighlighted(
                                props.itemFieldsOpenedOrderNumber,
                                item.orderNumber
                            )}
                        />
                    );
                })}
                <div
                    key={`${COMPONENT_NAME}__itemsListAddButton`}
                    className={`${FIELD_ASSIGNMENT_COMPONENT_NAME}__field-item-button`}>
                    <div className={`${FIELD_ASSIGNMENT_COMPONENT_NAME}__add-new-item`} onClick={onAddInvoiceListItem}>
                        <img
                            alt={t("ADD_NEW_ITEM_TITLE")}
                            src={`${addNewItemImage}`}
                            className={`${COMPONENT_NAME}__company-logo`}
                        />
                        {t("ADD_NEW_ITEM_CAPS_TITLE")}
                    </div>
                </div>
            </div>
        </div>
    ) : null;
};
