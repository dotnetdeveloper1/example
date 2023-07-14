import { AccessToken } from "./../models/accessToken";

export class JwtDecodeService {
    private static urlBase64Decode(base64url: string): string {
        let output = base64url.replace(/-/g, "+").replace(/_/g, "/");
        switch (output.length % 4) {
            case 0:
                break;
            case 2:
                output += "==";
                break;
            case 3:
                output += "=";
                break;
            default:
                throw new Error("Illegal base64url string");
        }
        return decodeURIComponent(escape(atob(output)));
    }

    public static decodeToken(token: string): AccessToken {
        const parts = token.split(".");

        if (parts.length !== 3) {
            throw new Error("JWT must have 3 parts");
        }

        const decoded = this.urlBase64Decode(parts[1]);

        if (!decoded) {
            throw new Error("Cannot decode token");
        }

        return JSON.parse(decoded) as AccessToken;
    }

    public static getCurrentCultureFromAccessToken(): string {
        const accessToken = window.accessToken;
        if (!accessToken) {
            return "en-US";
        }
        const decodedToken = this.decodeToken(accessToken);
        return decodedToken.Culture ? decodedToken.Culture : "en-US";
    }

    public static getCurrentTenantId(): string {
        const accessToken = window.accessToken;
        if (!accessToken) {
            return "Default";
        }
        const decodedToken = this.decodeToken(accessToken);
        return decodedToken.TenantId ? decodedToken.TenantId : "Default";
    }
}
