# Linting and Formatting Guide for FinanceFunds

This guide explains how to use the linting and formatting tools set up in the FinanceFunds project.

## ESLint Configuration

The project uses ESLint's new [flat configuration format](https://eslint.org/docs/latest/use/configure/configuration-files-new) in `eslint.config.mjs`, which offers better performance and more flexibility.

## Available Scripts

The following npm scripts are available for linting and formatting:

- `npm run lint`: Runs ESLint to check for code issues
- `npm run lint:fix`: Runs ESLint and automatically fixes issues where possible
- `npm run format`: Runs Prettier to format all code files

## Git Hooks

We've set up automatic linting and formatting on pre-commit using Husky and lint-staged. This means that whenever you commit your code, the following will happen automatically:

1. ESLint will run on your staged JavaScript/TypeScript files and fix issues if possible
2. Prettier will format your staged code files

If there are any linting errors that can't be automatically fixed, the commit will be blocked until you resolve them.

## VS Code Integration

For the best development experience, we recommend installing the ESLint and Prettier extensions for VS Code. With these extensions, you'll get real-time linting feedback as you write code, and you can format files on save.

The project includes VS Code settings that enable the following features:
- Format on save
- ESLint validation
- TypeScript validation

## Linting Rules

The project follows these linting rules:
- TypeScript best practices with `@typescript-eslint/recommended`
- React best practices with `react/recommended` and `react-hooks/recommended`
- Consistent code style with Prettier

### Common Rules

- No unused variables
- Prefer `const` over `let` when variables aren't reassigned
- No console.log statements (only console.warn and console.error are allowed)
- No explicit `any` types (warnings will show)
- Component files must only export components to work with Fast Refresh

## How to Fix Common Issues

### Unused Imports

If you see warnings about unused imports:
```
'IconInfoCircle' is defined but never used
```

Either remove the unused import or use it in your component.

### Type Annotations 

The project currently has `@typescript-eslint/no-explicit-any` rule disabled to allow for easier development, but it's considered good practice to properly type your variables instead of using `any`. Future updates will gradually introduce proper typing.

### Hook Rules

React Hooks must be called at the top level of your component, not inside conditions:
```
// INCORRECT
if (condition) {
  const { colorScheme } = useMantineColorScheme();
}

// CORRECT
const { colorScheme } = useMantineColorScheme();
if (condition) {
  // use colorScheme here
}
```

Also, make sure to include all dependencies in the dependency array of useEffect:

```
// INCORRECT
useEffect(() => {
  fetchUserData(userId);
}, []); // Missing userId dependency

// CORRECT
useEffect(() => {
  fetchUserData(userId);
}, [userId]); // Properly includes the userId dependency
```

### Any Types

Avoid using `any` types in TypeScript. Instead, define proper interfaces or types:

```typescript
// INCORRECT
const handleData = (data: any) => { ... }

// CORRECT
interface UserData {
  name: string;
  email: string;
}
const handleData = (data: UserData) => { ... }
```

### React Fast Refresh Rules

To ensure Fast Refresh works properly (allowing component updates without losing state):

1. Files should only export React components, custom hooks, or non-React code
2. If you need to export both components and non-component values from a file:
   - Move constants, types, and utility functions to a separate file
   - Or use `allowConstantExport` for simple constants

```typescript
// INCORRECT - Mixing component and function exports
export function MyComponent() { ... }
export function helperFunction() { ... } 

// CORRECT - Separate files
// MyComponent.tsx
export function MyComponent() { ... }

// helpers.ts
export function helperFunction() { ... }
```

## Manual Linting and Formatting

You can run linting and formatting manually with these commands:

```bash
# Check for linting issues
npm run lint

# Fix linting issues
npm run lint:fix

# Format code
npm run format

# Run all checks (format, lint, TypeScript type checking)
npm run check
```

The `check` command runs a PowerShell script that performs all code quality checks in sequence, making it easy to ensure your code meets all quality standards before committing.
