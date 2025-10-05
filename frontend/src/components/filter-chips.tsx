"use client";

import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { X } from 'lucide-react';
import { SearchFilters, ApplicationStatus, Priority } from '@/lib/types';

interface FilterChipsProps {
  filters: SearchFilters;
  onRemoveFilter: (filterType: string, value?: string) => void;
  onClearAll: () => void;
}

export function FilterChips({ filters, onRemoveFilter, onClearAll }: FilterChipsProps) {
  const activeFilters = [];

  // Search filter
  if (filters.search) {
    activeFilters.push({
      type: 'search',
      label: `Search: "${filters.search}"`,
      value: filters.search,
    });
  }

  // Status filters
  filters.status.forEach((status: ApplicationStatus) => {
    activeFilters.push({
      type: 'status',
      label: `Status: ${status}`,
      value: status,
    });
  });

  // Priority filters
  filters.priority.forEach((priority: Priority) => {
    activeFilters.push({
      type: 'priority',
      label: `Priority: ${priority}`,
      value: priority,
    });
  });

  // Date range filter
  if (filters.dateRange.from || filters.dateRange.to) {
    const fromDate = filters.dateRange.from ? new Date(filters.dateRange.from).toLocaleDateString() : 'Any';
    const toDate = filters.dateRange.to ? new Date(filters.dateRange.to).toLocaleDateString() : 'Any';
    activeFilters.push({
      type: 'dateRange',
      label: `Date: ${fromDate} - ${toDate}`,
      value: 'dateRange',
    });
  }

  // Salary range filter
  if (filters.salaryRange.min !== undefined || filters.salaryRange.max !== undefined) {
    const min = filters.salaryRange.min ? `$${filters.salaryRange.min.toLocaleString()}` : 'Any';
    const max = filters.salaryRange.max ? `$${filters.salaryRange.max.toLocaleString()}` : 'Any';
    activeFilters.push({
      type: 'salaryRange',
      label: `Salary: ${min} - ${max}`,
      value: 'salaryRange',
    });
  }

  // Location filters
  filters.location.forEach((location: string) => {
    activeFilters.push({
      type: 'location',
      label: `Location: ${location}`,
      value: location,
    });
  });

  // Has interviews filter
  if (filters.hasInterviews !== undefined) {
    activeFilters.push({
      type: 'hasInterviews',
      label: filters.hasInterviews ? 'Has Interviews' : 'No Interviews',
      value: 'hasInterviews',
    });
  }

  if (activeFilters.length === 0) {
    return null;
  }

  return (
    <div className="flex flex-wrap items-center gap-2">
      <span className="text-sm text-neutral-600">Active filters:</span>
      {activeFilters.map((filter, index) => (
        <Badge key={`${filter.type}-${filter.value}-${index}`} variant="secondary" className="gap-1">
          {filter.label}
          <Button
            variant="ghost"
            size="sm"
            className="h-4 w-4 p-0 hover:bg-transparent"
            onClick={() => onRemoveFilter(filter.type, filter.value)}
          >
            <X className="h-3 w-3" />
            <span className="sr-only">Remove filter</span>
          </Button>
        </Badge>
      ))}
      {activeFilters.length > 1 && (
        <Button
          variant="ghost"
          size="sm"
          onClick={onClearAll}
          className="h-6 text-xs text-neutral-500 hover:text-neutral-700"
        >
          Clear all
        </Button>
      )}
    </div>
  );
}