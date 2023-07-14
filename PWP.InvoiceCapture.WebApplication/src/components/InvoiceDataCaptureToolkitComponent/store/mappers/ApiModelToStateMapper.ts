import { InvoiceStatus } from "../state";
import { Invoice } from "./../../../../api/models/InvoiceManagement/Invoice";
import { IInvoice } from "./../state/IInvoice";
export class ApiModelToStateMapper {
    public mapInvoicesList = (apiInvoicesList: Invoice[]): IInvoice[] | undefined => {
        if (apiInvoicesList && apiInvoicesList.length) {
            apiInvoicesList.sort((a, b) => (new Date(b.createdDate) as any) - (new Date(a.createdDate) as any));
            return apiInvoicesList.map(this.mapInvoice);
        }
        return undefined;
    };

    public mapInvoice = (apiInvoice: Invoice): IInvoice => {
        return {
            id: apiInvoice.id,
            name: apiInvoice.name,
            status: InvoiceStatus[apiInvoice.status],
            createdDate: new Date(apiInvoice.createdDate).toLocaleString("en-US"),
            modifiedDate: new Date(apiInvoice.modifiedDate).toLocaleString("en-US")
        };
    };
}
