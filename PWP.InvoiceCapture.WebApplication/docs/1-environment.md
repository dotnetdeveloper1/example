# Project Environment Settings

-   [Description](#description)
    -   [List Of Application Configuration Points](#list-of-application-configuration-points)
-   [Create-React-App Settings](#create-react-app-settings)
    -   [Used CRA Parameters](#used-cra-parameters)
-   [Build-time Configuration And `SettingsProvider` Structure](#build-time-configuration-and-settingsprovider-structure)
    -   [Used `SettingsProvider` Fields](#used-settingsprovider-fields)
-   [Deployment-time Configuration](#deployment-time-configuration)
    -   [Used Deployment-time Settings](#used-deployment-time-settings)
-   [Localization Configuration](#localization-configuration)
-   [Runtime Configuration (Configuration API Endpoint)](#runtime-configuration-configuration-api-endpoint)

## Description

Project configuration is using `.env` files under Create-React-App infrastructure rules for specifying per environment settings (See [Environment Configuration](https://create-react-app.dev/docs/adding-custom-environment-variables/) manual).

### List Of Application Configuration Points

-   Development tasks are using [.env.development](../.env.development) config (i.e., [npm run start](../README.md#npm-run-start), [npm run build:dev](../README.md#npm-run-builddev)).
-   Production tasks are using [.env.production](../.env.production) config (i.e., [npm run build](../README.md#npm-run-build)).
-   Test tasks are using [.env.test](../.env.test) config (i.e., [npm run test](../README.md#npm-run-test), [npm run test:ci](../README.md#npm-run-testcli)).
-   Common settings that should be applied on all environment types are placed in [.env](../.env) config.
-   [Web Manifest](../public/manifest.json) is using for PWA mode.
-   [public/index.html](../public/index.html) is using for as-is initial HTML document structure configuration (for example, external CDN font references, polyfills, static `meta` tags, etc.) and has `<script>` tag section for deployment-time settings overriding.
-   [.browserlistrc](../.browserslistrc) defines browser support configuration for all application dependencies.
-   [public/locales/{language}/translation.json](../public/locales/) JSON files provide key-value pairs of per-language localized text.

## Create-React-App Settings

Create-React-App infrastructure has the list of configurable runtime/build options that can be overridden when it's needed. Check [Advanced Configuration](https://create-react-app.dev/docs/advanced-configuration) manual for more information.

### Used CRA Parameters

| Name                 | Description                                                                                                                             |
| -------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `GENERATE_SOURCEMAP` | By default, CRA includes source code maps in production build, but it should be disabled to decrease bundle size and increase security. |
| `PORT`               | Just explicit port specification.                                                                                                       |

## Build-time Configuration And `SettingsProvider` Structure

Application uses settings defined in `SettingsProvider` class and reference `settings` singleton instance.
`SettingsProvider` hides multiple configuration points. Currently it reads parameters from:

-   `process.env` object which injected by webpack configuration and provides values from environment variables and `.env` files.
-   browser `window` object that provides deployment-time configuration values from `index.html`.

All parameters from `process.env` object are specified on build and should be satisfied by Create-React-App environment variable name rules (i.e. parameter key name should start from `REACT_APP_` prefix).

### Used `SettingsProvider` Fields

| Field name                     | Type    | `.env` file key                        | Source                               | Value                                                             | Description                                                                                 |
| ------------------------------ | ------- | -------------------------------------- | ------------------------------------ | ----------------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `environment`                  | string  | REACT_APP_ENVIRONMENT                  | `.env` file                          | `development`, `production` or `test`                             | Provides the current environment name identifier.                                           |
| `invoiceManagementApiEndpoint` | string  | REACT_APP_INVOICEMANAGEMENTAPIENDPOINT | `.env` file or `index.html` override | valid absolute url for `PWP.InvoiceCapture.InvoiceManagement.API` | Provides InvoiceManagement API URL for Http clients.                                        |
| `identityApiEndpoint`          | string  | REACT_APP_IDENTITYAPIENDPOINT          | `.env` file or `index.html` override | valid absolute url for `PWP.InvoiceCapture.Identity.API`          | Provides Identity API URL for Http clients.                                                 |
| `language`                     | string  | REACT_APP_LANGUAGE                     | `.env` file                          | short or full language identifier (i.e, `en` or `en_US`)          | Provides default application language for localization library.                             |
| `version`                      | string  | REACT_APP_VERSION                      | `.env` file or `index.html` override | Application version descriptor (i.e, 1.x.x or 1.x.x-Release-y)    | Provides the current application version.                                                   |
| `applicationBasePath`          | string  | PUBLIC_URL                             | `.env` file                          | `/` or any valid relative url path                                | Always should be `/`, except the situation when application will be deployed in sub folder. |
| `isDevelopment`                | boolean | -                                      | Computed from `environment`          | `true` or `false`                                                 | Shortcut computed field which simplify development check in application code.               |

## Deployment-time Configuration

Besides [.env.production](../.env.production) settings `Invoice Capture Application` Release Pipeline can override API endpoint urls which will target specific deployment stage and can set deployment version.

### Used Deployment-time Settings

| `SettingsProvider` and `index.html` field name | field name                   | Value                                                             | Description                                                           |
| ---------------------------------------------- | ---------------------------- | ----------------------------------------------------------------- | --------------------------------------------------------------------- |
| `invoiceManagementApiEndpoint`                 | `__INVOICE_MANAGEMENT_API__` | valid absolute url for `PWP.InvoiceCapture.InvoiceManagement.API` | Provides Deployment Stage InvoiceManagement API URL for Http clients. |
| `identityApiEndpoint`                          | `__IDENTITY_API__`           | valid absolute url for `PWP.InvoiceCapture.Identity.API`          | Provides Deployment Stage Identity API URL for Http clients.          |
| `version`                                      | `__VERSION__`                | Application version descriptor (i.e, 1.x.x or 1.x.x-Release-y)    | Provides application version of the current deployment.               |
| `overridden`                                   | `__OVERRIDDEN__`             | `__OVERRIDDEN__` or `True`                                        | Detect that deployment-time parameters are specified.                 |

## Localization Configuration

`i18next` and `react-i18next` libraries are used for text localization feature in the application.

Library configuration defined in [i18n.ts](../src/localization/i18n.ts) file. All localized text should be wrapped in `t(...)` function provided by `useTranslation` hook.
All localization keys should be stored in [public/locales/{language}/translation.json](../public/locales/) files with appropriate translated value.

## Runtime Configuration (Configuration API Endpoint)

**TBD**

TODO Application should have Per Tenant configuration point.
Update documentation when the feature will be implemented.
