# Project Structure

-   [General](#general)
-   [Folder & File Structure](#folder--file-structure)
-   [Application Structure](#application-structure)
-   [Feature/Page Component Tree](#featurepage-component-tree)
-   [Code Validation](#code-validation)
    -   [Compiling](#compiling)
    -   [Linting](#linting)
    -   [Unit Tests](#unit-tests)

## General

Project use [Create React App](https://create-react-app.dev/docs/getting-started) scripts infrastructure. All available information from the official developer guide could be used in addition to the following guide.

## Folder & File Structure

-   **.vscode/** - VSCode workspace files. Only files that contain common project settings should be checked into git. You may need to add specific files or subfolders to the .gitignore file when adding plugins, etc., to prevent your user-specific customizations from being checked in.
-   **coverage/** - Unit test code coverage results. See [test-ci](../README.md#npm-run-testci) script.
-   **build/** - Distribution files are output here when building the project. Omitted from git.
-   **docs/** - Additional documentation about this project.
-   **.env.\* files** - Lists of environment variables which will be substituted in source code upon run or deploy to a given environment.
-   **node_modules/** - Third party dependencies managed by npm. Omitted from git.
-   **public/** - Static files that will be transferred as-is to `build/public` when building the project so that they can be served to the web app.
-   **src/** - Our project source code.
-   **src/mocks/** - File/module mocks used by Jest for unit testing.
-   **src/styles/** - Folder contains global application styles, theme variables and 3rd party libraries configuration.
-   **src/settings/** - Environment settings configuration.
-   **src/localization/** - i18n library configuration.
-   **src/store/** - Redux global store configuration (via Redux Toolkit).
-   **src/setupTests.ts** - Test environment initialization.
-   **src/react-app-env.d.ts** - Create-React-App TypeScript types registration and extension/overriding of global types (i.e., Window object).
-   **src/index.ts** - React application entry point.
-   **src/helperTypes.ts** - Helpful common generic types which reduce amount of import/export lines in source code.
-   **.browserslistrc** - Single configuration for all tools (i.e. POSTCSS) that need to know what browsers are supported.
-   **.gitignore** - Hides files from git so that unwanted files are not checked in.
-   **.prettierignore** - Hide files from Prettier so that they are not processed.
-   **.editorconfig** - IDE generic style guide configuration file.
-   **.prettierrc** - Prettier configuration file.
-   **.sass-lint.yml** - Sass-lint configuration file.
-   **package.json** - Defines what NPM modules our project depends on, and defines some useful scripts.
-   **package-lock.json** - NPM package lock file that ensures the same version of every node module is consistent.
-   **tsconfig.json** - Configuration file for TypeScript compiler.
-   **tslint.json** - Configuration file for TSLint.

## Application Structure

Invoice Capture Application development follows component-based approach. Codebase structure divided in:

-   `api` folder contains HTTP clients that implement `PWP.InvoiceCapture.InvoiceManagement.API` and `PWP.InvoiceCapture.Identity.API` consumers (See also [API Integration](./8-api-integration.md)).
    -   All request and response models have in-sync API interfaces and are stored in `models` sub folder.
    -   HTTP clients are structured by API resource name with `ApiEndpoint` suffix, inherits [BaseApiEndpoint](../src/api/BaseApiEndpoint.ts) class and should be used only by `Redux-Thunk` actions (See [FetchInvoiceDataAsync.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/actions/FetchInvoiceDataAsync.ts)). `ApiEndpoint` should reflect all API resource actions in one place that are used by the application.
-   `components` folder contains all React UI components and decomposed in the following way:
    -   `common` folder has miscellaneous or shared components that can be reused across all application components. [Components Development](4-components.md) conventions can/should be applied here too.
    -   `RootComponent` folder contains main Invoice Capture Application component which integrate and load feature/page components by specified routing rules. Integration of Authorization logic also can be done here.
        -   `index.ts` is common file in TypeScript world. It should be used for re-exporting local folder file to the parent folder. Using of it decrease complexity of `import`'s.
    -   `<Feature|Page>Component` folders contain feature or page React component with custom React logic hooks (think as Controllers), feature Redux store and all required sub components.
        -   `<Feature|Page>Component`.tsx, `<Feature|Page>Component`.scss, `<Feature|Page>Component`.test.tsx files define core functionality of the component.
        -   `<Element>`.tsx file can store partial element view for feature or page component.
        -   `hooks` folder has custom application logic hooks that used directly by feature or page component. All other hooks should be placed in `hooks` folder of a sub component directory.
        -   `store` folder define feature Redux store configuration and contains complex `actions`, state `mappers`, state `reducers` for all defined actions, reusable state query `selectors` and interfaces of feature store `state`. Any feature store related logic (i.e. validation schemas) can be placed here. `Slice` definition is the integration point of feature store (in terms of `Redux Toolkit`). It should be connected with global Redux store configuration.

## Feature/Page Component Tree

-   `InvoiceDataCaptureToolkitComponent` is main Invoice Capture Application functionality. It's loading Invoice data, handling Invoice state management and containing the following elements:
    -   `Header` element with:
        -   `ToolkitHeaderComponent` that displaying PWP logo, Invoice processing status and `Download Invoice Document` button.
    -   `Toolkit` element with:
        -   `DocumentViewComponent` that displaying left Document View area with:
            -   `NoContentPlaceholder` element that will be shown when Invoice data won't have document pages.
            -   `DocumentPageViewComponent` that displaying an invoice document page, fit page size with Document View container width with:
                -   `DocumentPageImagePaneComponent` that displaying document page image and will handle image operations.
                -   `MultiSelectionPaneComponent` that displaying all Invoice captured data annotations (i.e, Document Layout Items), highlight selected layout items and provide press-move-release left mouse button functionality that can select all layout items in specified area and with:
                    -   `DocumentLayoutItemComponent` that displaying Document Layout Item position, state (i.e., selected, assigned, enabled, disabled, in focus, out of focus) and can show `FieldAssignmentMenuComponent` context menu when selected.
                -   `FieldAssignmentMenuComponent` that displaying RMB context menu view and providing ability to assign selected Document Layout Items with specific Invoice field.
        -   `InvoiceFieldsComponent` that displaying right Invoice Fields form with:
            -   `FieldsClearConfirmationComponent` modal element.
            -   `Clear All` button element that displaying confirmation modal and reset form state.
            -   `InvoiceFieldComponent` that handling an Invoice Field edit input, show validation errors and displaying highlighting state.
    -   `ControlPanel` element with:
        -   `DocumentControlPanelComponent` that displaying footer bar with:
            -   `DocumentPageNavigationComponent` that implements Invoice document page navigation functionality with:
                -   `NextButton` element.
                -   `PageNavigationView` input element.
                -   `PreviousButton` element.
            -   `ToggleCompareBoxesButton` button element is displaying the current data annotation visibility state and able to toggle it.
        -   `ToolkitControlPanelComponent` button group provide ability to save intermediate state, submit Invoice data for approval or close the application.
    -   `ErrorNotification` modal element that should display any unexpected errors during work with Invoice Data Capture Toolkit.

## Code Validation

The following items will need to be completed/passing before committing any code and making a pull request. You can
perform all the following steps individually or you can run the [precommit](../README.md#npm-run-precommit)
script to run all three at once.

### Compiling

To quickly compile the project, run the [compile](../README.md#npm-run-compile) script.
Fix any compiler errors at this time.

### Linting

To quickly run lint, run the [lint](../README.md#npm-run-lint) script. Some issues can be automatically fixed by running [lint-fix](../README.md#npm-run-lint-fix).

### Unit Tests

Run [test](../README.md#npm-run-test) and ensure all unit tests are passing.
