# Form Validation and Error Handling Documentation

## Overview

This document describes the comprehensive form validation and error handling system implemented in the Job Application Tracker. The system ensures data integrity, provides excellent user feedback, and follows accessibility best practices.

## Table of Contents

1. [Architecture](#architecture)
2. [Validation Schemas](#validation-schemas)
3. [Form Components](#form-components)
4. [Error Handling](#error-handling)
5. [Accessibility](#accessibility)
6. [Usage Examples](#usage-examples)
7. [Best Practices](#best-practices)

## Architecture

### Technology Stack

- **React Hook Form** (v7.63.0): Form state management and validation
- **Zod** (v3.x): Schema validation and type inference
- **@hookform/resolvers**: Integration between React Hook Form and Zod
- **React Query** (TanStack Query): API state management and mutations
- **Sonner**: Toast notifications for user feedback

### Key Components

```
frontend/src/
├── lib/
│   ├── validation.ts        # All Zod schemas
│   ├── mutations.ts          # React Query mutations
│   └── api.ts                # API client with error interceptors
├── components/
│   ├── forms/                # Form components
│   │   ├── application-form.tsx
│   │   ├── interview-form.tsx
│   │   ├── note-form.tsx
│   │   ├── contact-form.tsx
│   │   └── settings-form.tsx
│   ├── ui/                   # Reusable UI components
│   │   └── form.tsx          # Form context components
│   └── error-boundary.tsx    # Error boundary for forms
```

## Validation Schemas

All validation schemas are defined in `lib/validation.ts` using Zod.

### Application Schema

```typescript
export const applicationSchema = z.object({
  companyName: z.string()
    .min(1, 'Company name is required')
    .max(100, 'Company name must be less than 100 characters'),
  position: z.string()
    .min(1, 'Position is required')
    .max(100, 'Position must be less than 100 characters'),
  // ... other fields
}).refine(
  (data) => {
    if (data.salaryMin && data.salaryMax) {
      return data.salaryMin <= data.salaryMax;
    }
    return true;
  },
  {
    message: 'Minimum salary must be less than or equal to maximum salary',
    path: ['salaryMin'],
  }
);
```

**Features:**
- Required field validation
- String length constraints
- Custom cross-field validation (salary min/max)
- TypeScript type inference

### Interview Schema

```typescript
export const interviewSchema = z.object({
  applicationId: z.number().positive('Application ID is required'),
  interviewDate: z.string().min(1, 'Interview date is required'),
  interviewType: z.enum(['Phone', 'Video', 'Onsite', 'Technical', 'HR', 'Final']),
  duration: z.number()
    .positive('Duration must be positive')
    .max(480, 'Duration must be less than 8 hours')
    .optional(),
  // ... other fields
});
```

**Features:**
- Enum validation for interview types
- Numeric constraints (duration limits)
- Optional URL fields with empty string support
- Date/time validation

### Note Schema

```typescript
export const noteSchema = z.object({
  applicationId: z.number().positive('Application ID is required'),
  title: z.string()
    .min(1, 'Title is required')
    .max(200, 'Title must be less than 200 characters'),
  content: z.string()
    .min(1, 'Content is required')
    .max(10000, 'Content must be less than 10000 characters'),
  noteType: z.enum(['Research', 'Interview', 'Follow-up', 'General', 'Offer']),
});
```

**Features:**
- Rich text content validation
- Category-based organization
- Length limits for performance

### Contact Schema

```typescript
export const contactSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  email: z.string()
    .email('Must be a valid email')
    .optional()
    .or(z.literal('')),
  phone: z.string().max(20, 'Phone must be less than 20 characters').optional(),
  linkedin: z.string()
    .url('Must be a valid URL')
    .optional()
    .or(z.literal('')),
  // ... other fields
});
```

**Features:**
- Email validation
- URL validation (LinkedIn)
- Optional fields with empty string support
- Boolean flags (primary contact)

### Settings Schema

```typescript
export const settingsSchema = z.object({
  enableReminders: z.boolean(),
  showSalaryFields: z.boolean(),
  reminderDaysBefore: z.number()
    .int('Must be a whole number')
    .min(0, 'Must be 0 or greater')
    .max(30, 'Must be 30 days or less'),
  defaultApplicationStatus: z.enum(['Applied', 'Interview', 'Offer', 'Rejected']),
  theme: z.enum(['light', 'dark', 'system']),
});
```

**Features:**
- Boolean toggles for preferences
- Integer validation for numeric settings
- Enum-based dropdowns
- User preference persistence

### Search Filters Schema

```typescript
export const searchFiltersSchema = z.object({
  search: z.string(),
  status: z.array(z.enum(['Applied', 'Interview', 'Offer', 'Rejected'])),
  priority: z.array(z.enum(['High', 'Medium', 'Low'])),
  dateRange: z.object({
    from: z.string().optional(),
    to: z.string().optional(),
  }),
  salaryRange: z.object({
    min: z.number().positive().optional(),
    max: z.number().positive().optional(),
  }),
  location: z.array(z.string()),
  hasInterviews: z.boolean().optional(),
}).refine(
  (data) => {
    if (data.salaryRange.min && data.salaryRange.max) {
      return data.salaryRange.min <= data.salaryRange.max;
    }
    return true;
  },
  {
    message: 'Minimum salary must be less than or equal to maximum salary',
    path: ['salaryRange', 'min'],
  }
).refine(
  (data) => {
    if (data.dateRange.from && data.dateRange.to) {
      return new Date(data.dateRange.from) <= new Date(data.dateRange.to);
    }
    return true;
  },
  {
    message: 'Start date must be before or equal to end date',
    path: ['dateRange', 'from'],
  }
);
```

**Features:**
- Multiple filter types
- Range validation (salary, dates)
- Array-based multi-select
- Cross-field date validation

## Form Components

### Basic Form Structure

All forms follow this pattern:

```typescript
export function MyForm({ data, onSubmit, onCancel, isLoading }: MyFormProps) {
  const form = useForm<MyFormData>({
    resolver: zodResolver(mySchema),
    defaultValues: data || {
      // default values here
    },
  });

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        {/* Form fields */}
      </form>
    </Form>
  );
}
```

### Form Field Pattern

```typescript
<FormField
  control={form.control}
  name="fieldName"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Field Label *</FormLabel>
      <FormControl>
        <Input 
          placeholder="Enter value" 
          {...field}
          aria-required="true"
          aria-invalid={!!form.formState.errors.fieldName}
        />
      </FormControl>
      <FormDescription>
        Helpful hint about this field
      </FormDescription>
      <FormMessage />
    </FormItem>
  )}
/>
```

### Loading States

All forms support loading states:

```typescript
<Button type="submit" disabled={isLoading}>
  {isLoading ? 'Saving...' : 'Save'}
</Button>
```

## Error Handling

### Global Error Boundary

The application uses a global error boundary in `layout.tsx`:

```typescript
<ErrorBoundary>
  <Navigation />
  <main>{children}</main>
</ErrorBoundary>
```

### Form-Specific Error Boundary

Forms are wrapped with `FormErrorBoundary`:

```typescript
<FormErrorBoundary>
  <ApplicationForm
    onSubmit={handleSubmit}
    onCancel={handleCancel}
    isLoading={mutation.isPending}
  />
</FormErrorBoundary>
```

### API Error Handling

The API client (`lib/api.ts`) includes error interceptors:

```typescript
api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response) {
      // Handle specific HTTP errors
      if (error.response.status === 401) {
        // Redirect to login
      } else if (error.response.status >= 500) {
        console.error('Server Error');
      }
    }
    return Promise.reject(error);
  }
);
```

### Mutation Error Handling

React Query mutations include error handling:

```typescript
export function useCreateApplication() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data) => apiService.createApplication(data),
    onMutate: async (newData) => {
      // Optimistic update
      // Cancel queries and snapshot previous data
    },
    onError: (err, newData, context) => {
      // Rollback optimistic update
      if (context?.previousData) {
        queryClient.setQueryData(key, context.previousData);
      }
      toast.error('Failed to create application');
    },
    onSuccess: () => {
      toast.success('Application created successfully');
    },
    onSettled: () => {
      // Refetch to ensure data is up-to-date
      queryClient.invalidateQueries({ queryKey: ['applications'] });
    },
  });
}
```

### Toast Notifications

The system uses Sonner for toast notifications:

```typescript
import { toast } from 'sonner';

// Success
toast.success('Application created successfully');

// Error
toast.error('Failed to create application');

// Info
toast.info('Processing your request');

// Loading
toast.loading('Saving...');
```

## Accessibility

### ARIA Attributes

All forms include proper ARIA attributes:

```typescript
<Input 
  {...field}
  aria-required="true"
  aria-invalid={!!form.formState.errors.fieldName}
  aria-describedby={`${field.name}-description ${field.name}-error`}
/>
```

### Focus Management

Forms automatically focus on the first error field:

```typescript
useEffect(() => {
  const errors = form.formState.errors;
  if (errors.title) {
    titleInputRef.current?.focus();
  }
}, [form.formState.errors]);
```

### Screen Reader Announcements

Errors are announced to screen readers:

```typescript
// Announce error
const announcement = document.createElement('div');
announcement.setAttribute('role', 'alert');
announcement.setAttribute('aria-live', 'assertive');
announcement.textContent = errorMessage;
announcement.className = 'sr-only';
document.body.appendChild(announcement);
setTimeout(() => document.body.removeChild(announcement), 1000);
```

### Keyboard Navigation

All interactive elements are keyboard accessible:

- Tab navigation through form fields
- Enter to submit forms
- Escape to cancel
- Arrow keys for select/dropdown navigation

### Color and Contrast

Error messages use sufficient color contrast:

```css
/* Error text */
.text-red-500 /* Meets WCAG AA standards */

/* Error border */
.border-red-300 /* Clear visual indication */
```

## Usage Examples

### Creating a New Form

1. **Define the Schema** (`lib/validation.ts`):

```typescript
export const mySchema = z.object({
  field1: z.string().min(1, 'Field 1 is required'),
  field2: z.number().positive(),
});

export type MyFormData = z.infer<typeof mySchema>;
```

2. **Create the Form Component** (`components/forms/my-form.tsx`):

```typescript
export function MyForm({ data, onSubmit, isLoading }: MyFormProps) {
  const form = useForm<MyFormData>({
    resolver: zodResolver(mySchema),
    defaultValues: data || { field1: '', field2: 0 },
  });

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        {/* Fields */}
      </form>
    </Form>
  );
}
```

3. **Create the Mutation** (`lib/mutations.ts`):

```typescript
export function useCreateMyData() {
  return useMutation({
    mutationFn: (data: MyFormData) => apiService.create(data),
    onSuccess: () => toast.success('Created successfully'),
    onError: () => toast.error('Failed to create'),
  });
}
```

4. **Use in a Page**:

```typescript
export default function MyPage() {
  const [isOpen, setIsOpen] = useState(false);
  const mutation = useCreateMyData();

  const handleSubmit = (data: MyFormData) => {
    mutation.mutate(data, {
      onSuccess: () => setIsOpen(false),
    });
  };

  return (
    <FormErrorBoundary>
      <MyForm
        onSubmit={handleSubmit}
        onCancel={() => setIsOpen(false)}
        isLoading={mutation.isPending}
      />
    </FormErrorBoundary>
  );
}
```

### Adding Custom Validation

```typescript
const schema = z.object({
  password: z.string().min(8),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});
```

### Handling File Uploads

```typescript
const schema = z.object({
  file: z.instanceof(File)
    .refine((file) => file.size <= 5000000, 'Max file size is 5MB')
    .refine(
      (file) => ['image/jpeg', 'image/png'].includes(file.type),
      'Only .jpg and .png formats are supported'
    ),
});
```

## Best Practices

### Do's ✅

1. **Always use Zod schemas** for validation
2. **Wrap forms** in `FormErrorBoundary`
3. **Provide loading states** for all async actions
4. **Show success/error notifications** using toast
5. **Include ARIA attributes** for accessibility
6. **Focus on first error field** after validation
7. **Use optimistic updates** with proper rollback
8. **Validate on blur and submit** for better UX
9. **Provide helpful error messages** that guide users
10. **Test with keyboard navigation** and screen readers

### Don'ts ❌

1. **Don't skip validation** on the frontend
2. **Don't show technical error messages** to users
3. **Don't forget loading states** during submissions
4. **Don't ignore accessibility** requirements
5. **Don't validate on every keystroke** (use debouncing if needed)
6. **Don't forget to invalidate queries** after mutations
7. **Don't use inline styles** for error states
8. **Don't skip TypeScript types** for form data
9. **Don't forget to handle edge cases** (empty values, nulls)
10. **Don't leave forms without cancel/reset options**

### Performance Tips

1. Use `mode: 'onBlur'` for validation to reduce re-renders
2. Memoize expensive validation functions
3. Debounce async validations
4. Use React Query's `staleTime` and `cacheTime` appropriately
5. Implement proper loading skeletons

### Security Considerations

1. Always validate on the server-side
2. Sanitize user input before display
3. Use HTTPS for all API calls
4. Implement CSRF protection
5. Rate limit form submissions
6. Validate file uploads on server
7. Prevent XSS attacks in rich text editors

## Testing

### Unit Testing Schemas

```typescript
import { applicationSchema } from './validation';

describe('applicationSchema', () => {
  it('should validate valid data', () => {
    const validData = {
      companyName: 'Test Company',
      position: 'Developer',
      // ... other required fields
    };
    
    expect(() => applicationSchema.parse(validData)).not.toThrow();
  });

  it('should reject invalid data', () => {
    const invalidData = {
      companyName: '', // Empty string
      position: 'Developer',
    };
    
    expect(() => applicationSchema.parse(invalidData)).toThrow();
  });
});
```

### Testing Form Components

```typescript
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { ApplicationForm } from './application-form';

describe('ApplicationForm', () => {
  it('should show validation errors', async () => {
    render(<ApplicationForm onSubmit={jest.fn()} onCancel={jest.fn()} />);
    
    const submitButton = screen.getByRole('button', { name: /submit/i });
    fireEvent.click(submitButton);
    
    await waitFor(() => {
      expect(screen.getByText('Company name is required')).toBeInTheDocument();
    });
  });

  it('should call onSubmit with valid data', async () => {
    const onSubmit = jest.fn();
    render(<ApplicationForm onSubmit={onSubmit} onCancel={jest.fn()} />);
    
    fireEvent.change(screen.getByLabelText(/company name/i), {
      target: { value: 'Test Company' },
    });
    
    fireEvent.click(screen.getByRole('button', { name: /submit/i }));
    
    await waitFor(() => {
      expect(onSubmit).toHaveBeenCalledWith(
        expect.objectContaining({ companyName: 'Test Company' })
      );
    });
  });
});
```

## Troubleshooting

### Common Issues

**Issue**: Form doesn't validate on submit
- **Solution**: Ensure `resolver: zodResolver(schema)` is set in `useForm`

**Issue**: Error messages don't appear
- **Solution**: Check that `FormMessage` component is included in each `FormField`

**Issue**: Loading state doesn't work
- **Solution**: Pass `isLoading` prop from mutation's `isPending` state

**Issue**: Toast notifications don't show
- **Solution**: Ensure `<Toaster />` component is included in root layout

**Issue**: Accessibility warnings
- **Solution**: Add proper ARIA attributes and ensure keyboard navigation works

## Conclusion

This form validation and error handling system provides a robust, user-friendly, and accessible experience for the Job Application Tracker. By following the patterns and best practices outlined in this document, you can ensure data integrity and provide excellent user feedback throughout the application.

For questions or contributions, please refer to the project's CONTRIBUTING.md file.
