export interface TokenRequest {
    grant_type: string;
    refresh_token?: string;
    client_id: string;
    client_secret: string;
    scope?: string;
    username?: string;
    password?: string;
}
