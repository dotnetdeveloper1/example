# Components Testing

-   [Examples](#examples)
-   [General](#general)
-   [Running Tests](#running-tests)
-   [Fixing Broken Tests](#fixing-broken-tests)
-   [Design Code to be Testable](#design-code-to-be-testable)
    -   [Break code down into testable units](#break-code-down-into-testable-units)
    -   [Avoid direct dependencies on global/external environment/frameworks](#avoid-direct-dependencies-on-globalexternal-environmentframeworks)
    -   [Avoid dependencies on large structures of data when only a small subset of that data is necessary](#avoid-dependencies-on-large-structures-of-data-when-only-a-small-subset-of-that-data-is-necessary)
-   [Mocking API Data and Endpoints](#mocking-api-data-and-endpoints)
-   [Mocking And Testing of React Hooks](#mocking-and-testing-of-react-hooks)
-   [Testing React Components](#testing-react-components)

## Examples

-   API test - [IdentityApiEndpoint.test.ts](../src/api/IdentityApiEndpoint.test.ts)
-   Component test - [DocumentPageViewComponent.test.tsx](../src/components/InvoiceDataCaptureToolkitComponent/DocumentPageViewComponent/DocumentPageViewComponent.test.tsx)
-   Feature Store test - [InvoiceDataCaptureToolkitStoreSlice.test.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/InvoiceDataCaptureToolkitStoreSlice.test.ts)

## General

-   We use Jest as our unit testing framework, and Enzyme for testing React components.
-   In general, each source file (.ts or .tsx) should have a corresponding unit test file (.test.ts or .test.tsx).
-   Try to break unit tests down into separate, simple, focused tests for specific aspects of the expected behavior, edge cases, etc.
-   Aim to test all important behaviors and edge cases.
    -   Use code coverage results as a guide to help identify behaviors and edge cases you forgot, but do NOT assume that code coverage means that you adequately verified the behavior.
-   Make use of nested `describe()`s to organize related tests together.
-   There should be no side effects from one test that affects any other test. Each test should be 100% independent of all other tests.
    -   Make use of `beforeEach()` and `afterEach()` to initialize/reset any common dependencies/mocks that are used by all/many tests.
-   Generally avoid creating complex shared test data that all/many tests in the suite depend on. This creates coupling between unit tests and makes it hard to test new functionality in the future (changes to shared data to support testing new functionality often breaks existing test cases). Create more focused test data for each unit test or group of closely related unit tests.
    -   The closer to the unit test assertions the test data is defined, the easier it is to understand the unit tests.
    -   Ideally, each unit test should define its own input and expected output.
    -   If test data must be shared across closely related tests, group them into a `describe()` and define the test data in the `describe()`.

## Running Tests

There's several ways to run unit tests, depending on what your goal is.

-   `npm run test` is the standard way to run test with watch interactive session.
-   `npm run test:ci` is other way to run all unit tests and generate a code coverage report.
    -   View code coverage report by loading `/coverage/lcov-report/index.html` in a browser.
-   Use VSCode `Run` configuration for Jest tests.
-   Right click on test code to access more options, like debugging the tests (you can put breakpoints in the source code via the IDE).

## Fixing Broken Tests

After making changes to code, you may find that some existing unit tests are now failing. Do NOT simply change the unit test's expected results to match the new actual results to make the failure go away. The failing unit test(s) must be investigated to determine why they are failing before deciding on the appropriate solution. This will require analyzing the unit test to understand its purpose and what it is verifying.

Some possibilities include:

-   The purpose of the unit test and the behavior it is validating is still relevant/desired. The failure could mean:
    -   You actually introduced a bug that caused the test to fail. Find/fix the bug in your changes.
    -   You added extra complexity such that the behavior being tested is still desired, but under different or more specific conditions.
        -   Update the unit test to properly set up the new or more specific conditions for the behavior. Update the unit test description as necessary.
        -   Add new unit tests to confirm correct behavior under other conditions.
    -   You didn't technically introduce a bug in overall desired behavior, but you changed the structure of data/DOM in a way that is no longer compatible with how the unit test was implemented. After confirming that your changes are in fact desired, carefully update or re-implement the unit test to retain its original purpose, but implemented in terms of the new structure. - The original unit test may have been written in a "brittle" way that depended on too many private (possibly irrelevant)
        implementation details. Consider how the test could be reworked to rely on less irrelevant details while still serving its purpose.
    -   A combination of both points above.
-   The purpose of the unit test is no longer relevant because of the changes in functionality that you have implemented.
    -   Are the conditions of the unit test still relevant, but there's now a change in expected behavior? Update the unit test to expect the new desired behavior.
        -   Don't blindly copy the "actual" results from the unit test failure and paste them into the code as the new "expected" results. Analyze the inputs/conditions in the unit test and independently determine the expected result based on your knowledge of the requirements.
        -   Remember to update unit test descriptions as necessary.
-   Did you completely remove the functionality being tested? Deleting the unit test may be the right answer, but also consider whether those specific conditions should still be tested to confirm the new expected behavior.

## Design Code to be Testable

### Break code down into testable units

Only exported/public classes/functions can be reasonably tested by unit tests. Sometimes there's complex private implementation details that are difficult to test via the publicly accessible interface and/or publicly observable behavior.

A solution to this to organize the code into a "module" (see File Naming/Organization). Internally within the module's directory, you can break the code down into separate files with helper functions/classes/components, etc., which are all exported from their respective files. Each of these files would have its own corresponding unit test file to full test the helpers. The main file of "module" would import all the helper functions/classes/components and use them. The unit tests for the main file would focus on higher level behavior of module and not worry about fully testing all the helpers, because the helpers are all individually unit tested already. The `index` file of the "module" would only export the exports of the main file, making all the helpers "private" to the module.

### Avoid direct dependencies on global/external environment/frameworks

Unit tests run in a vacuum without the context of a web browser, a deployment environments, or any API services. Code that directly depends on such global/external environment/framework, or complex objects/data provided by global/external environment/framework, will be difficult to unit test, because those global/external environments/frameworks will need to be simulated/mocked.

It is much easier to simulate such dependencies when code does not directly depend on them, but depends on abstractions (params/props that provide specific pieces of necessary information, callbacks for performing actions or retrieving information, an implementation of an interface, etc). In the context of a unit test, implementations of these abstractions can be provided and precisely controlled much more easily than simulating the entire environment /framework. In the context of the application code, implementations can be provided in terms of the environment/framework. "Dependency Injection" is an example of this concept.

### Avoid dependencies on large structures of data when only a small subset of that data is necessary

This is similar to "Avoid direct dependencies on global/external environment".

For example, imagine we load information about an invoice from the API and want to present some of that information in a view. A common instinct is to pass that entire API response object structure (which may have 50+ properties, complex nested objects, etc.) to the view component, then implement the view component to present the few properties that are relevant (e.g., Name, Number, Total).

This both makes unit testing harder and reduces the reusability. Even though the view only actually depends on 3 properties of the structure, it is technically coupled with the entire structure. This leads to the following problems:

-   In unit tests, you are forced to mock the entire complex structure, even though only 3 properties are relevant. You will be tempted to do bad things like:
    -   Construct a bare minimum representation of the complete API Invoice response containing the 3 relevant properties, and all other required properties defaulted to arbitrary/invalid values.
    -   Use type casting to bypass compiler errors and pass in an object containing only the 3 relevant properties.
-   If you want to re-use the view in another context where values for the 3 relevant properties are available, but the entire API Invoice response is NOT available, you will be tempted to do bad things like:
    -   Construct a bare minimum representation of the complete API Invoice response containing the 3 relevant properties, and all other required properties defaulted to arbitrary/invalid values.
    -   Use type casting to bypass compiler errors and pass in an object containing only the 3 relevant properties.
-   While maintaining/enhancing the view in the future, you will be tempted to directly reference the many other properties that were not originally relevant, increasing the actual dependency on the entire API response structure.
    -   If anyone did any of the above bad things, then some usages of the view may not be obscurely broken in ways that are only evident at run time, conditionally based on the state of the data.

Instead, consider defining an interface for your view that contains only the specific data that the view needs. It helps clarify what the view actually depends on, makes it easier to write unit tests, and makes the view more generally reusable.

## Mocking API Data and Endpoints

If the code you are testing directly calls any API endpoints, then you MUST mock them. Runtime errors will tell you when an unmocked endpoint is called. A common pattern is to mock all necessary endpoints with very generic implementations for all unit tests (in `beforeEach()`), unmock all necessary endpoints in `afterEach()`, and then mock specific endpoints with more specific implementations as needed within specific unit tests.

You may want to validate request parameters to the endpoint call. If you need to validate whether the endpoint was called, or how many times, then you'll have to make use of a `jest.fn()` wrapper. Check `src/mocks/axios.ts` code and use cases. If it's not enough than try to use `axios-mock-adapter` mock library - [Documentation](https://github.com/ctimmerm/axios-mock-adapter).

## Mocking And Testing of React Hooks

Generally hooks should be tested with components that consuming them, but for complex scenarios React Hooks Testing Library can be used - [Documentation](https://github.com/testing-library/react-hooks-testing-library). It will simply hooks testing without injection to a fake component.

## Testing React Components

-   Define the specific type of an Enzyme ReactWrapper for the component that you are testing: `type MyComponentWrapper = EnzymeWrapper<typeof MyComponent>;`
-   In each unit test render your component using `mount()` and typecast the result to your `MyComponentWrapper` type for type safety and IDE assistance.
    -   `const wrapper = mount(<MyComponent isAwesome={true} />) as MyComponentWrapper;`
    -   Provide values for props that are relevant to the unit test.
-   Use the resulting `wrapper` to inspect how the component is rendered, etc.
    -   See [Enzyme's API Reference](https://enzymejs.github.io/enzyme/).
-   Unmount the component via `wrapper.unmount()` at the end of the test.
    -   For most components this isn't important, but it may be important for components that perform cleanup while unmounting. It's safest to get in the habit of always unmounting.
-   Only test the outer component. If you are digging into private implementation details of a child component, you're probably doing it wrong.
    -   Validate that a child component is rendered correctly by testing its props (the values of the its props that were provided by the parent component that you are testing).
        -   No need to validate that internal implementation details of how the child component renders itself. That should be covered by the child component's own unit tests. All you care about is that your component passed the correct props values to its child component.
    -   To simulate an event being triggered by a child component, simply directly call the relevant prop of that child component and pass exactly whatever params to that event that you want to simulate.
        -   There's no need to simulate internal implementation details within the child component that lead up to the event being triggered. The child component's own unit tests take care of simulating that and ensuring that it leads to the event being triggered properly. All you care about is how the parent component reacts to the event from the child element.
