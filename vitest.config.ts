import path from 'path';

import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    coverage: {
      provider: 'v8',
      reporter: ['text', 'html'],
    },
    projects: [
      {
        test: {
          name: 'uic-ecommerce',
          root: './uic-ecommerce',
          globals: true,
          environment: 'jsdom',
          setupFiles: [path.resolve(__dirname, './vitest.setup.ts')],
          include: ['features/**/*.test.ts', 'app/**/*.tests.ts'],
          alias: {
            features: path.resolve(__dirname, './uic-ecommerce/features'),
          },
        },
      },
    ],
  },
});
