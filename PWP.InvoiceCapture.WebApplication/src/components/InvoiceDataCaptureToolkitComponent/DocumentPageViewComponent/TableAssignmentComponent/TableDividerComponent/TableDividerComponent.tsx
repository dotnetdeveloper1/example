import classNames from "classnames";
import React, { SetStateAction, useCallback, useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import ReactTooltip from "react-tooltip";
import { IBox, IPoint } from "../../../store/state";
import { DividerOrientation, ITableDivider } from "../../../store/state/ITableDivider";
import { TablePositionService } from "../tablePositionService";
import "./TableDividerComponent.scss";

interface TableDividerProps {
    divider: ITableDivider;
    table: IBox;
    dividerDragMode: boolean;
    editMode: boolean;
    tooltipWasShown: boolean;
    tablePositionService: TablePositionService;
    onDividerContextMenu: (event: React.MouseEvent<Element, MouseEvent> | MouseEvent) => void;
    onDividerDeleted: (id: number) => void;
    onDividerPositionChanged: (id: number, position: IPoint) => void;
    onDividerDragModeChanged: (dragging: boolean) => void;
    onNotifyTooltipWasShown: () => void;
}

interface ITableDividerComponentState {
    hover: boolean;
    dragging: boolean;
}

const tooltipLeftShift = -75;
const tooltipTopShift = -80;
const tooltipDelay = 1000;

export const COMPONENT_NAME = "TableDividerComponent";

export const TableDividerComponent: React.FunctionComponent<TableDividerProps> = React.memo((props) => {
    const dividerRef = useRef<HTMLDivElement>(null);
    const { t } = useTranslation();

    const [state, setState] = useState<ITableDividerComponentState>({
        hover: false,
        dragging: false
    });

    const dividerStyles = [COMPONENT_NAME];

    const dashedDividerStyles = [`${COMPONENT_NAME}__dashed`];

    const innerDividerStyles = [`${COMPONENT_NAME}__inner`];

    const hoverDividerStyles = [`${COMPONENT_NAME}__hover`];

    if (props.divider.orientation === DividerOrientation.Horizontal) {
        dividerStyles.push(`${COMPONENT_NAME}--horizontal`);
        innerDividerStyles.push(`${COMPONENT_NAME}__inner--horizontal`);
        dashedDividerStyles.push(`${COMPONENT_NAME}__dashed--horizontal`);
        hoverDividerStyles.push(`${COMPONENT_NAME}__hover--horizontal`);
    } else {
        dividerStyles.push(`${COMPONENT_NAME}--vertical`);
        innerDividerStyles.push(`${COMPONENT_NAME}__inner--vertical`);
        dashedDividerStyles.push(`${COMPONENT_NAME}__dashed--vertical`);
        hoverDividerStyles.push(`${COMPONENT_NAME}__hover--vertical`);
    }

    if (!props.divider.isValid) {
        hoverDividerStyles.push(`${COMPONENT_NAME}__hover--invalid`);
    }

    if (!props.divider.isValid && !props.dividerDragMode) {
        innerDividerStyles.push(`${COMPONENT_NAME}__inner--invalid`);
        dashedDividerStyles.push(`${COMPONENT_NAME}__dashed--invalid`);
    }

    const onMouseOver = useCallback(
        (event: React.MouseEvent<Element, MouseEvent>) => {
            if (event.buttons === 0) {
                setState((prevState) => ({
                    ...prevState,
                    hover: true
                }));
            }
        },
        [setState]
    );

    const onMouseOut = useCallback(
        (event: React.MouseEvent<Element, MouseEvent>) => {
            if (!state.dragging) {
                setState((prevState) => ({
                    ...prevState,
                    hover: false
                }));
            }
        },
        [state.dragging]
    );

    const onMouseDown = useCallback(
        (event: React.MouseEvent<HTMLDivElement, MouseEvent>) => {
            if (event.button !== 0 || !props.editMode || event.nativeEvent.detail > 1) {
                return;
            }

            setState((prevState) => ({
                ...prevState,
                dragging: true,
                hover: true
            }));
            props.onDividerDragModeChanged(true);
            event.stopPropagation();
            event.preventDefault();
        },
        [props]
    );

    const handleGlobalMouseUp: EventListener = useCallback(
        (event): void => {
            onMouseUp(
                state,
                setState,
                props.editMode,
                props.tablePositionService,
                props.divider,
                props.onDividerDragModeChanged
            )(event as MouseEvent);
        },
        [props.divider, props.editMode, props.onDividerDragModeChanged, props.tablePositionService, state]
    );

    const handleGlobalMouseMove: EventListener = useCallback(
        (event): void => {
            onMouseMove(
                state,
                props.editMode,
                props.divider,
                props.tablePositionService,
                props.onDividerPositionChanged
            )(event as MouseEvent);
        },
        [props.divider, props.editMode, props.onDividerPositionChanged, props.tablePositionService, state]
    );

    const onContextMenu = useCallback(
        (event: React.MouseEvent<Element, MouseEvent> | MouseEvent): void => {
            props.onDividerContextMenu(event);
            event.preventDefault();
        },
        [props]
    );

    useEffect(() => {
        window.addEventListener("mousemove", handleGlobalMouseMove);
        window.addEventListener("mouseup", handleGlobalMouseUp);
        return () => {
            window.removeEventListener("mousemove", handleGlobalMouseMove);
            window.removeEventListener("mouseup", handleGlobalMouseUp);
        };
    }, [handleGlobalMouseMove, handleGlobalMouseUp]);

    const tooltipId: string = `dividerTooltip_${props.divider.id}`;

    return (
        <div>
            <ReactTooltip
                disable={state.dragging || !props.editMode || props.tooltipWasShown}
                className={`${COMPONENT_NAME}__custom-tooltip-theme`}
                id={tooltipId}
                backgroundColor={"transparent"}
                afterHide={() => {
                    props.onNotifyTooltipWasShown();
                }}
                delayShow={tooltipDelay}
                offset={{ top: tooltipTopShift, left: tooltipLeftShift }}>
                <div className={`${COMPONENT_NAME}--inner-tooltip`}>
                    <div className={`${COMPONENT_NAME}--tooltip-div`}>
                        {t("TOOLTIP->DIVIDER_CLICK")} <br />
                    </div>
                    <div className={`${COMPONENT_NAME}--tooltip-div`}>{t("TOOLTIP->DIVIDER_DOUBLE_CLICK")}</div>
                </div>
            </ReactTooltip>
            <div
                data-tip={true}
                data-for={tooltipId}
                ref={dividerRef}
                className={classNames(dividerStyles)}
                onMouseDown={onMouseDown}
                onDoubleClick={(e) => props.onDividerDeleted(props.divider.id)}
                onMouseOver={onMouseOver}
                onMouseOut={onMouseOut}
                onContextMenu={onContextMenu}
                style={{
                    top: props.tablePositionService.getDividerRelativeCoordinates(props.divider).top,
                    left: props.tablePositionService.getDividerRelativeCoordinates(props.divider).left,
                    zIndex: state.hover ? 500 : 300
                }}>
                <div
                    className={classNames(dashedDividerStyles)}
                    style={{ visibility: props.dividerDragMode || !props.editMode ? "hidden" : "visible" }}
                />
                <div
                    className={classNames(hoverDividerStyles)}
                    style={{ visibility: state.hover && props.editMode ? "visible" : "hidden" }}
                />

                <div
                    className={classNames(innerDividerStyles)}
                    style={
                        props.divider.orientation === DividerOrientation.Horizontal
                            ? {
                                  margin: `0 ${props.tablePositionService.getScaledControlAreaOffset()}px 0 ${props.tablePositionService.getScaledControlAreaOffset()}px`,
                                  width: `calc(100% - ${2 *
                                      props.tablePositionService.getScaledControlAreaOffset()}px)`,
                                  backgroundColor: props.dividerDragMode || !props.editMode ? "#cccccc" : "#0c3769"
                              }
                            : {
                                  margin: `${props.tablePositionService.getScaledControlAreaOffset()}px 0 0 0`,
                                  height: `calc(100% - ${2 *
                                      props.tablePositionService.getScaledControlAreaOffset()}px)`,
                                  backgroundColor: props.dividerDragMode || !props.editMode ? "#cccccc" : "#0c3769"
                              }
                    }
                />
            </div>
        </div>
    );
});

function onMouseUp(
    state: ITableDividerComponentState,
    setState: React.Dispatch<SetStateAction<ITableDividerComponentState>>,
    editMode: boolean,
    tablePositionService: TablePositionService,
    divider: ITableDivider,
    onDividerDragModeChanged: (dragMode: boolean) => void
): (event: MouseEvent | React.MouseEvent<HTMLDivElement, MouseEvent>) => void {
    return (event: MouseEvent | React.MouseEvent<HTMLDivElement, MouseEvent>) => {
        if (state.dragging && event.button === 0 && editMode) {
            const { pageX, pageY } = event;
            const absoluteDividerPosition = tablePositionService.getDividerAbsolutePosition(divider);
            const hover =
                absoluteDividerPosition.x - 1 <= pageX &&
                absoluteDividerPosition.x - 1 + divider.width! >= pageX &&
                absoluteDividerPosition.y - 1 <= pageY &&
                absoluteDividerPosition.y - 1 + divider.height! >= pageY;
            setState((prevState) => ({
                ...prevState,
                dragging: false,
                hover: hover
            }));

            onDividerDragModeChanged(false);

            event.stopPropagation();
            event.preventDefault();
        }
    };
}

function onMouseMove(
    state: ITableDividerComponentState,
    editMode: boolean,
    divider: ITableDivider,
    tablePositionService: TablePositionService,
    onDividerPositionChanged: (dividerId: number, nextPosition: IPoint) => void
): (event: MouseEvent | React.MouseEvent<HTMLDivElement, MouseEvent>) => void {
    return (event: MouseEvent | React.MouseEvent<HTMLDivElement, MouseEvent>) => {
        if (state.dragging && event.button === 0 && editMode) {
            const { pageX, pageY } = event;
            const nextPosition = tablePositionService.getDividerNextPosition(divider, {
                x: pageX,
                y: pageY
            });
            if (
                Math.trunc(nextPosition.x) !== Math.trunc(divider.left) ||
                Math.trunc(nextPosition.y) !== Math.trunc(divider.top)
            ) {
                onDividerPositionChanged(divider.id, nextPosition);
            }
            event.stopPropagation();
            event.preventDefault();
        }
    };
}
