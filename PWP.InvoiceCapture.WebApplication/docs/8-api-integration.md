# Working with API endpoints

-   [General](#general)
-   [`BaseApiEndpoint` Configuration](#baseapiendpoint-configuration)
-   [Invoice Management API Resources](#invoice-management-api-resources)
-   [Identity API Resources](#identity-api-resources)
-   [API Client usage restrictions](#api-client-usage-restrictions)

## General

[Axios Documentation](https://github.com/axios/axios)

In our project `axios` http promise-based client are used for REST API integration. Compare to `fetch` browser API it provides better configuration options (error handling, request-response transformation, interceptors, request cancellation, etc.) and fluent library API.

API integration divides on:

-   `BaseApiEndpoint` class that providing common settings for `axios` instance object and introduce overriding points for inheritor classes.
-   `<Resource>ApiEndpoint` classes (inherited from `BaseApiEndpoint` class) implement specific resource API clients.
-   `models` folder where Resource API request and response models are defined.

## `BaseApiEndpoint` Configuration

| Configuration Point        | Description                                                                                                                                                       |
| -------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `constructor`              | `AxiosConfig` could be specified to change default behavior and configuration of `AxiosInstance` endpoint. For example, endpoint `baseUrl`, `timeout` delay, etc. |
| `interceptRequestMessage`  | Should be used for changing request object with common endpoint headers, parameters or request settings                                                           |
| `interceptRequestError`    | Should be used for common endpoint error catching occurring on request invocation                                                                                 |
| `interceptResponseMessage` | Should be used for common endpoint validation, transformation or filter logic over response object                                                                |
| `interceptResponseError`   | Should be used for common endpoint error catching occurring on response handling                                                                                  |

## Invoice Management API Resources

| Resource Name                                                                                                       | Actions                                        | Description                                                                                                                             |
| ------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Invoice (TBD)                                                                                                       | `documentLink`                                 | Invoice resource endpoint integration is used in application to get initial Invoice Document file                                       |
| InvoicePage ([InvoicePageApiEndpoint.ts](../src/api/InvoicePageApiEndpoint.ts))                                     | `pages`, `pageImageLink`                       | InvoicePage resource endpoint integration is used in application to get Invoice page data and image links                               |
| InvoiceProcessingResult ([InvoiceProcessingResultApiEndpoint.ts](../src/api/InvoiceProcessingResultApiEndpoint.ts)) | `getProcessingResults`, `putProcessingResults` | InvoiceProcessingResult resource endpoint integration is used in application to get Invoice captured data and save Invoice data changes |

## Identity API Resources

| Resource Name                                                          | Actions | Description                                                                                                |
| ---------------------------------------------------------------------- | ------- | ---------------------------------------------------------------------------------------------------------- |
| Identity ([IdentityApiEndpoint.ts](../src/api/IdentityApiEndpoint.ts)) | `token` | Identity resource endpoint integration is used in application to validate and refresh Authorization token. |

## API Client usage restrictions

-   Resource API clients should handle only request-response logic. Business logic should be moved into State mappers or Redux reducers.
-   Resource API clients should not catch any unexpected errors, should validate response status code and response data object structure.
-   Resource API clients should be used only in scope of Thunk Actions or State Mappers. Any usage in other places will produce tricky issues and should be avoided.
