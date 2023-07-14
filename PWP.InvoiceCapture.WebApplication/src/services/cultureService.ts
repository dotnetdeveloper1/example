import { rootStore } from "./../store/configuration";
import { JwtDecodeService } from "./jwtDecodeService";
export class CultureService {
    public static getCurrentCultureName(): string {
        const storeCultureName = rootStore.getState().invoiceDataCaptureToolkit.cultureName;
        return storeCultureName ? storeCultureName : JwtDecodeService.getCurrentCultureFromAccessToken();
    }
}
