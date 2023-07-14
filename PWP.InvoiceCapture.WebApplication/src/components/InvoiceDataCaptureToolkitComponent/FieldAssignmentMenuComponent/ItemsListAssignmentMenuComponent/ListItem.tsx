import { faAngleRight } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { useTranslation } from "react-i18next";
import { COMPONENT_NAME } from "../FieldAssignmentMenuComponent";

interface ListItemProps {
    orderNumber: number;
    onOpenListItem: (event: React.MouseEvent, orderNumber: number) => void;
    isItemFieldsMenuOpened: boolean;
}

export const LIST_ITEM_SUFFIX = "_ListItem";

export const ListItem: React.FunctionComponent<ListItemProps> = (props) => {
    const { t } = useTranslation();

    return (
        <div
            id={`${props.orderNumber}${LIST_ITEM_SUFFIX}`}
            key={`${props.orderNumber}${LIST_ITEM_SUFFIX}`}
            className={`${COMPONENT_NAME}__field-items` + (props.isItemFieldsMenuOpened ? "-active" : "")}
            onMouseEnter={(event: React.MouseEvent) => props.onOpenListItem(event, props.orderNumber)}>
            {`${t("ITEM_TITLE")} ${props.orderNumber}`}
            <FontAwesomeIcon icon={faAngleRight} size="lg" />
        </div>
    );
};
