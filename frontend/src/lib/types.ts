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
