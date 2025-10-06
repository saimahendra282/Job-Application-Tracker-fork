# Testing Guide for Form Validation

This document provides guidance on testing the form validation system. While the project doesn't currently have a test runner configured, this guide shows how to set one up and provides test examples.

## Setting Up Testing (Optional)

To add testing capabilities, you can install one of these test frameworks:

### Option 1: Vitest (Recommended for Vite/Next.js)

```bash
pnpm add -D vitest @testing-library/react @testing-library/jest-dom @testing-library/user-event jsdom
```

Create `vitest.config.ts`:

```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/__tests__/setup.ts'],
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
});
```

### Option 2: Jest

```bash
pnpm add -D jest @testing-library/react @testing-library/jest-dom @testing-library/user-event jest-environment-jsdom
```

## Manual Testing Checklist

Until automated tests are set up, use this manual testing checklist:

### Application Form
- [ ] Submit empty form - should show "Company name is required" error
- [ ] Enter company name > 100 characters - should show length error
- [ ] Set salary min > salary max - should show validation error
- [ ] Enter invalid URL - should show "Must be a valid URL" error
- [ ] Submit valid form - should show success toast
- [ ] Click cancel - should close form without submitting

### Interview Form
- [ ] Submit without date - should show "Interview date is required"
- [ ] Set duration > 480 minutes - should show duration error
- [ ] Enter invalid meeting link - should show URL error
- [ ] Select each interview type - all should work
- [ ] Submit valid form - should create interview

### Note Form
- [ ] Submit empty title - should show error and focus title field
- [ ] Submit empty content - should show error and focus editor
- [ ] Enter title > 200 characters - should show length error
- [ ] Use formatting buttons - should apply formatting
- [ ] Submit valid note - should save successfully

### Contact Form
- [ ] Submit empty name - should show "Name is required"
- [ ] Enter invalid email - should show email error
- [ ] Enter invalid LinkedIn URL - should show URL error
- [ ] Toggle primary contact - should update checkbox
- [ ] Submit valid form - should create contact

### Settings Form
- [ ] Toggle each setting - should update value
- [ ] Set reminder days < 0 - should show validation error
- [ ] Set reminder days > 30 - should show validation error
- [ ] Click reset - should restore default values
- [ ] Save settings - should show success toast

### Filter Sidebar
- [ ] Set min salary > max salary - should show error message
- [ ] Set start date > end date - should show error message
- [ ] Add location with Enter key - should add to list
- [ ] Remove location - should remove from list
- [ ] Clear all filters - should reset everything

### Error Handling
- [ ] Submit form while offline - should show error toast
- [ ] Submit form with server error - should show error toast
- [ ] Cancel form submission - should reset form
- [ ] Multiple rapid submissions - should prevent duplicate requests

### Accessibility
- [ ] Tab through all forms - should follow logical order
- [ ] Submit with keyboard (Enter) - should work
- [ ] Cancel with Escape - should work
- [ ] Screen reader announces errors - test with NVDA/JAWS
- [ ] Error messages have sufficient contrast
- [ ] Form labels are associated with inputs

## Example Test Cases

Here are example test cases for when you set up a test runner:

### Testing Zod Schemas

```typescript
import { describe, it, expect } from 'vitest';
import { applicationSchema } from '@/lib/validation';

describe('applicationSchema', () => {
  it('validates correct data', () => {
    const validData = {
      companyName: 'Tech Corp',
      position: 'Developer',
      status: 'Applied',
      priority: 'High',
      applicationDate: '2025-01-15',
    };
    
    expect(() => applicationSchema.parse(validData)).not.toThrow();
  });

  it('rejects empty company name', () => {
    const invalidData = {
      companyName: '',
      position: 'Developer',
      status: 'Applied',
      priority: 'High',
      applicationDate: '2025-01-15',
    };
    
    expect(() => applicationSchema.parse(invalidData)).toThrow();
  });
});
```

### Testing Form Components

```typescript
import { render, screen, fireEvent } from '@testing-library/react';
import { ApplicationForm } from '@/components/forms/application-form';

describe('ApplicationForm', () => {
  it('shows validation errors on submit', async () => {
    render(
      <ApplicationForm 
        onSubmit={vi.fn()} 
        onCancel={vi.fn()} 
      />
    );
    
    const submitButton = screen.getByRole('button', { name: /create/i });
    fireEvent.click(submitButton);
    
    expect(await screen.findByText('Company name is required')).toBeInTheDocument();
  });

  it('calls onSubmit with valid data', async () => {
    const onSubmit = vi.fn();
    render(
      <ApplicationForm 
        onSubmit={onSubmit} 
        onCancel={vi.fn()} 
      />
    );
    
    // Fill form
    fireEvent.change(screen.getByLabelText(/company name/i), {
      target: { value: 'Tech Corp' },
    });
    fireEvent.change(screen.getByLabelText(/position/i), {
      target: { value: 'Developer' },
    });
    
    // Submit
    fireEvent.click(screen.getByRole('button', { name: /create/i }));
    
    // Wait for submission
    await waitFor(() => {
      expect(onSubmit).toHaveBeenCalledWith(
        expect.objectContaining({
          companyName: 'Tech Corp',
          position: 'Developer',
        })
      );
    });
  });
});
```

### Testing Mutations

```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useCreateApplication } from '@/lib/mutations';

describe('useCreateApplication', () => {
  it('creates application and shows toast', async () => {
    const queryClient = new QueryClient();
    const wrapper = ({ children }) => (
      <QueryClientProvider client={queryClient}>
        {children}
      </QueryClientProvider>
    );

    const { result } = renderHook(() => useCreateApplication(), { wrapper });

    const applicationData = {
      companyName: 'Tech Corp',
      position: 'Developer',
      status: 'Applied',
      priority: 'High',
      applicationDate: '2025-01-15',
    };

    result.current.mutate(applicationData);

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
  });
});
```

## Browser DevTools Testing

### Testing Validation

1. Open browser DevTools (F12)
2. Go to Console tab
3. Try to submit forms with invalid data
4. Check for validation errors in the UI
5. Verify error messages are user-friendly

### Testing Network Requests

1. Open Network tab in DevTools
2. Submit a form
3. Verify request payload is correct
4. Check response status codes
5. Verify error handling for failed requests

### Testing Accessibility

1. Install axe DevTools extension
2. Run accessibility audit on each form
3. Fix any issues found
4. Test with keyboard navigation
5. Test with screen reader (NVDA on Windows, VoiceOver on Mac)

## Performance Testing

### Form Rendering

1. Open Performance tab in DevTools
2. Start recording
3. Open a form
4. Stop recording
5. Check for slow renders or memory leaks

### Validation Performance

1. Open Performance monitor
2. Type rapidly in a validated field
3. Monitor CPU and memory usage
4. Ensure no performance degradation

## Integration Testing

Test the complete flow:

1. **Create Application Flow**
   - Navigate to applications page
   - Click "Add Application"
   - Fill form with valid data
   - Submit
   - Verify application appears in list
   - Verify toast notification shows

2. **Edit Application Flow**
   - Click on an application
   - Click edit
   - Modify data
   - Submit
   - Verify changes are saved
   - Verify toast notification shows

3. **Error Recovery Flow**
   - Submit invalid data
   - See error messages
   - Correct errors
   - Successfully submit
   - Verify success

## Testing Error Boundaries

1. **Component Error**
   - Cause a component to throw an error
   - Verify error boundary catches it
   - Verify fallback UI is shown
   - Verify error is logged

2. **Form Error**
   - Cause a form validation error
   - Verify FormErrorBoundary catches it
   - Verify error message is displayed
   - Verify user can retry

## Regression Testing

After making changes, verify:

- [ ] All forms still submit correctly
- [ ] All validation rules still work
- [ ] Error messages are still displayed
- [ ] Toast notifications still show
- [ ] Loading states still work
- [ ] Accessibility is not broken
- [ ] Performance is not degraded

## Known Issues and Limitations

Document any known issues here:

- ⚠️ Rich text editor in NoteForm may have issues with some formatting combinations
- ⚠️ Date validation doesn't account for timezones
- ⚠️ File uploads are not yet implemented

## Future Testing Improvements

- Set up automated E2E testing with Playwright or Cypress
- Add visual regression testing
- Implement snapshot testing for forms
- Add performance benchmarks
- Set up continuous integration testing

## Resources

- [Testing Library Docs](https://testing-library.com/docs/react-testing-library/intro/)
- [Vitest Documentation](https://vitest.dev/)
- [React Hook Form Testing](https://react-hook-form.com/advanced-usage#TestingForm)
- [Zod Testing Guide](https://zod.dev/?id=testing)
- [Web Accessibility Testing](https://www.w3.org/WAI/test-evaluate/)
