module.exports = {
  root: true,
  env: {
    browser: true,
    es2021: true,
  },
  extends: [
    'eslint:recommended',
    'plugin:react/recommended',
    'plugin:jsx-a11y/recommended',
    'prettier',
  ],
  parserOptions: {
    ecmaVersion: 2021,
    sourceType: 'module',
    ecmaFeatures: {
      jsx: true,
    },
  },
  plugins: ['react'],
  rules: {
    'no-console': ['error', { allow: ['warn', 'error'] }],
  },
  settings: {
    react: {
      version: 'detect',
    },
  },
};
module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/recommended',
    'plugin:react-hooks/recommended',
    'plugin:react/recommended',
    'plugin:react/jsx-runtime',
    'prettier',
  ],
  ignorePatterns: [
    'dist',
    '.eslintrc.cjs',
    'node_modules',
    'build',
    'public',
    'vite.config.ts',
    '*.d.ts',
  ],
  parser: '@typescript-eslint/parser',
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module',
    ecmaFeatures: {
      jsx: true,
    },
  },
  plugins: ['react-refresh', 'react', '@typescript-eslint', 'prettier'],
  settings: {
    react: {
      version: 'detect',
    },
  },
  rules: {
    'react-refresh/only-export-components': [
      'warn', // Making this a warning instead of error since we're still reorganizing code
      { allowConstantExport: true },
    ],
    'react/prop-types': 0,
    'react/react-in-jsx-scope': 0,
    'no-unused-vars': 'off',
    '@typescript-eslint/no-unused-vars': ['error'],
    'prettier/prettier': 'error',
    '@typescript-eslint/no-explicit-any': 'off', // TODO: Properly type these variables and re-enable this rule (either as warning or error)
    'prefer-const': 'error',
    'no-console': ['error', { allow: ['warn', 'error'] }],
  },
};

// Allow console only inside the SafeLogger implementation
module.exports = Object.assign(module.exports || {}, {
  overrides: [
    {
      files: ['src/utils/SafeLogger.ts'],
      rules: {
        'no-console': 'off',
      },
    },
  ],
});
