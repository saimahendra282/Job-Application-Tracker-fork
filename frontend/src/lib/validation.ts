import { z } from 'zod';

// Application validation schemas
export const applicationSchema = z.object({
  companyName: z.string()
    .min(1, 'Company name is required')
    .max(100, 'Company name must be less than 100 characters'),
  position: z.string()
    .min(1, 'Position is required')
    .max(100, 'Position must be less than 100 characters'),
  location: z.string()
    .max(100, 'Location must be less than 100 characters')
    .optional(),
  jobUrl: z.string()
    .url('Must be a valid URL')
    .optional()
    .or(z.literal('')),
  status: z.enum(['Applied', 'Interview', 'Offer', 'Rejected']),
  priority: z.enum(['High', 'Medium', 'Low']),
  salary: z.number()
    .positive('Salary must be positive')
    .optional(),
  salaryMin: z.number()
    .positive('Minimum salary must be positive')
    .optional(),
  salaryMax: z.number()
    .positive('Maximum salary must be positive')
    .optional(),
  applicationDate: z.string()
    .min(1, 'Application date is required'),
  responseDate: z.string().optional(),
  offerDate: z.string().optional(),
  jobDescription: z.string()
    .max(5000, 'Job description must be less than 5000 characters')
    .optional(),
  requirements: z.string()
    .max(5000, 'Requirements must be less than 5000 characters')
    .optional(),
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

export type ApplicationFormData = z.infer<typeof applicationSchema>;

// Interview validation schema
export const interviewSchema = z.object({
  applicationId: z.number().positive('Application ID is required'),
  interviewDate: z.string()
    .min(1, 'Interview date is required'),
  interviewType: z.enum(['Phone', 'Video', 'Onsite', 'Technical', 'HR', 'Final']),
  duration: z.number()
    .positive('Duration must be positive')
    .max(480, 'Duration must be less than 8 hours')
    .optional(),
  interviewerName: z.string()
    .max(100, 'Interviewer name must be less than 100 characters')
    .optional(),
  interviewerPosition: z.string()
    .max(100, 'Interviewer position must be less than 100 characters')
    .optional(),
  location: z.string()
    .max(200, 'Location must be less than 200 characters')
    .optional(),
  meetingLink: z.string()
    .url('Must be a valid URL')
    .optional()
    .or(z.literal('')),
  notes: z.string()
    .max(2000, 'Notes must be less than 2000 characters')
    .optional(),
  outcome: z.string()
    .max(500, 'Outcome must be less than 500 characters')
    .optional(),
});

export type InterviewFormData = z.infer<typeof interviewSchema>;

// Note validation schema
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

export type NoteFormData = z.infer<typeof noteSchema>;

// Contact validation schema
export const contactSchema = z.object({
  applicationId: z.number().positive('Application ID is required'),
  name: z.string()
    .min(1, 'Name is required')
    .max(100, 'Name must be less than 100 characters'),
  position: z.string()
    .max(100, 'Position must be less than 100 characters')
    .optional(),
  email: z.string()
    .email('Must be a valid email')
    .optional()
    .or(z.literal('')),
  phone: z.string()
    .max(20, 'Phone must be less than 20 characters')
    .optional(),
  linkedin: z.string()
    .url('Must be a valid URL')
    .optional()
    .or(z.literal('')),
  notes: z.string()
    .max(1000, 'Notes must be less than 1000 characters')
    .optional(),
  isPrimaryContact: z.boolean().optional(),
});

export type ContactFormData = z.infer<typeof contactSchema>;

// Search and filter validation schemas
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

export type SearchFiltersFormData = z.infer<typeof searchFiltersSchema>;

// Settings validation schema
export const settingsSchema = z.object({
  enableReminders: z.boolean(),
  showSalaryFields: z.boolean(),
  weeklySummaryEmail: z.boolean(),
  browserNotifications: z.boolean(),
  defaultApplicationStatus: z.enum(['Applied', 'Interview', 'Offer', 'Rejected']),
  defaultPriority: z.enum(['High', 'Medium', 'Low']),
  reminderDaysBefore: z.number()
    .int('Must be a whole number')
    .min(0, 'Must be 0 or greater')
    .max(30, 'Must be 30 days or less'),
  theme: z.enum(['light', 'dark', 'system']),
});

export type SettingsFormData = z.infer<typeof settingsSchema>;
