# Store Logic & State

-   [General](#general)
-   [Feature Redux Store (Slice)](#feature-redux-store-slice)
-   [Reducers](#reducers)
-   [Actions](#actions)
    -   [Action dispatching](#action-dispatching)
-   [Selectors](#selectors)
-   [Thunk Actions](#thunk-actions)
-   [State Mapping](#state-mapping)

## General

Using of Redux store aims decoupling of state management logic from details of application implementation. Following of [Flux](https://facebook.github.io/flux/docs/in-depth-overview) architecture pattern will form unidirectional data flow in the application.

Global store configuration is located in [configuration.ts](../src/store/configuration.ts). All feature slices should be registered here.

Documentation references:

-   [React Redux Toolkit](https://redux-toolkit.js.org/introduction/quick-start)
-   [Redux Store library](https://redux.js.org/introduction/getting-started)
-   [Redux React binding library](https://react-redux.js.org/introduction/quick-start)
-   [Redux Store DevTools browser extension](https://github.com/reduxjs/redux-devtools)

## Feature Redux Store (Slice)

Examples:

-   [InvoiceDataCaptureToolkitStoreSlice.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/InvoiceDataCaptureToolkitStoreSlice.ts)
-   [InvoiceDataCaptureToolkitState.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/InvoiceDataCaptureToolkitState.ts)

Any Feature or Page Component should have their own feature store definition, or in other words - slice. Creation of new feature slice divided into:

1. Describing of feature store state interfaces and default state object. Store state should have **clean objects**, `classes are not allowed`. Store can have nested state objects. It depends from feature requirements.

2. Using of `createSlice` function from `redux-toolkit` and defining:
    - Initial feature store state. Function will infer feature store state type from the type of initial state object.
    - Use `reducers` field to set collection of feature store actions. Reducer function can be defined as inline arrow function or can be imported from `reducers` folder.
    - Use `extraReducers` field to associate additional reducers with `thunk-action`'s.

## Reducers

Examples:

-   [changeInvoiceFieldDataAnnotationReducer.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/reducers/changeInvoiceFieldDataAnnotationReducer.ts)
-   [assignLayoutItemsToInvoiceFieldReducer.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/reducers/assignLayoutItemsToInvoiceFieldReducer.ts)

Redux Toolkit helps with definition of state reducing logic. It use [Immer.js](https://immerjs.github.io/immer/docs/introduction) library under the hood. Without toolkit helpers, reducer will be looked like function with large `switch (action.type)` branch and new state object building logic for all switch cases. For large features this kind of reducer implementation produce unreadable code. Immer.js helps here by providing state proxy logic which add ability to directly mutate state fields. Library will automatically check where changes applied and form new state object after reducer action call.

In our project reducer is a pure function that should be written in manner that the same input state and action payload will produce the same output state. Any kind of side-effect logic (API requests, animations, browser API interactions) should be moved into `Redux-Thunk` actions or directly into component logic.

Creation of new reducer:

1. Define new function in `reducers` folder with the following arguments:
    - `state` argument that have feature store state interface type
    - optional `action` argument that have `Action<PayloadType>` type
2. Add export statement to `index.ts` file in `reducers` folder
3. Add assignment of new reducer function to new action in `reducers` object of `createSlice` function call.

## Actions

Example:

See `InvoiceDataCaptureToolkitStoreSlice.actions` export statement in [InvoiceDataCaptureToolkitStoreSlice.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/InvoiceDataCaptureToolkitStoreSlice.ts)

Redux Toolkit helps with action types definition. It provides `createAction` helper function, but we're using inferred action definition notation.

All actions are defined in `reducers` and `extraReducers` sections of `createSlice` function call argument.

Action name will be inferred from field name of `reducers` object. See example:

```typescript
reducers: {
    actionName: actionReducer;
}
```

After implementation of new action reducer and adding of new action to `reducers` object. You need to extend `.actions` export statement with new action.

Thunk actions has their own set of actions and appropriate action reducers should be registered in `extraReducers` builder.

### Action dispatching

See dispatch usage example in [useDocumentViewState.ts](../src/components/InvoiceDataCaptureToolkitComponent/hooks/useDocumentViewState.ts).

Generally you need to import `useDispatch` Redux hook and define callback that wraps invocation of specific action creator and dispatching of the created action.

## Selectors

Examples:

-   [invoiceFieldsInFocusSelector.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/selectors/invoiceFieldsInFocusSelector.ts)

-   [assignedDocumentLayoutItemsSelector.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/selectors/assignedDocumentLayoutItemsSelector.ts)

Redux store selector is a state mapping function. Selectors are helpful in situations when the same state mapping logic should be reused or it has complex implementation and should be defined as testable unit. Redux can remember selector result and use caching optimization techniques.

All feature stores should have hook helper like `useToolkitSelector` in [index.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/selectors/index.ts), because default `useSelector` hook provide global state object and require additional overheads in usage of feature state selectors.

## Thunk Actions

Example:

-   [FetchInvoiceDataAsync.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/actions/FetchInvoiceDataAsync.ts)

By default actions in Redux are dispatched synchronously, which is a problem for any non-trivial application that needs to communicate with an external API or perform side effects. Thankfully though, Redux allows for middleware that sits between an action being dispatched and the action reaching the reducers. There are two very popular middleware libraries that allow for side effects and asynchronous actions: [Redux Thunk](https://github.com/reduxjs/redux-thunk) and [Redux Saga](https://github.com/redux-saga/redux-saga).

Redux Thunk is a middleware that lets you call action creators that return a function instead of an action object. That function receives the store’s dispatch method, which is then used to dispatch regular synchronous actions inside the body of the function once the asynchronous operations have completed.

If you’re curious, a [thunk](https://en.wikipedia.org/wiki/Thunk) is a concept in programming where a function is used to delay the evaluation/calculation of an operation.

In our project thunk actions are defined with help of `createAsyncThunk` helper from `redux-toolkit`.

Thunk action should have:

1. Async function implementation with action options argument and access to `thunkAPI`.
2. Reducer implementations for `.pending`, `.fulfilled`, `.rejected` thunk states which should be registered in `extraReducers` feature slice builder.

Creation and dispatching of thunk actions are the same as for normal Redux actions (See [Action Dispatching](#action-dispatching)).

## State Mapping

Example:

-   [ApiModelMapper.ts](../src/components/InvoiceDataCaptureToolkitComponent/store/mappers/ApiModelMapper.ts)

State mapping guideline isn't related to Redux itself, but will help with organizing of complex object transformation logic from/to feature store state. For example, in progress of loading/saving/validation of Invoice data.
