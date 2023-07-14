import { AxiosResponse } from "axios";
import { axiosMock } from "../mocks/axios";
import { identityApi } from "./IdentityApiEndpoint";
import { ApiResponse } from "./models/ApiResponse";
import { TokenResponse } from "./models/Identity/TokenResponse";

describe("IdentityApiEndpoint", () => {
    const successResponse: AxiosResponse<TokenResponse> = {
        data: {
            access_token: "test_access_token",
            expires_in: 3600,
            refresh_token: "test_refresh_token",
            token_type: "Bearer",
            scope: ""
        },
        status: 200,
        statusText: "OK",
        config: {},
        headers: {}
    };
    const errorResponse: ApiResponse = {
        code: 500,
        message: "Internal Error"
    };
    test("token - 200 (OK) token received", async () => {
        axiosMock.post.mockReturnValueOnce(Promise.resolve(successResponse));

        const tokenRequestData = {
            client_id: "test_client_id",
            client_secret: "test_secret_id",
            grant_type: "simple"
        };

        const urlEncodedData = Object.entries(tokenRequestData)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
            .join("&");

        const token = await identityApi.token(urlEncodedData);

        expect(token).toBe(successResponse.data);
    });

    test("token - 500 (Error) exception with error message", async (done) => {
        axiosMock.post.mockReturnValueOnce(Promise.reject(errorResponse));

        const tokenRequestData = {
            clientId: "test_client_id",
            clientSecret: "test_secret_id",
            grantType: "simple"
        };

        const urlEncodedData = Object.entries(tokenRequestData)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
            .join("&");

        identityApi.token(urlEncodedData).catch((error) => {
            expect(error).toBe(errorResponse);
            done();
        });
    });
});
