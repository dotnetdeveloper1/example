import { faAngleRight } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { useTranslation } from "react-i18next";
import { invoiceFieldsSelector, useToolkitSelector } from "../store/selectors";
import { IInvoiceFields } from "../store/state";
import { COMPONENT_NAME } from "./FieldAssignmentMenuComponent";
import { useFieldAssignmentMenu } from "./hooks";

interface FieldSelectionListProps extends Pick<ReturnType<typeof useFieldAssignmentMenu>, "onAssignField"> {}
interface FieldSelectionListProps extends Pick<ReturnType<typeof useFieldAssignmentMenu>, "onOpenListItems"> {}
interface FieldSelectionListProps extends Pick<ReturnType<typeof useFieldAssignmentMenu>, "listItemsMenuState"> {}
interface FieldSelectionListProps extends Pick<ReturnType<typeof useFieldAssignmentMenu>, "onEnterMainMenuField"> {}

export const ITEMS_LIST_PARENT_FIELD_ID = "itemsListParentField";

export const FieldSelectionList: React.FunctionComponent<FieldSelectionListProps> = (props) => {
    const { t } = useTranslation();
    const fields: IInvoiceFields | undefined = useToolkitSelector(invoiceFieldsSelector);

    return (
        <div className={`${COMPONENT_NAME}__fields-group`}>
            {fields &&
                fields.invoiceFields &&
                fields.invoiceFields.map((field) => (
                    <div
                        key={`assign-menu-item-${field.fieldId}`}
                        className={`${COMPONENT_NAME}__field`}
                        onMouseEnter={props.onEnterMainMenuField}
                        onClick={props.onAssignField(field.fieldId)}>
                        {field.fieldName}
                    </div>
                ))}

            <div
                id={ITEMS_LIST_PARENT_FIELD_ID}
                key="itemsList"
                className={`${COMPONENT_NAME}__field-items` + (props.listItemsMenuState.isOpen ? "-active" : "")}
                onMouseEnter={props.onOpenListItems}>
                {t("ITEMS_LIST_TITLE")}
                <FontAwesomeIcon icon={faAngleRight} size="lg" />
            </div>
        </div>
    );
};
