import { faCloudUploadAlt } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import classNames from "classnames";
import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { postInvoiceFileAsync } from "../../InvoiceDataCaptureToolkitComponent/store/InvoiceDataCaptureToolkitStoreSlice";
import "./UploadInvoiceComponent.scss";

export const COMPONENT_NAME = "UploadInvoiceComponent";

interface UploadInvoiceComponentProps {
    accessToken: string;
}

export const UploadInvoiceComponent: React.FunctionComponent<UploadInvoiceComponentProps> = (props) => {
    const { t } = useTranslation();

    const defaultMessage = t("DRAG_INVOICE_MESSAGE->DRAG_PROMPT");
    const containerStyles = [`${COMPONENT_NAME}__container`];

    const [fileData, setFileData] = useState<File | undefined>();
    const [currentMessage, setMessage] = useState(defaultMessage);
    const dispatch = useDispatch();

    useEffect(() => {
        if (fileData) {
            dispatch(postInvoiceFileAsync({ file: fileData, accessToken: props.accessToken }));
            setFileData(undefined);
        }
    }, [dispatch, fileData, props.accessToken]);

    const onDrop = (e: any) => {
        e.preventDefault();
        const {
            dataTransfer: { files }
        } = e;
        const { length } = files;
        if (length === 0) {
            return false;
        }
        const fileTypes = [
            "application/pdf",
            "image/png",
            "image/bmp",
            "image/tif",
            "image/tiff",
            "image/jpeg",
            "image/jpg"
        ];

        const { size, type } = files[0];

        setFileData(undefined);

        if (!fileTypes.includes(type)) {
            setMessage(t("DRAG_INVOICE_MESSAGE->INVALID_FILE_FORMAT"));
            return false;
        }
        if (size / 1024 / 1024 > 100) {
            setMessage(t("DRAG_INVOICE_MESSAGE->INVALID_FILE_SIZE"));
            return false;
        }
        setMessage(defaultMessage);

        setFileData(files[0]);
    };

    const onDragOver = (e: any) => {
        e.preventDefault();
    };

    return (
        <div className={`${COMPONENT_NAME}`} onDrop={(e) => onDrop(e)} onDragOver={(e) => onDragOver(e)}>
            <div className={classNames(containerStyles)}>
                <FontAwesomeIcon className={`${COMPONENT_NAME}__icon`} icon={faCloudUploadAlt} size="4x" />
                {currentMessage && <div className={`${COMPONENT_NAME}__message`}>{currentMessage}</div>}
            </div>
        </div>
    );
};
