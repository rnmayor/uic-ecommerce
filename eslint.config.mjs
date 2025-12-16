import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

import { defineConfig, globalIgnores } from 'eslint/config';
import nextCoreWebVitals from 'eslint-config-next/core-web-vitals';
import nextTypescript from 'eslint-config-next/typescript';
import prettier from 'eslint-config-prettier';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Collect all tsconfig.json files acrross apps and packages
const tsconfigPaths = [];
['apps', 'packages'].forEach((dir) => {
  const baseDir = path.join(__dirname, dir);
  if (fs.existsSync(baseDir)) {
    for (const folder of fs.readdirSync(baseDir)) {
      const tsconfigPath = path.join(baseDir, folder, 'tsconfig.json');
      if (fs.existsSync(tsconfigPath)) {
        tsconfigPaths.push(tsconfigPath);
      }
    }
  }
});

export default defineConfig([
  globalIgnores([
    '**/node_modules/**',
    '**/.next/**',
    '**/dist/**',
    '**/build/**',
    '**/out/**',
    '**/coverage/**',

    '**/.turbo/**',
    '**/.cache/**',
    '**/.eslintcache',
    '**/*.tsbuildinfo',
    '**/.vercel/**',
    '**/.env',
    '**/.env.*',
    '**/*.log',
    '**/next-env.d.ts',

    'packages/db/src/generated/**', // prisma
  ]),

  ...nextCoreWebVitals,
  ...nextTypescript,
  {
    settings: {
      react: {
        version: 'detect',
      },
      next: {
        rootDir: ['apps/*/'],
      },
      // explicitly added to handle monorepo path aliases for Typescript accross apps/packages
      'import/resolver': {
        typescript: {
          project: tsconfigPaths,
        },
      },
    },
    rules: {
      'no-unused-vars': 'off',
      '@typescript-eslint/no-unused-vars': [
        'warn',
        {
          argsIgnorePattern: '^_',
          varsIgnorePattern: '^_',
        },
      ],
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/consistent-type-imports': ['error', { prefer: 'type-imports' }],
      'no-console': ['warn', { allow: ['warn', 'error'] }],
      eqeqeq: ['error', 'always'],
      curly: ['error', 'all'],
      'no-throw-literal': 'error',

      'next/next/no-html-link-for-pages': 'off', // for next pages

      // eslint-plugin-import - dependency of eslint-config-next
      'import/order': [
        'error',
        {
          groups: [
            'builtin',
            'external',
            ['internal', 'parent', 'sibling'],
            'type',
            ['index', 'object'],
          ],
          pathGroups: [{ pattern: '@ecommerce/**', group: 'internal' }],
          alphabetize: { order: 'asc', caseInsensitive: true },
          sortTypesGroup: true,
          'newlines-between': 'always',
        },
      ],
    },
  },

  // prettier last in config to avoid conflict versus eslint setup
  prettier,
]);
