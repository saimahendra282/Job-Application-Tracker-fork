export type ApplicationStatus = 'Applied' | 'Interview' | 'Offer' | 'Rejected';
export type Priority = 'High' | 'Medium' | 'Low';
export type InterviewType = 'Phone' | 'Video' | 'Onsite' | 'Technical' | 'HR' | 'Final';
export type NoteType = 'Research' | 'Interview' | 'Follow-up' | 'General' | 'Offer';

export interface Application {
  id: number;
  companyName: string;
  position: string;
  location?: string;
  jobUrl?: string;
  status: ApplicationStatus;
  priority: Priority;
  salary?: number;
  salaryMin?: number;
  salaryMax?: number;
  applicationDate: string;
  responseDate?: string;
  offerDate?: string;
  jobDescription?: string;
  requirements?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Interview {
  id: number;
  applicationId: number;
  interviewDate: string;
  interviewType: InterviewType;
  duration?: number;
  interviewerName?: string;
  interviewerPosition?: string;
  location?: string;
  meetingLink?: string;
  notes?: string;
  outcome?: string;
  reminderSent: boolean;
  createdAt: string;
}

export interface Note {
  id: number;
  applicationId: number;
  title: string;
  content: string;
  noteType: NoteType;
  createdAt: string;
  updatedAt: string;
}

export interface Contact {
  id: number;
  applicationId: number;
  name: string;
  position?: string;
  email?: string;
  phone?: string;
  linkedin?: string;
  notes?: string;
  isPrimaryContact: boolean;
  createdAt: string;
}

export interface Statistics {
  totalApplications: number;
  activeApplications: number;
  interviewCount: number;
  offerCount: number;
  rejectedCount: number;
  responseRate: number;
  averageResponseTime: number;
}

export interface StatusCounts {
  Applied: number;
  Interview: number;
  Offer: number;
  Rejected: number;
}

export type SortOption = 
  | 'applicationDate-desc'
  | 'applicationDate-asc'
  | 'companyName-asc'
  | 'companyName-desc'
  | 'status'
  | 'priority'
  | 'responseTime';

export interface SearchFilters {
  search: string;
  status: ApplicationStatus[];
  priority: Priority[];
  dateRange: {
    from?: string;
    to?: string;
  };
  salaryRange: {
    min?: number;
    max?: number;
  };
  location: string[];
  hasInterviews?: boolean;
}

export interface FilterPreset {
  id: string;
  name: string;
  filters: SearchFilters;
  createdAt: string;
}

export interface SortConfig {
  field: SortOption;
  direction: 'asc' | 'desc';
}

// Analytics Types
export interface ChartDataPoint {
  name: string;
  value: number;
  color?: string;
}

export interface TimelineDataPoint {
  date: string;
  applications: number;
  responses: number;
  interviews?: number;
  offers?: number;
}

export interface SalaryDataPoint {
  range: string;
  count: number;
  average?: number;
}

export interface CompanyAnalytics {
  company: string;
  applications: number;
  responses: number;
  interviews: number;
  offers: number;
  responseRate: number;
  averageResponseTime?: number;
}

export interface AnalyticsMetrics {
  totalApplications: number;
  responseRate: number;
  interviewRate: number;
  offerRate: number;
  averageResponseTime: number;
  averageInterviewWaitTime: number;
  averageSalary: number;
}

export interface DateRange {
  from: string;
  to: string;
}
