import { useCallback, useEffect, useState } from "react";
import { PageNavigationView } from "../PageNavigationView";
import { ReactPropsType, ReactPropType } from "./../../../../helperTypes";

interface IPageNavigationViewHookResult {
    inputValue: string;
    onInputBlur(event: React.FocusEvent<HTMLInputElement>): void;
    onInputChange(event: React.ChangeEvent<HTMLInputElement>): void;
}

export function usePageNavigationView(props: ReactPropsType<typeof PageNavigationView>): IPageNavigationViewHookResult {
    const pageNumber = props.pageNumber.toString();

    const [inputValue, setInputValue] = useState<string>(pageNumber);

    const onInputBlur = useCallback(
        (event) => {
            const inputNumber = parseInt(event.target.value, 10);
            if (!tryChangePageNumber(inputNumber, props.totalPages, props.onPageChange)) {
                setInputValue(pageNumber);
            }
        },
        [props, setInputValue, pageNumber]
    );

    const onInputChange = useCallback(
        (event: React.ChangeEvent<HTMLInputElement>) => {
            setInputValue(event.target.value);

            const inputNumber = parseInt(event.target.value, 10);
            tryChangePageNumber(inputNumber, props.totalPages, props.onPageChange);
        },
        [props, setInputValue]
    );

    useEffect(() => {
        setInputValue(pageNumber);
    }, [props, pageNumber]);

    return {
        inputValue: inputValue,
        onInputBlur: onInputBlur,
        onInputChange: onInputChange
    };
}

function tryChangePageNumber(
    inputNumber: number,
    totalPages: ReactPropType<typeof PageNavigationView, "totalPages">,
    onPageChange: ReactPropType<typeof PageNavigationView, "onPageChange">
): boolean {
    if (inputNumber && inputNumber > 0 && inputNumber < totalPages + 1) {
        onPageChange(inputNumber);
        return true;
    }

    return false;
}
