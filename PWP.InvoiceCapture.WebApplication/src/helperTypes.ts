import { ReactWrapper } from "enzyme";

/**
 * Extracts the type of a React component's Props from the React component type T.
 * Usage example: type MyComponentProps = ReactPropsType<typeof MyComponent>;
 */
export type ReactPropsType<T> = T extends React.ComponentType<infer P>
    ? P
    : T extends React.Component<infer P>
    ? P
    : never;

/**
 * Extracts the type of a React component's State from the React component type T.
 * Usage example: type MyComponentState = ReactStateType<typeof MyComponent>;
 */
export type ReactStateType<T> = T extends React.ComponentClass<any, infer S>
    ? S
    : T extends React.Component<any, infer S>
    ? S
    : never;

/**
 * Extracts the type of a React ForwardRefComponent's ref from the React component of type T.
 * Usage example: type MyComponentRefType = ReactRefType<typeof MyComponent>;
 */
export type ReactRefType<T> = T extends React.ForwardRefExoticComponent<infer P>
    ? P extends React.RefAttributes<infer TRef>
        ? TRef
        : never
    : never;

/**
 * Extracts the type of a particular prop K of a React component type T.
 */
export type ReactPropType<T, K extends keyof ReactPropsType<T>> = ReactPropsType<T>[K];

/**
 * Extracts the function signature type of a particular prop K of a React component type T.
 */
export type ReactPropFunctionType<T, K extends keyof ReactPropsType<T>> = Extract<
    ReactPropType<T, K>,
    (...params: any[]) => any
>;

/**
 * Builds an enzyme ReactWrapper type from the React component type T.
 * Usage example: type MyComponentWrapper = EnzymeWrapperType<typeof MyComponent>;
 */
export type EnzymeWrapperType<T> = ReactWrapper<ReactPropsType<T>, ReactStateType<T> | any>;
