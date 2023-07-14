# Components Styling

-   [General](#general)
-   [BEM Methodology for CSS](#bem-methodology-for-css)
-   [General CSS Guidelines](#general-css-guidelines)
    -   [Resources](#resources)
-   [SASS](#sass)
-   [Global Variables / Mixins / Functions](#global-variables--mixins--functions)
-   [Stylesheets for 3rd Party Components](#stylesheets-for-3rd-party-components)
-   [Stylesheets for Our Components](#stylesheets-for-our-components)
    -   [Files / Imports](#files--imports)
-   [CSS Class Naming / BEM](#css-class-naming--bem)
-   [Browser/Vendor-specific CSS Property Prefixes](#browservendor-specific-css-property-prefixes)

## General

-   Check [InvoiceFieldComponent.scss](../src/components/InvoiceDataCaptureToolkitComponent/InvoiceFieldsComponent/InvoiceFieldComponent/InvoiceFieldComponent.scss), [DocumentLayoutItemComponent.scss](../src/components/InvoiceDataCaptureToolkitComponent/DocumentPageViewComponent/DocumentLayoutItemComponent.scss) or [FieldAssignmentMenuComponent.scss](../src/components/InvoiceDataCaptureToolkitComponent/FieldAssignmentMenuComponent/FieldAssignmentMenuComponent.scss) as styling examples.
-   Try to use `theme.scss` variables and `bootstrap` mixins and atomic classes whenever possible.
-   Organize CSS styles into variables and maps.
-   Create custom mixins for complex styling composition between several selectors or classes.

## BEM Methodology for CSS

BEM (Block, Element, Modifier) is a component-based approach to CSS styling. The idea behind it is to divide the user interface into independently styled blocks. The official [BEM](https://en.bem.info/methodology/quick-start/) website has a good introduction to the concepts.

NOTE: For this project, the [modified naming convention](http://getbem.com/naming/) is being used.

[Example of BEM in SASS](https://codepen.io/anon/pen/qoJqwz), and for reference: [The SASS Ampersand](https://css-tricks.com/the-sass-ampersand/).

## General CSS Guidelines

### Resources

-   [Flexbox Guide](https://css-tricks.com/snippets/css/a-guide-to-flexbox/)
-   [Flexbox Bugs/Workarounds](https://github.com/philipwalton/flexbugs)

## SASS

We use SASS (.scss) in our project: https://sass-lang.com/guide

## Global Variables / Mixins / Functions

We have files in `src/styles` named:

-   "globals.scss" file define "global" SASS styles, mixins and functions.
-   "theme.scss" file contains Bootstrap and custom theme variables.

Include these files where they are needed.

## Stylesheets for 3rd Party Components

To override/augment styles for a 3rd party package, create a SCSS file (appropriately named to match the package name) in our `src/styles` directory and import that file into `globals.scss` after the import for the 3rd party styles. (See Bootstrap theme overriding).

## Stylesheets for Our Components

### Files / Imports

Each of our components is responsible for importing its own styles as needed. The general pattern is to create a SCCS file alongside and named identically to (except for extension) the source file that it provides styling to.

The top-level DOM element should have a CSS class name identical to the name of the component itself. Then all styles for that component should be nested in a selector for that component's class name.

For example, a component named "MyComponent":

-   Defined in a file named "MyComponent.tsx"
-   Its styles are defined in "MyComponent.scss".
-   "MyComponent.tsx" would contain an import: `import "./MyComponent.scss";` (our Webpack configuration takes care of processing this import to build a combined stylesheet for the entire web app).
-   "MyComponent.scss" would have all its selectors nested within a `.MyComponent {}` selector.

## CSS Class Naming / BEM

We use a variation of BEM: http://getbem.com/naming/

-   Generally prefer all-lowercase names with a single dash (-) to separate words of a multi-word block/element/modifier name.

    -   Exception: When the styling a component, the block name is the name of the component, and should be written exactly the same as the component name (e.g., "MyComponent", rather than "my-component").
    -   Exception: element/modifier names that correlate to string enum values should general exactly match the string enum values themselves. This is convenient for code that dynamically generates CSS class names from those enum values.

-   Use double underscore (\_\_) before an element name.
-   Use double dash (--) before a modifier name
    -   This is different than some versions of BEM that use a single underscore before a modifier name.
    -   We decided that a double dash is more instantly recognizable as a separator, while a single underscore takes additional brainpower to not interpret it as simply a space between words.
-   When styling a component, only the component itself (outermost DOM element) is considered to be a "block".
    -   All nested DOM elements within the component are considered "elements".
    -   If you feel like a DOM element rendered by a component really should be its own "block", then this may be a good sign that you should split that portion of the component out to its own component.
-   Name CSS classes based on the concepts they are used to represent (unread, error, clickable) rather than the styles they apply (bold, red, pointer).

## Browser/Vendor-specific CSS Property Prefixes

Don't worry about them! Just use the standard CSS property name. Our webpack configuration (autoprefixer) will take care of expanding them to make use of vendor
prefixes for properties that need them.
