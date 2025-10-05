"use client";
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Plus, MapPin, Calendar, Columns, Rows } from 'lucide-react';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import Link from 'next/link';
import { useState, useMemo, useRef } from 'react';
import { SearchInput, SearchInputRef } from '@/components/search-input';
import { FilterSidebar } from '@/components/filter-sidebar';
import { FilterChips } from '@/components/filter-chips';
import { SortDropdown } from '@/components/sort-dropdown';
import { useDebounce, useKeyboardShortcut, useLocalStorage } from '@/lib/hooks';
import { filterApplications, sortApplications, getDefaultFilters, hasActiveFilters } from '@/lib/filters';
import { SearchFilters, SortOption } from '@/lib/types';

import { Application, ApplicationStatus, Priority } from '@/lib/types';

export default function ApplicationsPage() {
  // Mock data - will be replaced with real API calls
  const [viewMode, setViewMode] = useState<'list' | 'kanban'>('list');
  const [isAddOpen, setIsAddOpen] = useState(false);
  const [newCompany, setNewCompany] = useState('');
  const [newPosition, setNewPosition] = useState('');
  const [newLocation, setNewLocation] = useState('Remote');
  const [newStatus, setNewStatus] = useState<ApplicationStatus>('Applied');
  const [newPriority, setNewPriority] = useState<Priority>('Medium');
  
  // Search and Filter State
  const [filters, setFilters] = useLocalStorage<SearchFilters>('app-filters', getDefaultFilters());
  const [isFilterSidebarOpen, setIsFilterSidebarOpen] = useState(false);
  const [sortOption, setSortOption] = useLocalStorage<SortOption>('app-sort', 'applicationDate-desc');
  const { value: searchValue, debouncedValue: debouncedSearch, setValue: setSearchValue } = useDebounce(filters.search, 300);
  const searchInputRef = useRef<SearchInputRef>(null);
  
  // Keyboard shortcuts
  useKeyboardShortcut(['Control', 'k'], () => {
    searchInputRef.current?.focus();
  });

  // Update search in filters when debounced value changes
  const updateFilters = (newFilters: SearchFilters) => {
    setFilters(newFilters);
  };

  const handleSearchChange = (search: string) => {
    setSearchValue(search);
    updateFilters({ ...filters, search });
  };

  // Remove specific filter
  const removeFilter = (filterType: string, value?: string) => {
    const updatedFilters = { ...filters };
    
    switch (filterType) {
      case 'search':
        updatedFilters.search = '';
        setSearchValue('');
        break;
      case 'status':
        updatedFilters.status = updatedFilters.status.filter(s => s !== value);
        break;
      case 'priority':
        updatedFilters.priority = updatedFilters.priority.filter(p => p !== value);
        break;
      case 'dateRange':
        updatedFilters.dateRange = {};
        break;
      case 'salaryRange':
        updatedFilters.salaryRange = {};
        break;
      case 'location':
        updatedFilters.location = updatedFilters.location.filter(l => l !== value);
        break;
      case 'hasInterviews':
        updatedFilters.hasInterviews = undefined;
        break;
    }
    
    updateFilters(updatedFilters);
  };

  // Clear all filters
  const clearAllFilters = () => {
    const defaultFilters = getDefaultFilters();
    updateFilters(defaultFilters);
    setSearchValue('');
  };

  const [applications, setApplications] = useState<Application[]>(
    [
    {
      id: 1,
      companyName: 'Tech Company Inc.',
      position: 'Software Engineer',
      location: 'San Francisco, CA',
      status: 'Applied',
      priority: 'High',
      applicationDate: '2025-03-01',
      salary: 120000,
      createdAt: '2025-03-01T00:00:00Z',
      updatedAt: '2025-03-01T00:00:00Z',
    },
    {
      id: 2,
      companyName: 'Startup XYZ',
      position: 'Frontend Developer',
      location: 'Remote',
      status: 'Interview',
      priority: 'Medium',
      applicationDate: '2025-02-28',
      salary: 95000,
      responseDate: '2025-03-02',
      createdAt: '2025-02-28T00:00:00Z',
      updatedAt: '2025-03-02T00:00:00Z',
    },
    {
      id: 3,
      companyName: 'Enterprise Corp',
      position: 'Full Stack Engineer',
      location: 'New York, NY',
      status: 'Offer',
      priority: 'High',
      applicationDate: '2025-02-25',
      salary: 140000,
      responseDate: '2025-02-27',
      offerDate: '2025-03-01',
      createdAt: '2025-02-25T00:00:00Z',
      updatedAt: '2025-03-01T00:00:00Z',
    },
    {
      id: 4,
      companyName: 'Innovation Labs',
      position: 'Backend Developer',
      location: 'Austin, TX',
      status: 'Rejected',
      priority: 'Low',
      applicationDate: '2025-02-20',
      salary: 85000,
      responseDate: '2025-02-22',
      createdAt: '2025-02-20T00:00:00Z',
      updatedAt: '2025-02-22T00:00:00Z',
    },
    ]
  );

  // Filtered and sorted applications with performance optimization
  const filteredAndSortedApplications = useMemo(() => {
    const filtered = filterApplications(applications, { ...filters, search: debouncedSearch });
    return sortApplications(filtered, sortOption);
  }, [applications, filters, debouncedSearch, sortOption]);

  function handleAddApplication() {
    if (!newCompany.trim() || !newPosition.trim()) return;
    const now = new Date().toISOString();
    setApplications((prev) => [
      {
        id: (prev.at(-1)?.id || 0) + 1,
        companyName: newCompany.trim(),
        position: newPosition.trim(),
        location: newLocation.trim(),
        status: newStatus,
        priority: newPriority,
        applicationDate: now,
        createdAt: now,
        updatedAt: now,
      },
      ...prev,
    ]);
    setIsAddOpen(false);
    setNewCompany('');
    setNewPosition('');
  }

  const getStatusColor = (status: string) => {
    const colors = {
      Applied: 'bg-blue-100 text-blue-800',
      Interview: 'bg-yellow-100 text-yellow-800',
      Offer: 'bg-green-100 text-green-800',
      Rejected: 'bg-red-100 text-red-800',
    };
    return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800';
  };

  const getPriorityColor = (priority: string) => {
    const colors = {
      High: 'bg-red-100 text-red-800',
      Medium: 'bg-orange-100 text-orange-800',
      Low: 'bg-gray-100 text-gray-800',
    };
    return colors[priority as keyof typeof colors] || 'bg-gray-100 text-gray-800';
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Applications</h2>
          <p className="text-neutral-500">
            Manage all your job applications
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button variant={viewMode === 'list' ? 'default' : 'outline'} size="sm" onClick={() => setViewMode('list')}>
            <Rows className="mr-2 h-4 w-4" /> List
          </Button>
          <Button variant={viewMode === 'kanban' ? 'default' : 'outline'} size="sm" onClick={() => setViewMode('kanban')}>
            <Columns className="mr-2 h-4 w-4" /> Kanban
          </Button>
          <Dialog open={isAddOpen} onOpenChange={setIsAddOpen}>
            <DialogTrigger asChild>
              <Button>
                <Plus className="mr-2 h-4 w-4" />
                Add Application
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Add Application</DialogTitle>
              </DialogHeader>
              <div className="space-y-3">
                <Input placeholder="Company Name *" value={newCompany} onChange={(e) => setNewCompany(e.target.value)} />
                <Input placeholder="Position *" value={newPosition} onChange={(e) => setNewPosition(e.target.value)} />
                <Input placeholder="Location" value={newLocation} onChange={(e) => setNewLocation(e.target.value)} />
                <div className="flex flex-wrap gap-3">
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-neutral-600">Status</span>
                    <Select value={newStatus} onValueChange={(v) => setNewStatus(v as ApplicationStatus)}>
                      <SelectTrigger className="min-w-[10rem]"><SelectValue placeholder="Select" /></SelectTrigger>
                      <SelectContent>
                        {(['Applied','Interview','Offer','Rejected'] as const).map(s => (
                          <SelectItem key={s} value={s}>{s}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-neutral-600">Priority</span>
                    <Select value={newPriority} onValueChange={(v) => setNewPriority(v as Priority)}>
                      <SelectTrigger className="min-w-[10rem]"><SelectValue placeholder="Select" /></SelectTrigger>
                      <SelectContent>
                        {(['High','Medium','Low'] as const).map(p => (
                          <SelectItem key={p} value={p}>{p}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              </div>
              <DialogFooter>
                <Button variant="ghost" onClick={() => setIsAddOpen(false)}>Cancel</Button>
                <Button onClick={handleAddApplication}>Save</Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      <div className="flex flex-col space-y-2 sm:flex-row sm:items-center sm:space-y-0 sm:space-x-2">
        <SearchInput
          ref={searchInputRef}
          value={searchValue}
          onChange={handleSearchChange}
          placeholder="Search by company, position, or location..."
          className="flex-1"
        />
        <div className="flex items-center space-x-2">
          <SortDropdown
            currentSort={sortOption}
            onSortChange={setSortOption}
          />
          <FilterSidebar
            filters={filters}
            onFiltersChange={updateFilters}
            isOpen={isFilterSidebarOpen}
            onToggle={() => setIsFilterSidebarOpen(!isFilterSidebarOpen)}
          />
        </div>
      </div>

      {hasActiveFilters(filters) && (
        <FilterChips
          filters={filters}
          onRemoveFilter={removeFilter}
          onClearAll={clearAllFilters}
        />
      )}

      {viewMode === 'list' ? (
        <Card>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Company</TableHead>
                <TableHead>Position</TableHead>
                <TableHead>Location</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Priority</TableHead>
                <TableHead>Application Date</TableHead>
                <TableHead>Salary</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredAndSortedApplications.map((app) => (
                <TableRow key={app.id} className="cursor-pointer hover:bg-neutral-50 dark:hover:bg-neutral-900">
                  <TableCell>
                    <Link href={`/applications/${app.id}`} className="block">
                      <div className="font-medium">{app.companyName}</div>
                    </Link>
                  </TableCell>
                  <TableCell>
                    <Link href={`/applications/${app.id}`} className="block">
                      <div className="text-neutral-700 dark:text-neutral-300">{app.position}</div>
                    </Link>
                  </TableCell>
                  <TableCell>
                    <Link href={`/applications/${app.id}`} className="block">
                      <div className="flex items-center text-neutral-500">
                        <MapPin className="mr-1 h-3 w-3" />
                        {app.location}
                      </div>
                    </Link>
                  </TableCell>
                  <TableCell>
                    <Link href={`/applications/${app.id}`} className="block">
                      <Badge className={getStatusColor(app.status)} variant="secondary">
                        {app.status}
                      </Badge>
                    </Link>
                  </TableCell>
                  <TableCell>
                    <Link href={`/applications/${app.id}`} className="block">
                      <Badge className={getPriorityColor(app.priority)} variant="secondary">
                        {app.priority}
                      </Badge>
                    </Link>
                  </TableCell>
                  <TableCell>
                    <Link href={`/applications/${app.id}`} className="block">
                      <div className="flex items-center text-neutral-500">
                        <Calendar className="mr-1 h-3 w-3" />
                        {new Date(app.applicationDate).toLocaleDateString()}
                      </div>
                    </Link>
                  </TableCell>
                  <TableCell>
                    <Link href={`/applications/${app.id}`} className="block">
                      <div className="text-neutral-600">
                        {app.salary ? `$${app.salary.toLocaleString()}` : 'N/A'}
                      </div>
                    </Link>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-4">
          {(['Applied','Interview','Offer','Rejected'] as const).map((col) => (
            <Card key={col}>
              <CardHeader>
                <CardTitle className="text-sm">{col}</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                {filteredAndSortedApplications.filter((a) => a.status === col).map((app) => (
                  <Link key={app.id} href={`/applications/${app.id}`}>
                    <div className="border rounded-md p-3 hover:bg-neutral-50 dark:hover:bg-neutral-900 cursor-pointer">
                      <div className="flex items-center justify-between">
                        <span className="font-medium text-sm">{app.companyName}</span>
                        <Badge className={getPriorityColor(app.priority)} variant="secondary">{app.priority}</Badge>
                      </div>
                      <p className="text-xs text-neutral-600">{app.position}</p>
                    </div>
                  </Link>
                ))}
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
