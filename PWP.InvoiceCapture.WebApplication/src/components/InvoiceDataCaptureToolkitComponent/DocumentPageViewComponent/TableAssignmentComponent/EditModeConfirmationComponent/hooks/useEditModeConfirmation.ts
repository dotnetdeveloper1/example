import { useCallback, useState } from "react";

interface IEditModeConfirmationHookResult {
    isEditModeModalOpen: boolean;
    onOpen(): void;
    onCancel(): void;
    onConfirm(): void;
}

export function useEditModeConfirmation(onConfirmAction: () => void): IEditModeConfirmationHookResult {
    const [isModalOpen, setState] = useState<boolean>(false);

    const onOpen = useCallback(() => {
        setState(true);
    }, [setState]);

    const onConfirm = useCallback(() => {
        setState(false);
        onConfirmAction();
    }, [onConfirmAction]);

    const onCancel = useCallback(() => setState(false), [setState]);

    return {
        isEditModeModalOpen: isModalOpen,
        onOpen: onOpen,
        onCancel: onCancel,
        onConfirm: onConfirm
    };
}
