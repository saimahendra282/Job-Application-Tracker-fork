import { Application, SearchFilters, SortOption } from '@/lib/types';

/**
 * Filter applications based on search criteria
 */
export function filterApplications(applications: Application[], filters: SearchFilters): Application[] {
  return applications.filter((app) => {
    // Search filter - checks company name, position, and location
    if (filters.search) {
      const searchTerm = filters.search.toLowerCase();
      const searchableText = [
        app.companyName,
        app.position,
        app.location || '',
      ].join(' ').toLowerCase();
      
      if (!searchableText.includes(searchTerm)) {
        return false;
      }
    }

    // Status filter
    if (filters.status.length > 0 && !filters.status.includes(app.status)) {
      return false;
    }

    // Priority filter
    if (filters.priority.length > 0 && !filters.priority.includes(app.priority)) {
      return false;
    }

    // Date range filter
    if (filters.dateRange.from || filters.dateRange.to) {
      const appDate = new Date(app.applicationDate);
      
      if (filters.dateRange.from) {
        const fromDate = new Date(filters.dateRange.from);
        if (appDate < fromDate) return false;
      }
      
      if (filters.dateRange.to) {
        const toDate = new Date(filters.dateRange.to);
        if (appDate > toDate) return false;
      }
    }

    // Salary range filter
    if (filters.salaryRange.min !== undefined || filters.salaryRange.max !== undefined) {
      const salary = app.salary || app.salaryMax || 0;
      
      if (filters.salaryRange.min !== undefined && salary < filters.salaryRange.min) {
        return false;
      }
      
      if (filters.salaryRange.max !== undefined && salary > filters.salaryRange.max) {
        return false;
      }
    }

    // Location filter
    if (filters.location.length > 0) {
      const appLocation = app.location?.toLowerCase() || '';
      const matchesLocation = filters.location.some(location => 
        appLocation.includes(location.toLowerCase())
      );
      if (!matchesLocation) return false;
    }

    // Has interviews filter (mock implementation - would need actual interview data)
    if (filters.hasInterviews !== undefined) {
      // For now, randomly assign interviews to some applications
      // In real implementation, this would check actual interview data
      const hasInterviews = app.id % 3 === 0; // Mock: every 3rd application has interviews
      if (filters.hasInterviews !== hasInterviews) {
        return false;
      }
    }

    return true;
  });
}

/**
 * Sort applications based on sort option
 */
export function sortApplications(applications: Application[], sortOption: SortOption): Application[] {
  const sorted = [...applications];

  switch (sortOption) {
    case 'applicationDate-desc':
      return sorted.sort((a, b) => new Date(b.applicationDate).getTime() - new Date(a.applicationDate).getTime());
    
    case 'applicationDate-asc':
      return sorted.sort((a, b) => new Date(a.applicationDate).getTime() - new Date(b.applicationDate).getTime());
    
    case 'companyName-asc':
      return sorted.sort((a, b) => a.companyName.localeCompare(b.companyName));
    
    case 'companyName-desc':
      return sorted.sort((a, b) => b.companyName.localeCompare(a.companyName));
    
    case 'status':
      const statusOrder = { 'Applied': 0, 'Interview': 1, 'Offer': 2, 'Rejected': 3 };
      return sorted.sort((a, b) => statusOrder[a.status] - statusOrder[b.status]);
    
    case 'priority':
      const priorityOrder = { 'High': 0, 'Medium': 1, 'Low': 2 };
      return sorted.sort((a, b) => priorityOrder[a.priority] - priorityOrder[b.priority]);
    
    case 'responseTime':
      return sorted.sort((a, b) => {
        const aResponseTime = a.responseDate 
          ? new Date(a.responseDate).getTime() - new Date(a.applicationDate).getTime()
          : Infinity;
        const bResponseTime = b.responseDate 
          ? new Date(b.responseDate).getTime() - new Date(b.applicationDate).getTime()
          : Infinity;
        return aResponseTime - bResponseTime;
      });
    
    default:
      return sorted;
  }
}

/**
 * Get default search filters
 */
export function getDefaultFilters(): SearchFilters {
  return {
    search: '',
    status: [],
    priority: [],
    dateRange: {},
    salaryRange: {},
    location: [],
    hasInterviews: undefined,
  };
}

/**
 * Check if any filters are active
 */
export function hasActiveFilters(filters: SearchFilters): boolean {
  return (
    filters.search !== '' ||
    filters.status.length > 0 ||
    filters.priority.length > 0 ||
    filters.dateRange.from !== undefined ||
    filters.dateRange.to !== undefined ||
    filters.salaryRange.min !== undefined ||
    filters.salaryRange.max !== undefined ||
    filters.location.length > 0 ||
    filters.hasInterviews !== undefined
  );
}

/**
 * Highlight search terms in text
 */
export function highlightSearchTerm(text: string, searchTerm: string): string {
  if (!searchTerm) return text;
  
  const regex = new RegExp(`(${searchTerm})`, 'gi');
  return text.replace(regex, '<mark class="bg-yellow-200 dark:bg-yellow-800">$1</mark>');
}