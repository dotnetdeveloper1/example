import axios from "axios";
import { useCallback, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { invoiceApi } from "../../../../api/InvoiceApiEndpoint";
import { DropdownlistItem, DropdownlistState } from "../../../common/DropdownlistComponent/DropdownlistComponent";
import { ReactPropsType } from "./../../../../helperTypes";
import { CultureService } from "./../../../../services/cultureService";
import { getCulture } from "./../../store/InvoiceDataCaptureToolkitStoreSlice";
import { InvoiceStatus } from "./../../store/state";
import { ToolkitHeaderComponent } from "./../ToolkitHeaderComponent";

interface IUseToolkitHeaderHookResult {
    onDownloadButtonPress: (event: React.MouseEvent) => void;
    downloadingFile: boolean;
    toggleLanguageDropdownListOpen: () => void;
    languageDropdownListValueChanged: (item: DropdownlistItem) => void;
    languageListState: DropdownlistState;
}

const australiaItem: DropdownlistItem = { id: 99, value: "English (Australia)", name: "en-AU" };
const usaItem: DropdownlistItem = { id: 188, value: "English (United States)", name: "en-US" };

export function useToolkitHeader(props: ReactPropsType<typeof ToolkitHeaderComponent>): IUseToolkitHeaderHookResult {
    const [downloadingFile, setDownloadingFile] = useState(false);

    const isFormFieldsEnabled = useMemo(() => props.invoiceStatus === InvoiceStatus.PendingReview, [props]);

    const [languageListState, setLanguageListState] = useState<DropdownlistState>({
        enabled: isFormFieldsEnabled,
        isToggled: false,
        values: [australiaItem, usaItem],
        selectedValue: getCultureSelectedValue()
    });

    const toggleLanguageDropdownListOpen = useCallback(() => {
        setLanguageListState((prevState) => ({
            ...prevState,
            isToggled: !languageListState.isToggled
        }));
    }, [languageListState, setLanguageListState]);

    const dispatch = useDispatch();
    const languageChanged = useCallback(
        (item: DropdownlistItem) => {
            setLanguageListState((prevState) => ({
                ...prevState,
                isToggled: false,
                selectedValue: item
            }));
            dispatch(getCulture(item.name));
        },
        [dispatch]
    );

    const onDownloadButtonPress = useCallback(
        async (event: React.MouseEvent) => {
            if (props.invoiceId && !downloadingFile) {
                setDownloadingFile(true);
                const fileLink = await invoiceApi.getDocumentLink(props.invoiceId);
                axios({
                    url: fileLink,
                    method: "GET",
                    responseType: "blob"
                }).then((response) => {
                    const url = window.URL.createObjectURL(new Blob([response.data]));
                    const link = document.createElement("a");
                    link.href = url;
                    link.setAttribute("download", `${props.invoiceFileName}`);
                    document.body.appendChild(link);
                    link.click();
                    setDownloadingFile(false);
                });
            }
        },
        [downloadingFile, props.invoiceFileName, props.invoiceId]
    );

    return {
        onDownloadButtonPress: onDownloadButtonPress,
        downloadingFile: downloadingFile,
        toggleLanguageDropdownListOpen: toggleLanguageDropdownListOpen,
        languageDropdownListValueChanged: languageChanged,
        languageListState: languageListState
    };
}

function getCultureSelectedValue(): DropdownlistItem {
    const cultureName = CultureService.getCurrentCultureName();
    return cultureName === australiaItem.name ? australiaItem : usaItem;
}
