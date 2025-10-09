module.exports = {
    root: true,
    parser: "@typescript-eslint/parser",
    parserOptions: {
        project: "./tsconfig.json",
        sourceType: "module",
    },
    plugins: ["@typescript-eslint"],
    extends: [
        "eslint:recommended",
        "plugin:@typescript-eslint/recommended",
        "plugin:react/recommended",
    ],
    rules: {
        // Disallow raw console methods except warn/error
        "no-console": ["error", { allow: ["warn", "error"] }],
        // Allow any other project-level overrides here
    },
    settings: {
        react: {
            version: "detect",
        },
    },
};
/**
 * This is intended to be a basic starting point for linting in your app.
 * It relies on recommended configs out of the box for simplicity, but you can
 * and should modify this configuration to best suit your team's needs.
 */

/** @type {import('eslint').Linter.Config} */
module.exports = {
    root: true,
    parserOptions: {
        ecmaVersion: "latest",
        sourceType: "module",
        ecmaFeatures: {
            jsx: true,
        },
    },
    env: {
        browser: true,
        commonjs: true,
        es6: true,
    },
    ignorePatterns: ["!**/.server", "!**/.client"],

    // Base config
    extends: ["eslint:recommended"],

    overrides: [
        // React
        {
            files: ["**/*.{js,jsx,ts,tsx}"],
            plugins: ["react", "jsx-a11y"],
            extends: [
                "plugin:react/recommended",
                "plugin:react/jsx-runtime",
                "plugin:react-hooks/recommended",
                "plugin:jsx-a11y/recommended",
            ],
            settings: {
                react: {
                    version: "detect",
                },
                formComponents: ["Form"],
                linkComponents: [
                    { name: "Link", linkAttribute: "to" },
                    { name: "NavLink", linkAttribute: "to" },
                ],
                "import/resolver": {
                    typescript: {},
                },
            },
        },

        // Typescript
        {
            files: ["**/*.{ts,tsx}"],
            plugins: ["@typescript-eslint", "import"],
            parser: "@typescript-eslint/parser",
            settings: {
                "import/internal-regex": "^~/",
                "import/resolver": {
                    node: {
                        extensions: [".ts", ".tsx"],
                    },
                    typescript: {
                        alwaysTryTypes: true,
                    },
                },
            },
            extends: [
                "plugin:@typescript-eslint/recommended",
                "plugin:import/recommended",
                "plugin:import/typescript",
            ],
        },

        // Node
        {
            files: [".eslintrc.cjs"],
            env: {
                node: true,
            },
        },
        // Prevent accidental logging of sensitive variables via console.log
        {
            files: ["**/*.{ts,tsx,js,jsx}"],
            excludedFiles: ["**/build/**"],
            rules: {
                "no-restricted-syntax": [
                    "error",
                    {
                        selector:
                            "CallExpression[callee.object.name='console'][callee.property.name='log'] > Identifier[name='accessToken']",
                        message:
                            "Avoid logging access tokens (accessToken) via console.log",
                    },
                    {
                        selector:
                            "CallExpression[callee.object.name='console'][callee.property.name='log'] > Identifier[name='refreshToken']",
                        message:
                            "Avoid logging refresh tokens (refreshToken) via console.log",
                    },
                    {
                        selector:
                            "CallExpression[callee.object.name='console'][callee.property.name='log'] > Identifier[name='idToken']",
                        message:
                            "Avoid logging id tokens (idToken) via console.log",
                    },
                    {
                        selector:
                            "CallExpression[callee.object.name='console'][callee.property.name='log'] > Identifier[name='token']",
                        message:
                            "Avoid logging token variables via console.log",
                    },
                    {
                        selector:
                            "CallExpression[callee.object.name='console'][callee.property.name='log'] > Identifier[name='secret']",
                        message: "Avoid logging secrets via console.log",
                    },
                    {
                        selector:
                            "CallExpression[callee.object.name='console'][callee.property.name='log'] > Identifier[name='password']",
                        message: "Avoid logging passwords via console.log",
                    },
                    {
                        selector:
                            "CallExpression[callee.object.name='console'][callee.property.name='log'] > Identifier[name='clientSecret']",
                        message: "Avoid logging client secrets via console.log",
                    },
                ],
            },
        },
    ],
};
