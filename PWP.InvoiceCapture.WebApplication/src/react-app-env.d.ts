/// <reference types="react-scripts" />

declare namespace NodeJS {
    interface ProcessEnv {
        readonly NODE_ENV: "development" | "production" | "test";
        readonly PUBLIC_URL: string;
        readonly REACT_APP_ENVIRONMENT: string;
        readonly REACT_APP_INVOICEMANAGEMENTAPIENDPOINT: string;
        readonly REACT_APP_DOCUMENTAGGREGATIONAPIENDPOINT: string;
        readonly REACT_APP_IDENTITYAPIENDPOINT: string;
        readonly REACT_APP_FIELDAPIENDPOINT: string;
        readonly REACT_APP_FIELDGROUPAPIENDPOINT: string;
        readonly REACT_APP_INVOICEFIELDAPIENDPOINT: string;
        readonly REACT_APP_OCRAPIENDPOINT: string;
        readonly REACT_APP_VERSION: string;
        readonly REACT_APP_API_VERSION: string;
        readonly REACT_APP_LANGUAGE: string;
        readonly REACT_APP_USERNAME: string;
        readonly REACT_APP_PASSWORD: string;
        readonly REACT_APP_CLIENT_ID: string;
    }
}

declare interface OVERRIDDEN_ENV_SETTINGS {
    invoiceManagementApiEndpoint: string;
    documentAggregationApiEndpoint: string;
    identityApiEndpoint: string;
    fieldApiEndpoint: string;
    fieldGroupApiEndpoint: string;
    invoiceFieldApiEndpoint: string;
    ocrApiEndpoint: string;
    environment: string;
    version: string;
    apiVersion: string;
    overridden: string;
    username: string;
    password: string;
}
