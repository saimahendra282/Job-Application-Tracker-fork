"use client";

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { X, Filter } from 'lucide-react';
import { useState } from 'react';
import { SearchFilters, ApplicationStatus, Priority } from '@/lib/types';

interface FilterSidebarProps {
  filters: SearchFilters;
  onFiltersChange: (filters: SearchFilters) => void;
  isOpen: boolean;
  onToggle: () => void;
}

const statusOptions: ApplicationStatus[] = ['Applied', 'Interview', 'Offer', 'Rejected'];
const priorityOptions: Priority[] = ['High', 'Medium', 'Low'];

export function FilterSidebar({ filters, onFiltersChange, isOpen, onToggle }: FilterSidebarProps) {
  const [newLocation, setNewLocation] = useState('');

  const updateFilters = (updates: Partial<SearchFilters>) => {
    onFiltersChange({ ...filters, ...updates });
  };

  const toggleStatus = (status: ApplicationStatus) => {
    const newStatus = filters.status.includes(status)
      ? filters.status.filter(s => s !== status)
      : [...filters.status, status];
    updateFilters({ status: newStatus });
  };

  const togglePriority = (priority: Priority) => {
    const newPriority = filters.priority.includes(priority)
      ? filters.priority.filter(p => p !== priority)
      : [...filters.priority, priority];
    updateFilters({ priority: newPriority });
  };

  const addLocation = () => {
    if (newLocation.trim() && !filters.location.includes(newLocation.trim())) {
      updateFilters({ location: [...filters.location, newLocation.trim()] });
      setNewLocation('');
    }
  };

  const removeLocation = (location: string) => {
    updateFilters({ location: filters.location.filter(l => l !== location) });
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      addLocation();
    }
  };

  const clearAllFilters = () => {
    onFiltersChange({
      search: '',
      status: [],
      priority: [],
      dateRange: {},
      salaryRange: {},
      location: [],
      hasInterviews: undefined,
    });
  };

  const FilterContent = () => (
    <div className="space-y-6">
      {/* Status Filter */}
      <div className="space-y-2">
        <h4 className="text-sm font-medium">Status</h4>
        <div className="grid grid-cols-2 gap-2">
          {statusOptions.map((status) => (
            <Button
              key={status}
              variant={filters.status.includes(status) ? 'default' : 'outline'}
              size="sm"
              onClick={() => toggleStatus(status)}
              className="justify-start"
            >
              {status}
            </Button>
          ))}
        </div>
      </div>

      {/* Priority Filter */}
      <div className="space-y-2">
        <h4 className="text-sm font-medium">Priority</h4>
        <div className="grid grid-cols-3 gap-2">
          {priorityOptions.map((priority) => (
            <Button
              key={priority}
              variant={filters.priority.includes(priority) ? 'default' : 'outline'}
              size="sm"
              onClick={() => togglePriority(priority)}
              className="justify-start"
            >
              {priority}
            </Button>
          ))}
        </div>
      </div>

      {/* Date Range Filter */}
      <div className="space-y-2">
        <h4 className="text-sm font-medium">Application Date</h4>
        <div className="grid grid-cols-2 gap-2">
          <Input
            type="date"
            placeholder="From"
            value={filters.dateRange.from || ''}
            onChange={(e) => updateFilters({
              dateRange: { ...filters.dateRange, from: e.target.value }
            })}
          />
          <Input
            type="date"
            placeholder="To"
            value={filters.dateRange.to || ''}
            onChange={(e) => updateFilters({
              dateRange: { ...filters.dateRange, to: e.target.value }
            })}
          />
        </div>
      </div>

      {/* Salary Range Filter */}
      <div className="space-y-2">
        <h4 className="text-sm font-medium">Salary Range</h4>
        <div className="grid grid-cols-2 gap-2">
          <Input
            type="number"
            placeholder="Min ($)"
            value={filters.salaryRange.min || ''}
            onChange={(e) => updateFilters({
              salaryRange: { 
                ...filters.salaryRange, 
                min: e.target.value ? parseInt(e.target.value) : undefined 
              }
            })}
          />
          <Input
            type="number"
            placeholder="Max ($)"
            value={filters.salaryRange.max || ''}
            onChange={(e) => updateFilters({
              salaryRange: { 
                ...filters.salaryRange, 
                max: e.target.value ? parseInt(e.target.value) : undefined 
              }
            })}
          />
        </div>
      </div>

      {/* Location Filter */}
      <div className="space-y-2">
        <h4 className="text-sm font-medium">Location</h4>
        <div className="flex gap-2">
          <Input
            placeholder="Add location"
            value={newLocation}
            onChange={(e) => setNewLocation(e.target.value)}
            onKeyPress={handleKeyPress}
          />
          <Button size="sm" onClick={addLocation}>Add</Button>
        </div>
        {filters.location.length > 0 && (
          <div className="flex flex-wrap gap-1">
            {filters.location.map((location) => (
              <Badge key={location} variant="secondary" className="gap-1">
                {location}
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-4 w-4 p-0 hover:bg-transparent"
                  onClick={() => removeLocation(location)}
                >
                  <X className="h-3 w-3" />
                </Button>
              </Badge>
            ))}
          </div>
        )}
      </div>

      {/* Has Interviews Filter */}
      <div className="space-y-2">
        <h4 className="text-sm font-medium">Interviews</h4>
        <Select
          value={filters.hasInterviews === undefined ? 'all' : filters.hasInterviews.toString()}
          onValueChange={(value) => updateFilters({
            hasInterviews: value === 'all' ? undefined : value === 'true'
          })}
        >
          <SelectTrigger>
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All Applications</SelectItem>
            <SelectItem value="true">Has Interviews</SelectItem>
            <SelectItem value="false">No Interviews</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Clear Filters */}
      <Button
        variant="outline"
        className="w-full"
        onClick={clearAllFilters}
      >
        Clear All Filters
      </Button>
    </div>
  );

  // Check if any filters are active
  const hasActiveFilters = filters.status.length > 0 || 
                          filters.priority.length > 0 || 
                          filters.dateRange.from || 
                          filters.dateRange.to || 
                          filters.salaryRange.min !== undefined || 
                          filters.salaryRange.max !== undefined || 
                          filters.location.length > 0 || 
                          filters.hasInterviews !== undefined;

  if (!isOpen) {
    return (
      <Button
        variant={hasActiveFilters ? "default" : "outline"}
        onClick={onToggle}
        className="gap-2 relative"
      >
        <Filter className="h-4 w-4" />
        <span className="hidden sm:inline">Filters</span>
        {hasActiveFilters && (
          <span className="absolute -top-1 -right-1 h-2 w-2 bg-blue-600 rounded-full"></span>
        )}
      </Button>
    );
  }

  return (
    <Dialog open={isOpen} onOpenChange={onToggle}>
      <DialogContent className="h-full max-w-md p-0 m-0 translate-x-0 translate-y-0 right-0 left-auto top-0 bottom-0 data-[state=open]:slide-in-from-right data-[state=closed]:slide-out-to-right">
        <div className="flex flex-col h-full">
          <DialogHeader className="px-6 py-4 border-b">
            <DialogTitle>Filters</DialogTitle>
          </DialogHeader>
          <div className="flex-1 overflow-y-auto px-6 py-4">
            <FilterContent />
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}