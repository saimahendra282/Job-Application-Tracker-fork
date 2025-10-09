import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/__tests__/setup.ts'],
    css: true,
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      exclude: [
        'node_modules/',
        'src/__tests__/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
        'src/lib/utils.ts', // Simple utility functions
        'src/lib/api.ts', // Axios configuration (hard to test in jsdom)
        'src/lib/providers.tsx', // React Query provider wrapper
        'src/lib/types.ts', // TypeScript type definitions
        '.next/**',
        'dist/**',
        'build/**',
        'coverage/**',
        'src/app/**/page.tsx', // Exclude Next.js pages (mostly layout/data fetching)
        'src/app/**/layout.tsx', // Exclude layouts
      ],
      include: ['src/**/*.{ts,tsx}'],
      thresholds: {
        lines: 70,
        functions: 70,
        branches: 70,
        statements: 70,
      },
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
})
