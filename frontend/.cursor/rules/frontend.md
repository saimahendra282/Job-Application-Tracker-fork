# Frontend Development Rules

## Technology Stack

-   **Framework**: Next.js 14+ with App Router
-   **Language**: TypeScript (strict mode)
-   **Styling**: Tailwind CSS with shadcn/ui components
-   **State Management**: React Query (TanStack Query) for server state
-   **Forms**: React Hook Form with Zod validation
-   **Icons**: Lucide React
-   **Notifications**: Sonner (toast notifications)

## Code Style & Standards

### TypeScript

-   Use strict TypeScript configuration
-   Define proper interfaces and types for all data structures
-   Use type assertions sparingly and with proper type guards
-   Prefer `interface` over `type` for object shapes
-   Use generic types for reusable components
-   Implement proper error handling with typed error objects

### React Best Practices

-   Use functional components with hooks
-   Implement proper component composition
-   Use React.memo for performance optimization when needed
-   Follow the single responsibility principle for components
-   Use custom hooks for reusable logic
-   Implement proper error boundaries

### Next.js Specific

-   Use App Router patterns and conventions
-   Implement proper loading and error states
-   Use Server Components where appropriate
-   Implement proper metadata for SEO
-   Use Next.js Image component for optimized images
-   Follow Next.js file naming conventions

### Styling Guidelines

-   Use Tailwind CSS utility classes
-   Follow mobile-first responsive design
-   Use shadcn/ui components as the base
-   Create custom components that extend shadcn/ui
-   Maintain consistent spacing and typography
-   Use CSS variables for theme customization

### Component Structure

```
components/
├── ui/           # shadcn/ui base components
├── forms/        # Form-specific components
├── layout/       # Layout components
├── features/     # Feature-specific components
└── common/       # Shared components
```

### File Naming

-   Use kebab-case for file names
-   Use PascalCase for component names
-   Use camelCase for utility functions
-   Use UPPER_CASE for constants

### Import Organization

1. React and Next.js imports
2. Third-party library imports
3. Internal imports (components, utils, types)
4. Relative imports

### State Management

-   Use React Query for server state
-   Use React state for local component state
-   Use URL state for shareable state
-   Implement proper loading and error states
-   Use optimistic updates where appropriate

### API Integration

-   Use the centralized API client
-   Implement proper error handling
-   Use React Query for caching and synchronization
-   Implement proper loading states
-   Handle network errors gracefully

### Performance Optimization

-   Use dynamic imports for code splitting
-   Implement proper image optimization
-   Use React.memo for expensive components
-   Optimize bundle size with proper imports
-   Use Next.js built-in optimizations

### Accessibility

-   Use semantic HTML elements
-   Implement proper ARIA attributes
-   Ensure keyboard navigation
-   Maintain proper color contrast
-   Test with screen readers

### Testing

-   Write unit tests for utility functions
-   Test component behavior with React Testing Library
-   Test API integration with MSW
-   Maintain test coverage above 80%
-   Use proper test data and mocks

### Security

-   Sanitize user inputs
-   Implement proper CSRF protection
-   Use secure authentication patterns
-   Validate data on both client and server
-   Follow OWASP frontend security guidelines

### Development Workflow

-   Use ESLint and Prettier for code formatting
-   Run type checking before commits
-   Use proper Git commit messages
-   Create feature branches for new development
-   Test components in Storybook when applicable

### Error Handling

-   Implement proper error boundaries
-   Use consistent error message patterns
-   Log errors appropriately
-   Provide user-friendly error messages
-   Handle network errors gracefully

### Performance Monitoring

-   Monitor Core Web Vitals
-   Use Next.js analytics
-   Implement proper error tracking
-   Monitor bundle size
-   Optimize for mobile performance
