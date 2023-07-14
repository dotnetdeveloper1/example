# Components Development

-   [Code Patterns](#code-patterns)
-   [Component Conventions](#component-conventions)
    -   [Feature or Page Component Structure](#feature-or-page-component-structure)
    -   [Common or Sub Component Structure](#common-or-sub-component-structure)
    -   [Element Component Structure](#element-component-structure)
-   [General TypeScript Guidelines](#general-typescript-guidelines)
    -   [TypeScript Resources](#typescript-resources)
-   [Naming Things](#naming-things)
    -   [Special Names for Variables/Parameters/Properties](#special-names-for-variablesparametersproperties)
    -   [Special Names for Functions](#special-names-for-functions)
-   [Language Feature Usage](#language-feature-usage)
    -   [Declaring Types](#declaring-types)
    -   [Type `any`](#type-any)
    -   [Non-Null Assertion](#non-null-assertion)
    -   [Object Destructuring](#object-destructuring)
-   [React Components](#react-components)
    -   [General Guidelines](#general-guidelines)
    -   [Component Name](#component-name)
    -   [Event Handlers](#event-handlers)
        -   [Inline Arrow Function](#inline-arrow-function)
        -   [`useCallback`, `useMemo`, `useReducer` Arrow Functions (via Custom Component React Hooks)](#usecallback-usememo-usereducer-arrow-functions-via-custom-component-react-hooks)
        -   [Private Arrow Function Property](#private-arrow-function-property)
        -   [Patterns to Avoid](#patterns-to-avoid)

## Code Patterns

| Name                     | Description                                                                                                                              | Sample(s) in the project                                                                                                                      |
| ------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| Functional Component     | General function that receives props objects and returns JSX node                                                                        | [LoadingPlaceholder.tsx](../src/components/common/LoadingPlaceholder.tsx)                                                                     |
| Stateless/Pure Component | Any type of React component that doesn't use `this.state` object or `useState` hook                                                      | [NextButton.tsx](../src/components/InvoiceDataCaptureToolkitComponent/DocumentControlPanelComponent/NextButton.tsx)                           |
| Stateful Component       | Any type of React component that use `this.state` object or `useState` hook                                                              | [PageNavigationView.tsx](../src/components/InvoiceDataCaptureToolkitComponent/DocumentControlPanelComponent/PageNavigationView.tsx)           |
| Custom Component Hook    | Helper function that encapsulate all React Hooks usage for specified component                                                           | [useMultiSelectionPane.ts](../src/components/InvoiceDataCaptureToolkitComponent/DocumentPageViewComponent/hooks/useMultiSelectionPane.ts)     |
| Forward Ref Component    | Any type of React component that automatically passing a ref(Imperative State Management API) through a component to one of its children | [DocumentPageViewComponent.tsx](../src/components/InvoiceDataCaptureToolkitComponent/DocumentPageViewComponent/DocumentPageViewComponent.tsx) |
| Connected Component      | Any type of React component that has direct reference to React context object, React Router parameters or Redux Store state              | [InvoiceDataCaptureToolkitComponent.tsx](../src/components/InvoiceDataCaptureToolkitComponent/InvoiceDataCaptureToolkitComponent.tsx)         |

## Component Conventions

### Feature or Page Component Structure

Examples:

-   [InvoiceDataCaptureToolkitComponent.tsx](../src/components/InvoiceDataCaptureToolkitComponent/InvoiceDataCaptureToolkitComponent.tsx)

Feature or Page Components should define only business valuable parts of application functionality, because only in that situation amount of infrastructure code will help in the future code maintainability. Component has their own feature Redux Store, optional routing configuration and can rely on Redux context or hooks. It provides action dispatchers via event handlers and store state into child component props.

-   `<ComponentName>Component.tsx` file where functional component and `COMPONENT_NAME` constant are defined.
-   `<ComponentName>Component.test.tsx` file where all necessary component tests are located.
-   `<ComponentName>Component.scss` file where component styling are located (with all related "Element" components styling).
-   `hooks` folder where all necessary custom component hooks are located.
    -   `index.ts` where all public custom hooks are re-exported.
-   `store` folder where feature Redux Toolkit store slice is defined.
    -   Required definition of feature "-Slice".ts and "-State".ts files
    -   Optional `actions` folder for `Redux-Thunk` async action handlers.
    -   Optional `mappers` folder with state mapping helpers.
    -   Optional `reducers` folder with collection of action handling functions.
    -   Optional `selectors` folder with feature store state slice computation helpers.
    -   Optional `state` folder with definition of feature state children models.
-   `index.ts` file.
-   Optional `<Element>.tsx` files with child partial views.

Feature or Page Component should be only located in `src/components/<<ComponentName>Component>` folder.

### Common or Sub Component Structure

Examples:

-   [DocumentViewComponent.tsx](../src/components/InvoiceDataCaptureToolkitComponent/DocumentViewComponent/DocumentViewComponent.tsx)
-   [InformationModal.tsx](../src/components/common/InformationModal.tsx)

Common or Sub Components can define any type of application functionality, but they're not connected directly to Redux Store and don't rely on Redux context or hooks. Instead of it, they should define all necessary props which values will be provided by Feature or Page Component.

Common or Sub Component consists from:

-   `<ComponentName>Component.tsx` file where functional component and `COMPONENT_NAME` constant are defined.
-   `<ComponentName>Component.test.tsx` file where all necessary component tests are located.
-   `<ComponentName>Component.scss` file where component styling are located (with all related "Element" components styling).
-   `hooks` folder where all necessary custom components hooks are located.
    -   `index.ts` where all public custom hooks are re-exported.
-   `index.ts` file if component has own folder.
-   Optional `<Element>.tsx` files with child partial views.

Common or Sub Component component can be located in separate folder or be in the same folder where parent component located. It depends from component complexity.

### Element Component Structure

Examples:

-   [Toolkit.tsx](../src/components/InvoiceDataCaptureToolkitComponent/Toolkit.tsx)
-   [ToggleCompareBoxesButton.tsx](../src/components/InvoiceDataCaptureToolkitComponent/DocumentControlPanelComponent/ToggleCompareBoxesButton.tsx)

"Element" component is a simple partial view that should strictly used only by one "Block" component in BEM terminology.

"Element" component consists from:

-   `<Element>.tsx` file
-   Optional `<Element>.test.tsx` file
-   Component should import `COMPONENT_NAME` constant from parent component module.
-   Component can use a few React Hooks, but preferable to form and pass all React Hooks results from parent component.

"Element" component is refactoring evolution from private "render-" functions within parent component module.

"Element" components should be in the same folder when parent component located.

## General TypeScript Guidelines

### TypeScript Resources

-   [Documentation](https://www.typescriptlang.org/docs/home.html)
    -   The "Handbook" section has a lot of good stuff.
-   [Official Language Spec](https://github.com/Microsoft/TypeScript/blob/master/doc/spec.md)

## Naming Things

Generally, use descriptive names that make the code intuitive to understand.

Name things based on what the mean within the context of the code in which it is defined/used, rather than what it means in the context of an external usage. For example: parameters of a function should be named in terms of what they mean to the implementation of the function; not in terms of what some value in external code represents that is passed as an argument to the function.

-   Use `lowerCamelCase` style for functions, parameters, properties, variables.
-   Use `UpperCamelCase` for classes, interfaces, enums, type names, and React component names (even if it is a function component).
-   Capitalize only the first letter of an acronym. For example: `getHtmlText` instead of `getHTMLText`.
-   Use `ALL_CAPS` for variables that hold "constants" - static data like string constants, numeric constants, etc.
-   No Hungarian notation style prefixes or suffixes (Except I- prefix for interface types and -Type suffix for type expressions).
-   Generally avoid abbreviations. The full word is easier to understand, it's just a few more characters, and the IDE can auto-complete it for you to save keystrokes. Very common/standard abbreviations are fine, like "id" for "identifier".
-   Generally avoid acronyms. Acronyms that are very common/standard are fine, like "html", but don't make up new acronyms to save characters.
-   Use noun phrases for variables, parameters, classes., etc., that describe what its value represents.
-   Use verb phrases for functions that describe what the function does.

### Special Names for Variables/Parameters/Properties

-   -Id suffix: If its value is the ID of something, then add the -Id suffix to make it clear that it is an ID, and not the entire thing itself.
-   -Promise or -Async suffix: Use this suffix for Promises. The base of the name should describe what the Promise resolves to.
-   -ByXyz suffix: Use this suffix for Maps (or objects used as maps). The base of the name should describe the values stored in the map, and the "Xyz" part of the suffix should describe the key of the map. A common suffix is "-ById" when it is a map of things keyed by their IDs.
-   -Is or -Has prefix: If its value is computed boolean of something, then add the -Is or -Has prefix to make it clear that value has computed logic expression.

### Special Names for Functions

-   render- prefix: A function that renders content and returns it as JSX (DOM) node.
-   on- prefix: An event (not event handler!).
-   handle- prefix: A function that is specifically designed to exactly match the signature of an event and be used directly as the implementation of an event handler. It should be named consistently with the name of the event it is designed to handle. If a function is simply called from the implementation of an event handler, but is not directly used as the event handler itself, then do not use this naming. Instead, simply name the function for what it actually does. Example: handler for a cancel button's `onClick` event could be named `handleCancelButtonClick`. The general pattern is `handle[WhatFiresTheEvent][EventName]`.
-   get- prefix: A function that returns an intrinsic property of an object. The result can be calculated, but only use the "get-" prefix for things that can be considered part of the essence of what defines the object. Consider other prefixes like `calculate-`, `generate-`, `create-`, etc., for methods that produce a result in terms of the object that isn't really intrinsic to the definition of the object itself.
-   request- prefix: A function that makes an asynchronous request for data and returns a Promise that resolves to the requested data.
-   load- prefix: A function that loads data into some internal state of an object/component. A common example is somewhat similar to a "request-" function in that it asynchronously requests data, but also handles the response of the request itself and loads data from the response into internal state. A "load-" function may still return a Promise to indicate completion/success, but should generally not actually expose the loaded data via the returned Promise.
-   use- prefix: A function that encapsulate components control logic via React Hooks (i.e. custom component hook).
-   update- prefix: A function that used in `useEffect` logic and re-compute internal component state.

## Language Feature Usage

These are some guidelines about when/how to use specific TypeScript/ES6 language features.

### Declaring Types

Our project is setup to never allow implicit `any` types. This means that the type of every variable, property, parameter, etc., must either have its type explicitly declared, or or its type must be inferred from context.

In general, prefer allowing types to be automatically inferred where possible (avoid unnecessarily declaring types).

Examples of situations where an explicit type declaration is required/preferred:

-   Params and return values of all (non-arrow) functions.
-   Properties of all interfaces.
-   A var/const/property that is not immediately initialized to a value.
-   A var/const/property that is initialized to an object literal that is intended to be an implementation of a particular interface.
    -   Type declaration gives you more immediate feedback/assistance in creating a valid object literal.
    -   Without a type declaration, any object literal is technically valid at this point in code, and errors will occur later when you incorrectly assume the object implements a particular interface.
-   A var/const/property whose value will be an arrow function, and a type exists that defines the desired function signature.
    -   Using a type declaration will prevent you from duplicating the types of the params and return value and guarantee consistency/correctness.
-   Return type of an arrow function whose implementation returns an object literal that is intended to be an implementation of a particular interface.
    -   Type declaration gives you more immediate feedback/assistance in creating a valid object literal in the implementation of the arrow function.
    -   Without a type declaration, any object literal is technically valid at this point in code, and errors will occur due to signature mismatch with the inferred return type.

Examples of situations where an explicit type declaration is NOT required/preferred:

-   A var/const/property that is initialized to a well-typed expression.
    -   If you want to purposely make your var/const/property have a different type than that of its initial value, then typecast the initializer to clearly communicate this intent. For example, if you are initializing a local const to the value of a parameter/property of type `any`, but you know for sure that its value will actually be a specific type (e.g., number), then do something like "const foo = value as number;" rather than "const foo: number = value;"
    -   Params of an arrow function whose signature is inferred from context (passed as a param, assigned to a property of an interface, assigned to a var/const/property with an explicit type declaration, etc).
        -   Allowing the param types to be inferred is much more convenient and guarantees correctness.

### Type `any`

Do not declare something as type `any` out of laziness. Always specify the actual type if possible. Valid uses of type `any` are usually in very generalized implementations of code that truly does not care what type a value is. Generics are preferable to type `any` where applicable.

### Non-Null Assertion

Use non-null assertions carefully. If TypeScript complains that something may be undefined, do NOT blindly slap a non-null assertion on it to "fix" the error. Doing so may simply mask a bug at compile time that now only exposes itself at runtime under certain conditions. The compiler error is there for a reason: the typescript compiler believes that the value can sometimes be undefined. Only add a non-null assertion if you 100% know that it is impossible for that code to execute while the value is undefined. Otherwise, update the code to properly handle the possibility of an undefined value.

When coming to the conclusion that a non-null assertion is a valid solution, consider:

-   Can the code be reasonably reorganized so that the compiler is correctly aware of the type as NOT being possibly undefined?
-   Is it obvious from the context of the code that the value should never be undefined, or is a comment necessary to justify the use of the non-null assertion?
    -   If a comment is necessary, can the code instead be reorganized to make a comment unnecessary?

### Object Destructuring

Generally avoid using object destructuring because it usually causes a loss of context. For example, many React tutorials/examples will destructure the `props` object into several local variables of the `render` function. We prefer to NOT destructure in situations like this so that it is more clear where `props` is being used. It is also more convenient while writing code to start typing `props.` and let IDE auto completion suggest to you what is available on the object, versus checking to see if the property you want has already been destructured from the object, possibly adding the desired property to the destructured assignment, then using the local variable.

Some exceptions to the rule:

-   Destructuring a tuple into named variables. Destructured variable names are much more meaningful than hard-coded indexes into the tuple.
-   Destructuring an object whose only purpose is package a couple related values together like multiple named parameters, but otherwise is not a meaningful concept.
    -   For example, a function that has to return more than one value, and chooses to do so via properties of an object.
    -   Such objects are usually very difficult to name/describe, because the object as a whole isn't very meaningful; only the individual properties are (which is why destructuring often provide more readable code, rather than coming up with a name for the variable containing the entire object).

## React Components

### General Guidelines

-   One exported component per file.
-   Define `I<ComponentName>Props` interface for every component.
-   Inherit child props in parent component props interface when it's needed. Use `Pick<>`, `Omit<>`, `ReactPropsType<>` helper types.
-   Use "-Component" suffix for feature, page or sub components.
-   Enclose React Hooks into custom component hook with "use-" prefix to hide implementation details from functional component view (Think as Controller pattern).
-   Define `I<ComponentName>HookResult` interface for components hook return type.
-   Define `I<ComponentName>State` interface when it's needed in components hook implementation module.
-   Avoid giant `render()` or functional component implementations.
    -   Break down into smaller helper (i.e., Element) functions or functional components that render parts of the component.
        -   Each function that renders a part of the component should have a name starting with the "render-" prefix.
        -   Each functional component that renders a part of the component should not have a name ending with the "-Component" suffix.
        -   "render-" functions or "Element" components should use the same COMPONENT_NAME constant as component where they're used.
    -   If possible/reasonable, abstract these smaller parts into separate components that can be more easily/fully unit tested. It also simplifies unit testing of the larger outer component (See the "Modules" section of File Naming/Organization).

### Component Name

-   Each component module should define/export a COMPONENT_NAME const whose value is the name of the component.
-   Use COMPONENT_NAME internally:
    -   Add it as a CSS class name to the main element of the component.
    -   Use it to build BEM CSS class names for elements within the component.
    -   Use it as the value of the component's static `displayName` property (used in debugging tools)
-   Use COMPONENT_NAME externally:
    -   In unit tests, use it to build CSS selectors for elements rendered by the component.
    -   In "render-" functions or "Element" functional components to build BEM CSS class names for elements within the JSX node.

### Event Handlers

There are many different ways to define and pass event handler implementations to components. Here's the patterns we prefer.

#### Inline Arrow Function

An inline arrow function is the simplest and most convenient approach. This should be the default approach, unless there's a reason to avoid it.

Pros:

-   "this" context is automatically what you most likely expect/want.
-   Has access to other local variables in the surrounding render function implementation (values can be derived from props/state once and shared across multiple inline arrow functions).
-   The IDE can auto-complete the parameter list, and TypeScript infers the types of the parameters correctly. You don't have to figure out and duplicate the event signature yourself.
-   Simple inline implementations can make it easier to understand whole the component is configured to work as a whole, without needing to bounce around the file looking at multiple functions.
-   You don't have to come up with a name and description of a function, document its params/return, etc.
-   Can be used as a simple translation layer between the signature of the event and the signature of a private method that performs the bulk of the work. Multiple events across different components with differences in signatures can share a common core implementation in this way.

Cons:

-   Becomes less readable as the implementation becomes bigger (consider abstracting the core implementation out to a private method, or abstracting the entire handler out to a private arrow function property).
-   Can possibly become a performance issue on a component that is heavily used.
    -   Each time the parent component's `render()` function is called, there is some overhead of creating the arrow function.
    -   If the child component is a Pure component, or otherwise optimized to only re-render when props are detected to be changed in particular ways, then an arrow function event handler would break this optimization (parent component creates a new arrow function every time, which can be a false positive change detection).
-   **IF** a performance issue is confirmed, it can likely be solved with a private arrow function property implementation.
    -   Beware of possible incorrect behavior (see "Cons" of "Private Arrow Function Property" below).
    -   Avoid premature optimization.

#### `useCallback`, `useMemo`, `useReducer` Arrow Functions (via Custom Component React Hooks)

Define an arrow function wrapped into `useCallback`, `useMemo` or `useReducer` of the parent component and directly pass it as the value of the child component's event prop.
Use the following pattern for the name of the property: "on[ChildComponentDescription][eventname]" or "handle[ChildComponentDescription][eventname]".

#### Private Arrow Function Property

Define an arrow function as a private readonly property of the parent component class, and directly pass it as the value of the child component's event
prop.

-   Use the following pattern for the name of the property: "handle[ChildComponentDescription][eventname]".
    -   Example: A handler for a Button's "onClick" event. The button is the "Cancel" button. The event handler property would be named "handleCancelButtonClick".
-   Don't duplicate the function signature of the event. Instead, make use of the `ReactPropFunctionType` helper type to extract the type of the event prop, and use that as the type of the property.
    -   Doing this allows the IDE to auto-complete the arrow function's parameter list for your, and allows TypeScript to infer the types of the parameter.
-   Documentation for this property should describe it as a handler for the particular event of the particular kind of component.
-   The important concept here is that this arrow function property is specifically designed to be the handler of a particular event of a particular component. This is what allows us to pass it directly to the child' component as the event prop's value with confidence.
-   Do not explicitly call this function to implement the same behavior from other contexts. If you want to reuse the implementation of the function, then split it out to a separate private function that is called both from the event handler and from other contexts that require the same behavior.

Use this pattern as necessary if the "Inline Arrow Function" approach is not appropriate.

#### Patterns to Avoid

-   Passing a reference to a method of `this` class instance that is not an arrow specifically designed/implemented as a handler for the event.
    -   Direct reference to a props value whose signature is not guaranteed to exactly match. This can lead to subtle bugs if the signatures are assignable, extra parameters are passed, and the implementation of the prop unintentionally processes the extra parameter.
    -   Direct reference to a method of `this` class that is not an arrow function. The `this` context will not be correct when executing the function.
-   `this.handleXyz = this.handleXyz.bind(this)` in the constructor, then passing `this.handleXyz` directly as a prop value.
    -   Instead define `handleXyz` as a private arrow function which will already have its context bound.
