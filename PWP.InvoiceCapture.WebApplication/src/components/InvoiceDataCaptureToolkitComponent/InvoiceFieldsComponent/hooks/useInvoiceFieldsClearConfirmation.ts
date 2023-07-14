import { useCallback, useMemo, useState } from "react";
import { ReactPropsType } from "./../../../../helperTypes";
import { InvoiceStatus } from "./../../store/state";
import { InvoiceFieldsComponent } from "./../InvoiceFieldsComponent";

interface IFieldsClearConfirmationHookResult {
    isModalOpen: boolean;
    isClearAllEnabled: boolean;
    onOpen(): void;
    onCancel(): void;
    onConfirm(): void;
}

export function useFieldsClearConfirmation(
    props: ReactPropsType<typeof InvoiceFieldsComponent>
): IFieldsClearConfirmationHookResult {
    const [isModalOpen, setState] = useState<boolean>(false);
    const isClearAllEnabled = useMemo(() => props.invoiceStatus === InvoiceStatus.PendingReview, [props]);

    const onOpen = useCallback(() => {
        if (isClearAllEnabled) {
            setState(true);
        }
    }, [setState, isClearAllEnabled]);

    const onConfirm = useCallback(() => {
        setState(false);
        props.onFieldsClearAll();
    }, [props, setState]);

    const onCancel = useCallback(() => setState(false), [setState]);

    return {
        isModalOpen: isModalOpen,
        isClearAllEnabled: isClearAllEnabled,
        onOpen: onOpen,
        onCancel: onCancel,
        onConfirm: onConfirm
    };
}
