import React from "react";
import { useTranslation } from "react-i18next";
import { TranslationUtils } from "../../../../utils/translationUtils";
import { LineItemsFieldTypes, LineItemsFieldTypesMap } from "../../store/state";
import { COMPONENT_NAME as FIELD_ASSIGNMENT_COMPONENT_NAME } from "../FieldAssignmentMenuComponent";
import { useItemFieldsMenu } from "./hooks";

interface ItemFieldsComponentProps {
    isOpen: boolean;
    orderNumber: number;
    onAssignField(fieldType: LineItemsFieldTypes, orderNumber: number): () => void;
}

export const COMPONENT_NAME = "ItemFieldsMenuComponent";
export const ITEM_FIELDS_CONTAINER_ID = "itemFieldsContainer";

export const ItemFieldsMenuComponent: React.FunctionComponent<ItemFieldsComponentProps> = (props) => {
    const { t } = useTranslation();

    const { componentRef, itemFieldsState } = useItemFieldsMenu(props);

    const computedStyles: React.CSSProperties = {
        top: itemFieldsState.top,
        left: itemFieldsState.left
    };

    return props.isOpen ? (
        <div
            id={ITEM_FIELDS_CONTAINER_ID}
            ref={componentRef}
            className={`${FIELD_ASSIGNMENT_COMPONENT_NAME} ${FIELD_ASSIGNMENT_COMPONENT_NAME}__item-fields-menu`}
            style={computedStyles}>
            <div className={`${FIELD_ASSIGNMENT_COMPONENT_NAME}__fields-header`}>{`${t("ITEM_TITLE")} ${
                props.orderNumber
            }`}</div>
            <div className={`${FIELD_ASSIGNMENT_COMPONENT_NAME}__fields-group`}>
                {Array.from(LineItemsFieldTypesMap.values()).map((fieldType) => (
                    <div
                        key={`${COMPONENT_NAME}_${fieldType.toString()}`}
                        className={`${FIELD_ASSIGNMENT_COMPONENT_NAME}__field`}
                        onClick={props.onAssignField(fieldType, props.orderNumber)}>
                        {t(TranslationUtils.getItemsFieldTypeLocalizationKeyByString(fieldType.toString()))}
                    </div>
                ))}
            </div>
        </div>
    ) : null;
};
