import { faTrashAlt } from "@fortawesome/free-solid-svg-icons";
import React, { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { ReactPropsType } from "../../helperTypes";
import { AccordionComponent } from "../common/AccordionComponent/AccordionComponent";
import { DocumentViewComponent } from "./DocumentViewComponent";
import { COMPONENT_NAME } from "./InvoiceDataCaptureToolkitComponent";
import { InvoiceFieldsComponent } from "./InvoiceFieldsComponent";
import { FieldsClearConfirmationComponent } from "./InvoiceFieldsComponent/FieldsClearConfirmationComponent";
import { useFieldsClearConfirmation } from "./InvoiceFieldsComponent/hooks";
import { templateIdSelector, trainingsCountSelector, useToolkitSelector, vendorNameSelector } from "./store/selectors";
import { InvoiceStatus } from "./store/state";

interface ToolkitProps
    extends ReactPropsType<typeof DocumentViewComponent>,
        ReactPropsType<typeof InvoiceFieldsComponent> {}

export const Toolkit: React.FunctionComponent<ToolkitProps> = React.memo((props) => {
    const { t } = useTranslation();

    const { isModalOpen, onOpen, onConfirm, onCancel, isClearAllEnabled } = useFieldsClearConfirmation(props);
    const templateId = useToolkitSelector(templateIdSelector);
    const trainingsCount = useToolkitSelector(trainingsCountSelector);
    const vendorName = useToolkitSelector(vendorNameSelector);
    const isFormFieldsEnabled = useMemo(() => props.invoiceStatus === InvoiceStatus.PendingReview, [props]);

    return (
        <div className={`${COMPONENT_NAME}__body`}>
            <div className={`${COMPONENT_NAME}__document-view-container`}>
                <DocumentViewComponent
                    pages={props.pages}
                    showCompareBoxes={props.showCompareBoxes}
                    selectedPlainText={props.selectedPlainText}
                    onPageChange={props.onPageChange}
                    onLayoutItemsSelect={props.onLayoutItemsSelect}
                    onAssignItems={props.onAssignItems}
                    onAssignLineItems={props.onAssignLineItems}
                    onAssignColumnToLineItems={props.onAssignColumnToLineItems}
                    invoiceStatus={props.invoiceStatus}
                    tableSelectionMode={props.tableSelectionMode}
                />
            </div>
            <div className={`${COMPONENT_NAME}__invoice-fields-container`}>
                <AccordionComponent headerText={t("INVOICE_FIELDS_TITLE->TRAINING_TITLE")}>
                    <div className={`${COMPONENT_NAME}__info-table`}>
                        <div className={`${COMPONENT_NAME}__info-table__data-row`}>
                            <label className={`${COMPONENT_NAME}__info-table__data-row__label`}>
                                {t("INVOICE_FIELDS_TITLE->TEMPLATE_ID")}
                            </label>
                            <div className={`${COMPONENT_NAME}__info-table__data-row__value`}>
                                <b>{templateId?.toString()}</b>
                            </div>
                        </div>
                        <div className={`${COMPONENT_NAME}__info-table__data-row`}>
                            <label className={`${COMPONENT_NAME}__info-table__data-row__label`}>
                                {t("INVOICE_FIELDS_TITLE->VENDOR_NAME")}
                            </label>
                            <div className={`${COMPONENT_NAME}__info-table__data-row__value`}>
                                <b>{vendorName}</b>
                            </div>
                        </div>
                        <div className={`${COMPONENT_NAME}__info-table__data-row`}>
                            <label className={`${COMPONENT_NAME}__info-table__data-row__label`}>
                                {t("INVOICE_FIELDS_TITLE->TRAININGS_COUNT")}
                            </label>
                            <div className={`${COMPONENT_NAME}__info-table__data-row__value`}>
                                <b>{trainingsCount?.toString()}</b>
                            </div>
                        </div>
                    </div>
                </AccordionComponent>
                <AccordionComponent
                    headerText={t("INVOICE_FIELDS_TITLE->DATA_EXTRACTION_GROUP_HEADER")}
                    expanded={true}
                    buttonIcon={faTrashAlt}
                    buttonText={t("BUTTON_TITLE->CLEAR")}
                    onButtonClick={onOpen}
                    buttonDisabled={!isClearAllEnabled || !isFormFieldsEnabled}>
                    <InvoiceFieldsComponent
                        fieldsState={props.fieldsState}
                        fieldTypesInFocus={props.fieldTypesInFocus}
                        lineItemsFieldTypesInFocus={props.lineItemsFieldTypesInFocus}
                        onFieldFocus={props.onFieldFocus}
                        onLineItemFieldFocus={props.onLineItemFieldFocus}
                        onFieldBlur={props.onFieldBlur}
                        onFieldsClearAll={props.onFieldsClearAll}
                        onFieldsSubmit={props.onFieldsSubmit}
                        onFormValidationStateChanged={props.onFormValidationStateChanged}
                        invoiceStatus={props.invoiceStatus}
                        tableSelectionMode={props.tableSelectionMode}
                    />
                </AccordionComponent>
            </div>
            <FieldsClearConfirmationComponent isOpen={isModalOpen} onCancel={onCancel} onConfirm={onConfirm} />
        </div>
    );
});
