const OVERRIDDEN_FLAG = "__OVERRIDDEN__";

export interface ISettings {
    environment: string;
    ocrApiEndpoint: string;
    invoiceManagementApiEndpoint: string;
    fieldGroupApiEndpoint: string;
    invoiceFieldApiEndpoint: string;
    fieldApiEndpoint: string;
    documentAggregationApiEndpoint: string;
    identityApiEndpoint: string;
    apiVersion: string;
    language: string;
    version: string;
    applicationBasePath: string;
    isDevelopment: boolean;
    username: string;
    password: string;
    clientId: string;
}

class SettingsProvider implements ISettings {
    constructor() {
        this.environment = process.env.REACT_APP_ENVIRONMENT;
        this.version = process.env.REACT_APP_VERSION;
        this.apiVersion = process.env.REACT_APP_API_VERSION;
        this.language = process.env.REACT_APP_LANGUAGE;
        this.ocrApiEndpoint = process.env.REACT_APP_OCRAPIENDPOINT;
        this.invoiceManagementApiEndpoint = process.env.REACT_APP_INVOICEMANAGEMENTAPIENDPOINT;
        this.fieldGroupApiEndpoint = process.env.REACT_APP_INVOICEMANAGEMENTAPIENDPOINT;
        this.fieldApiEndpoint = process.env.REACT_APP_FIELDAPIENDPOINT;
        this.invoiceFieldApiEndpoint = process.env.REACT_APP_INVOICEFIELDAPIENDPOINT;
        this.documentAggregationApiEndpoint = process.env.REACT_APP_DOCUMENTAGGREGATIONAPIENDPOINT;
        this.identityApiEndpoint = process.env.REACT_APP_IDENTITYAPIENDPOINT;
        this.applicationBasePath = process.env.PUBLIC_URL || "/";
        this.username = process.env.REACT_APP_USERNAME;
        this.password = process.env.REACT_APP_PASSWORD;
        this.clientId = process.env.REACT_APP_CLIENT_ID;

        const releaseTimeEnvironmentSettings = (window as any) as OVERRIDDEN_ENV_SETTINGS;
        if (
            releaseTimeEnvironmentSettings &&
            releaseTimeEnvironmentSettings.overridden &&
            releaseTimeEnvironmentSettings.overridden !== OVERRIDDEN_FLAG
        ) {
            this.invoiceManagementApiEndpoint = releaseTimeEnvironmentSettings.invoiceManagementApiEndpoint;
            this.ocrApiEndpoint = releaseTimeEnvironmentSettings.ocrApiEndpoint;
            this.identityApiEndpoint = releaseTimeEnvironmentSettings.identityApiEndpoint;
            this.fieldApiEndpoint = releaseTimeEnvironmentSettings.fieldApiEndpoint;
            this.fieldGroupApiEndpoint = releaseTimeEnvironmentSettings.invoiceManagementApiEndpoint;
            this.invoiceFieldApiEndpoint = releaseTimeEnvironmentSettings.invoiceFieldApiEndpoint;
            this.version = releaseTimeEnvironmentSettings.version;
            this.documentAggregationApiEndpoint = releaseTimeEnvironmentSettings.documentAggregationApiEndpoint;
            this.environment = releaseTimeEnvironmentSettings.environment;
            this.username = releaseTimeEnvironmentSettings.username;
            this.password = releaseTimeEnvironmentSettings.password;
        }
    }

    public environment: string;
    public invoiceManagementApiEndpoint: string;
    public documentAggregationApiEndpoint: string;
    public ocrApiEndpoint: string;
    public identityApiEndpoint: string;
    public fieldGroupApiEndpoint: string;
    public fieldApiEndpoint: string;
    public invoiceFieldApiEndpoint: string;
    public apiVersion: string;
    public language: string;
    public version: string;
    public applicationBasePath: string;
    public username: string;
    public password: string;
    public clientId: string;

    public get isDevelopment(): boolean {
        return this.environment === "development";
    }
}

export const settings: ISettings = new SettingsProvider();
