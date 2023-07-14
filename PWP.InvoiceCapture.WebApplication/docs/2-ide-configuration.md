# IDE Configuration

-   [General](#general)
    -   [WebStorm IDE](#webstorm-ide)
    -   [Visual Studio Code Get Started Guide](#visual-studio-code-get-started-guide)
-   [Visual Studio Code Extensions](#visual-studio-code-extensions)
-   [Browser Developer Extensions](#browser-developer-extensions)
-   [Debug Session](#debug-session)
    -   [Application Debugging (VSCode + Chrome)](#application-debugging-vscode--chrome)
    -   [Application Debugging (Browser)](#application-debugging-browser)
    -   [Unit Tests Debugging (VSCode)](#unit-tests-debugging-vscode)

## General

Front-end development can be done by using of any text editor, terminal and browser, but the most powerful front-end IDE's, such as JetBrains WebStorm or VS Code, provide a set of valuable productivity features and should be used by default.

### WebStorm IDE

The current guide targets Visual Studio Code, but you can also check [Official WebStorm Guide for React](https://www.jetbrains.com/help/webstorm/react.html#) and configure the project by yourself.

### Visual Studio Code Get Started Guide

1. Install [VS Code](https://code.visualstudio.com/Download).
2. Configure your preferred Keymap (File -> Preferences -> Keymaps), Theme (File -> Preferences -> Color Theme) and Icons (File -> Preferences -> File Icon Theme).
3. Install all recommended by the project Workspace extensions (See [Visual Studio Code Extensions](#visual-studio-code-extensions)).
4. Install React+Redux developer tools for Chrome and/or Firefox (See [Browser Developer Extensions](#browser-developer-extensions)).
5. Look at [Official VS Code User Guide](https://code.visualstudio.com/docs/editor/codebasics).
6. Configure the project environment with [Getting Started Guide](../README.md#getting-started).
7. Try to debug the application using [Application Debugging (VSCode + Chrome)](#application-debugging-vscode).
8. Try to debug Jest tests using [Unit Tests Debugging (VSCode)](#unit-tests-debugging-vscode).
9. You can start development of new features!

## Visual Studio Code Extensions

`.vscode` directory contains [`extensions.json`](../.vscode/extensions.json) file with the list of recommended extensions. See [Workspace Recommended Extensions Install Guide](https://code.visualstudio.com/docs/editor/extension-gallery#_workspace-recommended-extensions) for manual installation or use shortcut script to install them all:

```bash
code --install-extension adamwalzer.scss-lint
code --install-extension andys8.jest-snippets
code --install-extension ban.spellright
code --install-extension christian-kohler.npm-intellisense
code --install-extension christian-kohler.path-intellisense
code --install-extension codezombiech.gitignore
code --install-extension CoenraadS.bracket-pair-colorizer
code --install-extension dbaeumer.vscode-eslint
code --install-extension DotJoshJohnson.xml
code --install-extension DSKWRK.vscode-generate-getter-setter
code --install-extension eamodio.gitlens
code --install-extension EditorConfig.EditorConfig
code --install-extension eg2.vscode-npm-script
code --install-extension esbenp.prettier-vscode
code --install-extension firefox-devtools.vscode-firefox-debug
code --install-extension firsttris.vscode-jest-runner
code --install-extension formulahendry.auto-close-tag
code --install-extension formulahendry.auto-rename-tag
code --install-extension glen-84.sass-lint
code --install-extension jasonnutter.search-node-modules
code --install-extension michelemelluso.code-beautifier
code --install-extension mikestead.dotenv
code --install-extension mrmlnc.vscode-scss
code --install-extension ms-vscode.powershell
code --install-extension ms-vscode.vscode-typescript-tslint-plugin
code --install-extension msjsdiag.debugger-for-chrome
code --install-extension oderwat.indent-rainbow
code --install-extension Orta.vscode-jest
code --install-extension pflannery.vscode-versionlens
code --install-extension pranaygp.vscode-css-peek
code --install-extension rbbit.typescript-hero
code --install-extension redhat.vscode-yaml
code --install-extension steoates.autoimport
code --install-extension streetsidesoftware.code-spell-checker
code --install-extension stringham.move-ts
code --install-extension waderyan.nodejs-extension-pack
code --install-extension xabikos.JavaScriptSnippets
code --install-extension yzhang.markdown-all-in-one
code --install-extension Zignd.html-css-class-completion
```

## Browser Developer Extensions

-   React Developer Tools: [Chrome Extension](https://chrome.google.com/webstore/detail/react-developer-tools/fmkadmapgofadopljbjfkapdkoienihi), [Firefox Extension](https://addons.mozilla.org/en-US/firefox/addon/react-devtools/)
-   Redux Developer Tools: [Chrome Extension](https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd), [Firefox Extension](https://addons.mozilla.org/en-US/firefox/addon/reduxdevtools/)

## Debug Session

### Application Debugging (VSCode + Chrome)

1. Run `npm run start` in VSCode integrated terminal to start local dev server.
2. Set breakpoints in the application source code.
3. Switch to `Run` view (See [Docs](https://code.visualstudio.com/docs/editor/debugging#_run-view) for details).
4. Choose `Chrome` configuration and press `Start Debugging`.
5. Use `Variables`, `Watch`, `Call Stack` and `Debug Console` for code inspection.

**NOTE:** React and Redux DevTools should be enabled additionally in Developer Mode to be accessible in Debug session.

### Application Debugging (Browser)

1. Run `npm run start` in VSCode integrated terminal to start local dev server.
2. Open `localhost:8081` in browser.
3. Press `F12`.
4. Navigate to any file source map in `Sources` tab under `static/js/<local source directory path>` folder for code inspection or use `CTRL+SHIFT+F` for quick source code navigation.

### Unit Tests Debugging (VSCode)

1. Go to test file.
2. Set breakpoints in the application or unit test source code.
3. Switch to `Run` view (See [Docs](https://code.visualstudio.com/docs/editor/debugging#_run-view) for details).
4. Choose `Debug Jest Tests` or `Debug Jest Tests - Current File`.
5. Use `Variables`, `Watch`, `Call Stack` and `Debug Console` for code inspection.

**NOTE:** VSCode Jest extensions provide `Run Jest`, `Run Jest File` and `Debug Jest` context menu items, but correctness of them are not guaranteed, because string interpolation feature (i.e, describe(\`\${COMPONENT_NAME}\`) constructions) is breaking extension debug startup script. You can temporary change `describe()` and/or `test()` names to static `"<name>"` values and debug Jest tests from extension runner.

**NOTE:** [VSCode Jest extension](https://github.com/jest-community/vscode-jest) automatically detect Jest tests and run test suite on every change in the project. `Jest: ...` mark in VSCode status bar will show result of the latest test run. Also it can annotate source code with Jest Coverage Report. Press `CTRL+SHIFT+P` (Command Pallete) and type `Jest: Toggle Coverage Overlay` to see project test coverage.
