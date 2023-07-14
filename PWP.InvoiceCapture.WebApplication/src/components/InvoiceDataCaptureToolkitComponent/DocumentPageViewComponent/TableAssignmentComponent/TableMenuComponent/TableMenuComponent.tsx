import { faCheck } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { useTranslation } from "react-i18next";
import { TranslationUtils } from "../../../../../utils/translationUtils";
import { RootMountComponent } from "../../../../common/RootMountComponent";
import { LineItemsFieldTypes, LineItemsFieldTypesMap } from "../../../store/state";
import { SwitchToEditModeComponent } from "../SwitchToEditModeComponent/SwitchToEditModeComponent";
import { ITableMenuState } from "../TableAssignmentComponent";
import { useTableMenuComponent } from "./hooks/useTableMenuComponent";
import "./TableMenuComponent.scss";

interface TableMenuComponentProps {
    tableMenuState: ITableMenuState;
    onAssignColumn(fieldType: LineItemsFieldTypes, left: number): () => void;
    onTableMenuClosed: () => void;
}

export const COMPONENT_NAME = "TableMenuComponent";

export const TableMenuComponent: React.FunctionComponent<TableMenuComponentProps> = (props) => {
    const { t } = useTranslation();

    const { isEdge, isAssignedFieldType, tableMenuStyles, componentRef } = useTableMenuComponent(
        props.onTableMenuClosed,
        props.tableMenuState
    );

    const menuHeaderClass = isEdge ? `${COMPONENT_NAME}__edge-header-label` : `${COMPONENT_NAME}__header-label`;

    return props.tableMenuState.isOpen ? (
        <RootMountComponent>
            <div ref={componentRef} className={`${COMPONENT_NAME}`} style={tableMenuStyles}>
                <div className={`${COMPONENT_NAME}__header`}>
                    <div className={menuHeaderClass}>{t("CONTEXT_MENU_HEADER->COLUMN_TO_FIELD")}</div>
                </div>
                <div className={`${COMPONENT_NAME}__fields-group`}>
                    {Array.from(LineItemsFieldTypesMap.values()).map((fieldType) => (
                        <div
                            key={`${COMPONENT_NAME}_${fieldType.toString()}`}
                            className={`${COMPONENT_NAME}__field`}
                            onClick={props.onAssignColumn(fieldType, props.tableMenuState.left)}>
                            {t(TranslationUtils.getItemsFieldTypeLocalizationKeyByString(fieldType.toString()))}
                            {isAssignedFieldType(fieldType) ? <FontAwesomeIcon icon={faCheck} /> : null}
                        </div>
                    ))}
                    <SwitchToEditModeComponent
                        beforeSwitchToEditMode={props.onTableMenuClosed}
                        askConfirmation={false}
                        isMenuStyles={true}
                    />
                </div>
            </div>
        </RootMountComponent>
    ) : null;
};
