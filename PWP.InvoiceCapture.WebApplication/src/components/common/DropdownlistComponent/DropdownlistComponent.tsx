import React from "react";
import "./DropdownlistComponent.scss";

interface DropdownlistProps {
    onToggled(): void;
    onValueSelected(item: DropdownlistItem): void;
    state: DropdownlistState;
    noSelectionText: string;
    widthPx?: number;
}

export interface DropdownlistState {
    isToggled: boolean;
    values: DropdownlistItem[];
    enabled: boolean;
    selectedValue: DropdownlistItem | undefined;
}

export interface DropdownlistItem {
    id: number;
    value: string;
    name: string;
}

export const COMPONENT_NAME = "DropdownlistComponent";

export const Dropdownlist: React.FunctionComponent<DropdownlistProps> = (props) => {
    const buttonClass = props.state.enabled ? `${COMPONENT_NAME}__button` : `${COMPONENT_NAME}__button--disabled`;
    const menuClass = props.state.isToggled ? `${COMPONENT_NAME}__menu-toggled` : `${COMPONENT_NAME}__menu-hidden`;

    const buttonStyles: React.CSSProperties = props.widthPx ? { minWidth: props.widthPx, maxWidth: props.widthPx } : {};

    return (
        <div className={COMPONENT_NAME}>
            <div
                className={`${COMPONENT_NAME}__dropdown`}
                onClick={() => {
                    if (props.state.enabled) {
                        props.onToggled();
                    }
                }}>
                <button style={buttonStyles} className={buttonClass}>
                    {props.state.selectedValue?.value ?? props.noSelectionText}
                </button>
                <div className={menuClass}>
                    {props.state.values.map((item) => {
                        return (
                            <div
                                key={`${COMPONENT_NAME}__item_${item.id}`}
                                className={`${COMPONENT_NAME}__item`}
                                onClick={() => {
                                    if (props.state.enabled) {
                                        props.onValueSelected(item);
                                    }
                                }}>
                                {item.value}
                            </div>
                        );
                    })}
                </div>
            </div>
        </div>
    );
};
