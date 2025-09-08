// Minimal ESLint flat config shim to allow running ESLint v9 in this project.
// This file intentionally keeps settings minimal to avoid enabling type-aware rules
// which can require a specific tsconfig/project setup.
module.exports = [
  {
    files: ["**/*.{ts,tsx,js,jsx}"],
    ignores: ["node_modules/**", "dist/**", "build/**"],
    languageOptions: {
      parser: require.resolve("@typescript-eslint/parser"),
      parserOptions: {
        ecmaVersion: 2020,
        sourceType: "module",
        ecmaFeatures: { jsx: true },
      },
    },
    plugins: {
      "@typescript-eslint": require("@typescript-eslint/eslint-plugin"),
      react: require("eslint-plugin-react"),
    },
    rules: {
      // Enforce no raw console usage in application code. Logger files are expected
      // to have their own eslint-disable comment if they intentionally use console.
      "no-console": "error",
    },
  },
];
