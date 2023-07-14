# PWP.InvoiceCapture.WebApplication

-   [Description](#description)
-   [Getting Started](#getting-started)
-   [Development Workflow](#development-workflow)
-   [Development Guides](#development-guides)
-   [Available NPM Scripts (Commands)](#available-npm-scripts-commands)
    -   [Running scripts from VSCode](#running-scripts-from-vscode)
    -   [Running scripts from a terminal](#running-scripts-from-a-terminal)
    -   [Passing command-line parameters to scripts](#passing-command-line-parameters-to-scripts)
    -   [npm run start](#npm-run-start)
    -   [npm run build](#npm-run-build)
    -   [npm run build:dev](#npm-run-builddev)
    -   [npm run test](#npm-run-test)
    -   [npm run test:ci](#npm-run-testci)
    -   [npm run eject](#npm-run-eject)
    -   [npm run compile](#npm-run-compile)
    -   [npm run fix](#npm-run-fix)
    -   [npm run prettier](#npm-run-prettier)
    -   [npm run prettier-fix](#npm-run-prettier-fix)
    -   [npm run lint](#npm-run-lint)
    -   [npm run lint-fix](#npm-run-lint-fix)
    -   [npm run lint-sass](#npm-run-lint-sass)
    -   [npm run precommit](#npm-run-precommit)
    -   [npm install](#npm-install)
    -   [npm update](#npm-update)
-   [Learn More About Project Technologies](#learn-more-about-project-technologies)
    -   [Essentials](#essentials)
    -   [Libraries](#libraries)
    -   [Testing](#testing)
    -   [Tooling](#tooling)

## Description

Invoice Capture Web Application is React-based SPA project that represents Invoice Data Capturing Toolkit functionality and works with `PWP.InvoiceCapture.InvoiceManagement.API` to preview, edit, validate and save Invoice output data. Project are using and be accessible only by temporary link authorization process in production. Link(Access Token) validation and refreshing logic depends on `PWP.InvoiceCapture.Identity.API`.

## Getting Started

1.  Install [Visual Studio Code](https://code.visualstudio.com/Download), if not already installed.
2.  Follow [IDE Configuration](./docs/2-ide-configuration.md) guide and setup recommended extensions.
3.  [Install Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git) and [SourceTree](https://www.sourcetreeapp.com/) (Optional).
4.  [Install Node](https://nodejs.org/en/) -> Latest LTS version.
5.  Clone this repository.
6.  Run `npm install` from the project directory (terminal should have sudo/Admin access).
7.  Run `npm run start` from the project directory to start a local dev server.
8.  The application will launch in the system default browser at [localhost:8081](http://localhost:8081/).
9.  Make edits, and the application will automatically update via HMR (Hot Module Reloading) on your local dev server.
10. If changes cannot be committed (`husky` precommit action fails), running of `npm run fix` can help with style guide issues resolving.
11. When merging and/or checking out changes from another branch you will need to ensure that you run the [npm install](#npm-install) script to install any dependencies you might not have in your branch. If there are still issues present after running `npm install`, you can try delete `node_modules` directory and re-run `npm install` to perform a complete fresh reinstall of all dependencies.

## Development Workflow

1. Assign yourself to a `User Story`/`Task` and move Work Item status to `Active` (Use Azure Boards).
    - All `User Stories`/`Tasks` available to be worked on will have a status of `New` and assigned to Active Sprint.
2. Clarify all necessary requirements with the team to ensure that you can start working on the task.
3. Create branch off the `develop` branch. (Use branch name pattern - `feature/<Work Item ID>-<short-feature-title>`)
    - You should always start by creating a branch off of what ever branch is considered the `develop` branch you should be working from. You can create a branch directly through Azure Boards while viewing the assigned task. This will create a remote branch that you can then check out locally. The Work Item ID should be included in the name of the branch when the branch is created from Azure Boards, which allows for Azure Repos/Azure Pipelines to work properly when pushing changes. You can also create a branch locally to start and then push the branch to the remote repository. However, you will need to remember to include the ID of the Azure Boards Work Item in the name you give the branch, otherwise, our Azure Repos/Azure Pipelines will not work properly.
4. Write code and tests.
    - Write all necessary code to complete the task. It is expected that all new and modified code will covered by unit tests as much as possible. You will need to either add new or edit existing unit tests to provide appropriate code coverage. You can run the [npm run start](#npm-run-start) command to start the webpack server for testing changes made to the main application. The following is a list of some NPM commands that can be run during development to assist in cleaning up your code:
        - [compile](#npm-run-compile): Runs the compiler and will report any compile issues that need to be addressed.
        - [lint](#npm-run-lint): Runs TSLint and will point out any TypeScript linting issues that need to be addressed.
        - [lint-sass](#npm-run-lint-sass): Runs SASS linter and will report any SASS styles linting issues that need to be addressed.
        - [test:ci](#npm-run-testci): Will run all unit tests and present a code coverage report and any errors that occurred while running the unit tests.
        - [prettier-fix](#npm-run-prettier-fix): Runs prettier and formats all code added to our set standard.
        - [lint-fix](#npm-run-lint-fix): Will attempt to fix linting issues reported by the "lint" NPM command. Some issues will still need to be manually fixed, as this will not be able to auto fix all linting issues.
        - [fix](#npm-run-fix): Will run all lint analyzers with `fix` flag and try to correct all automatically fixable issues.
    - We also have a script called [precommit](#npm-run-precommit) that will run `compile`, `lint`, `lint-sass`, `prettier` and `test` one after the other from a single command. You can either run each individual command separately or run the `precommit` command to see if everything is passing or what needs to be fixed.
    - When code is pushed to the remote branch, the `precommit` command will be ran. If it results in no errors, then the Azure Pipelines build for that branch will succeed, otherwise, it will fail and you will need to fix any errors to get the build passing correctly.
5. Open repository in Azure Repos and create Pull Request for team peer review.
    - Once your assigned Work Item is complete, you will need to submit a Pull Request from your Work Item's feature branch to the `develop` branch of the project. Team members will then be able to review your code changes and provide any feedback they might have. If any rework is suggested, you will need to provide support for fixing/making any changes needed. Once Pull Request is approved and all Build Pipeline checks are passed, the changes will be merged into the `develop` branch.
6. Wrap up Work Item.
    - After the code is merged and changes are available on `Development` environment through running of `Invoice Capture Application` Release Pipeline, Work Item can then be marked as `Resolved`/`Closed` (or `Fixed` if it was a bug fix).

## Development Guides

[Project Environment Settings](./docs/1-environment.md)

[IDE Configuration](./docs/2-ide-configuration.md)

[Project Structure](./docs/3-project-structure.md)

[Components Development](./docs/4-components.md)

[Components Styling](./docs/5-styling.md)

[Components Testing](./docs/6-testing.md)

[Extending Store Logic & State](./docs/7-redux-store.md)

[Working with API endpoints](./docs/8-api-integration.md)

## Available NPM Scripts (Commands)

### Running scripts from VSCode

These scripts will be listed in the `NPM SCRIPTS` tool sidebar (under/over Folder Tree view). Double-click a script to open it. Right-click a script to access a `Run` option to run it.

### Running scripts from a terminal

NPM scripts can be run from the terminal if the working directory is the project root:

```
npm run script-name
```

### Passing command-line parameters to scripts

It is possible to pass command-line params to the underlying command of the NPM script by providing them after a `--` separator:

```
npm run script-name -- --flag-for-underlying-command --param-with-value=value
```

Just be aware that everything after the `--` is simply appended to the end of the NPM script's implementation.

NPM scripts are defined in `package.json`. In the project directory, you can run:

### npm run start

Runs the app in the development mode.<br />
Open [http://localhost:8081](http://localhost:8081) to view it in the browser.

The page will reload if you make edits.<br />
You will also see any lint errors in the console.

The application runs using [.env.development](.env.development) settings.

### npm run build

Bundles the application in production mode and save bundle in `build` folder.

It correctly bundles React in production mode and optimizes the build for the best performance. The build is minified and the filenames include the hashes.

The application builds using [.env.production](.env.development) settings.

### npm run build:dev

Command runs the same way as [`npm run build`](#npm-run-build), but will generate source maps and use development project environment settings.

The application builds using [.env.development](.env.development) settings.

### npm run test

Launches JEST test runner in the interactive watch mode.

See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### npm run test:ci

Runs all unit tests once and collects code coverage data.
A summary of coverage statistics is displayed, and detailed coverage results are output to the `coverage/` folder.

Load `coverage/lcov-report/index.html` in a browser to browser detailed coverage by folder/file/line.

**NOTE:** This command is used by Azure DevOps CI to run unit tests, generate `junit` test run report and `cobertura` test coverage report.

### npm run eject

**Note: this is a one-way operation. Once you `eject`, you can’t go back!**

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app). In most cases, CRA template structure is enough to build production-ready static content output and access all necessary developer tools.

If you aren’t satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (`webpack`, `Babel`, `ESLint`, etc.) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you’re on your own.

You don’t have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn’t feel obligated to use this feature. However we understand that this tool wouldn’t be useful if you couldn’t customize it when you are ready for it.

When custom `webpack` configuration will be required, the project should be ejected from CRA scripts and bundling build process should be reconfigured. As a consensus between handwritten `webpack` configuration and CRA template, [React App Rewired](https://github.com/timarney/react-app-rewired) can be used in the future.

### npm run compile

Runs the TypeScript compiler against the entire project for the purpose of testing for compiler errors. This does not generate any output. It only displays results of compilation.

Use this to easily confirm whether any of the code has any compiler errors/warnings.

Compiler configuration is in `tsconfig.json`.

See also, [npm run precommit](#npm-run-precommit)

### npm run fix

Command shortcut that runs [npm run lint-fix](#npm-run-lint-fix) and [npm run prettier-fix](#npm-run-prettier-fix) simultaneously.

### npm run prettier

Runs "prettier" code formatting validation. Fails if any files do not match the strict formatting style.

### npm run prettier-fix

Runs "prettier" code formatting and re-writes files properly formatted as necessary.

### npm run lint

Runs TSLint against the entire project and reports any linting errors.

Linting configuration is in `tslint.json`.

See also, [npm run lint-fix](#npm-run-lint-fix), [npm run precommit](#npm-run-precommit)

### npm run lint-fix

Runs TSLint against the entire project with the "fix" option.
All linting errors that can be automatically fixed will be fixed (.e.g., missing semicolon). All remaining linting errors are reported.

Linting configuration is in `tslint.json`.

See also, [npm run lint](#npm-run-lint)

### npm run lint-sass

Runs SASSLint against the entire project and reports any style linting errors.

Linting configuration is in `.sass-lint.yml`.

See also, [npm run precommit](#npm-run-precommit)

### npm run precommit

It is good practice to run this script before committing changes to ensure that your changes do not obviously break the application (`husky` has configured to run it automatically on commit creation).

This script runs the compiler, linter and tests. The script fails if any one of them fails.

### npm install

Installs all `package.json` dependencies.

Run this:

-   After cloning the repository for the first time.
-   After checking out a new branch, pulling changes, or merging changes from another branch that may introduce a new dependency.

### npm update

Upgrades all npm packages to the latest available version that matches the version descriptor in `package.json`. This will both install the updates and rebuild `package-lock.json` accordingly.

## Learn More About Project Technologies

### Essentials

-   TypeScript language - [Documentation](https://www.typescriptlang.org/docs/home.html)
-   React library (using Hooks) - [Documentation](https://reactjs.org/)
-   React Redux Toolkit - [Documentation](https://redux-toolkit.js.org/introduction/quick-start)
-   Bootstrap CSS Framework (with `Reactstrap` components) - [Bootstrap documentation](https://getbootstrap.com/docs/4.5/getting-started/introduction/), [Reactstrap documentation](https://reactstrap.github.io/)
-   SCSS styling language - [Documentation](https://sass-lang.com/documentation)
-   BEM styling guideline - [Information](https://en.bem.info/methodology/key-concepts/)

### Libraries

-   Axios HTTP client. Used for async requests (not used directly, but wrapped by our BaseApiEndpoint) - [Documentation](https://github.com/axios/axios)
-   Immer.js (Redux Store immutable state management) - [Documentation](https://immerjs.github.io/immer/docs/introduction)
-   FontAwesome icons for React - [Documentation](https://www.npmjs.com/package/@fortawesome/react-fontawesome)
-   React Router (with Hooks) - [Documentation](https://reacttraining.com/react-router/web/guides/quick-start)
-   Moment.js i18n date/time formatting/parsing library - [Documentation](https://momentjs.com/docs/)
-   Lodash object & collections utility library - [Documentation](https://lodash.com/docs/)
-   react-i18next localization and internationalization library - [Documentation](https://react.i18next.com/)
-   Formik web form state & validation management library - [Documentation](https://jaredpalmer.com/formik/docs/overview)
-   Yup object validation schema builder library - [Documentation](https://github.com/jquense/yup)
-   Redux Store library - [Documentation](https://redux.js.org/introduction/getting-started)
-   Redux React binding library - [Documentation](https://react-redux.js.org/introduction/quick-start)

### Testing

-   JEST unit testing library - [Documentation](https://jestjs.io/docs/en/getting-started)
-   Enzyme React utility testing library - [Documentation](https://enzymejs.github.io/enzyme/)
-   `CAN BE USED` React Hooks Testing Library - [Documentation](https://github.com/testing-library/react-hooks-testing-library)
-   `CAN BE USED` axios-mock-adapter mock library - [Documentation](https://github.com/ctimmerm/axios-mock-adapter)

### Tooling

-   cross-env command utility - [Documentation](https://github.com/kentcdodds/cross-env)
-   Create React App toolkit - [Documentation](https://facebook.github.io/create-react-app/docs/getting-started).
-   Webpack - [Documentation](https://webpack.js.org/concepts/)
-   Husky git events automation - [Documentation](https://github.com/typicode/husky)
-   React DevTools browser extension - [Documentation](https://github.com/facebook/react/tree/master/packages/react-devtools)
-   Redux Store DevTools browser extension - [Documentation](https://github.com/reduxjs/redux-devtools)
-   TSLint static typescript code analyzer - [Documentation](https://palantir.github.io/tslint/usage/cli/)
-   SASSLint static SCSS code analyzer - [Documentation](https://github.com/sasstools/sass-lint)
-   Prettier code formatter - [Documentation](https://prettier.io/docs/en/install.html)
