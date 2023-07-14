import { useCallback, useState } from "react";

export interface IFieldAssignmentMenuState {
    isOpen: boolean;
    top: number;
    left: number;
}

export interface IItemsListMenuComponentState {
    isOpen: boolean;
    top: number;
    left: number;
}

export interface IItemFieldsMenuComponentState {
    isOpen: boolean;
    top: number;
    left: number;
    orderNumber: number;
}

interface IFieldAssignmentMenuHookResult {
    contextMenuState: IFieldAssignmentMenuState;
    onContextMenuOpen: (event: MouseEvent) => void;
    onContextMenuClose: () => void;
}

interface IFieldAssignmentMenuHookConsumerParameters {}

export function useFieldAssignmentMenuControl(
    parameters: IFieldAssignmentMenuHookConsumerParameters
): IFieldAssignmentMenuHookResult {
    const [state, setState] = useState<IFieldAssignmentMenuState>({
        isOpen: false,
        top: 0,
        left: 0
    });

    const onContextMenuOpen = useCallback(
        (event: MouseEvent) => {
            setState({
                isOpen: true,
                left: event.pageX ?? 0,
                top: event.pageY ?? 0
            });
        },
        [setState]
    );

    const onContextMenuClose = useCallback(() => {
        setState({
            isOpen: false,
            left: 0,
            top: 0
        });
    }, [setState]);

    return {
        contextMenuState: state,
        onContextMenuOpen: onContextMenuOpen,
        onContextMenuClose: onContextMenuClose
    };
}
