"use client";
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Plus, Search, MapPin, Calendar, Columns, Rows } from 'lucide-react';
import Link from 'next/link';
import { useMemo, useState } from 'react';
import type { Application, ApplicationStatus, Priority } from '@/lib/types';
import { toast } from 'sonner';
import { formatDate } from '@/lib/utils';

export default function ApplicationsPage() {
  // Mock data - will be replaced with real API calls
  const [viewMode, setViewMode] = useState<'list' | 'kanban'>('list');
  const [isAddOpen, setIsAddOpen] = useState(false);
  const [newCompany, setNewCompany] = useState('');
  const [newPosition, setNewPosition] = useState('');
  const [newLocation, setNewLocation] = useState('Remote');
  const [newStatus, setNewStatus] = useState<ApplicationStatus>('Applied');
  const [newPriority, setNewPriority] = useState<Priority>('Medium');
  const [searchTerm, setSearchTerm] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [applications, setApplications] = useState<Application[]>([
    {
      id: 1,
      companyName: 'Tech Company Inc.',
      position: 'Software Engineer',
      location: 'San Francisco, CA',
      status: 'Applied',
      priority: 'High',
      applicationDate: '2025-03-01',
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
      createdAt: '2025-02-28T00:00:00Z',
      updatedAt: '2025-02-28T00:00:00Z',
    },
    {
      id: 3,
      companyName: 'Enterprise Corp',
      position: 'Full Stack Engineer',
      location: 'New York, NY',
      status: 'Offer',
      priority: 'High',
      applicationDate: '2025-02-25',
      createdAt: '2025-02-25T00:00:00Z',
      updatedAt: '2025-02-25T00:00:00Z',
    },
    {
      id: 4,
      companyName: 'Innovation Labs',
      position: 'Backend Developer',
      location: 'Austin, TX',
      status: 'Rejected',
      priority: 'Low',
      applicationDate: '2025-02-20',
      createdAt: '2025-02-20T00:00:00Z',
      updatedAt: '2025-02-20T00:00:00Z',
    },
  ]);

  function handleAddApplication() {
    if (!newCompany.trim() || !newPosition.trim()) {
      toast.error('Please fill in all required fields');
      return;
    }
    
    setIsLoading(true);
    
    try {
      const newApplication: Application = {
        id: (applications.at(-1)?.id || 0) + 1,
        companyName: newCompany.trim(),
        position: newPosition.trim(),
        location: newLocation.trim(),
        status: newStatus,
        priority: newPriority,
        applicationDate: new Date().toISOString().split('T')[0],
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };
      
      setApplications((prev) => [newApplication, ...prev]);
      setIsAddOpen(false);
      setNewCompany('');
      setNewPosition('');
      setNewLocation('Remote');
      setNewStatus('Applied');
      setNewPriority('Medium');
      toast.success('Application added successfully');
    } catch (error) {
      console.error('Error adding application:', error);
      toast.error('Failed to add application');
    } finally {
      setIsLoading(false);
    }
  }

  const getStatusColor = (status: ApplicationStatus) => {
    const colors: Record<ApplicationStatus, string> = {
      Applied: 'bg-blue-100 text-blue-800',
      Interview: 'bg-yellow-100 text-yellow-800',
      Offer: 'bg-green-100 text-green-800',
      Rejected: 'bg-red-100 text-red-800',
    };
    return colors[status] || 'bg-gray-100 text-gray-800';
  };

  const getPriorityColor = (priority: Priority) => {
    const colors: Record<Priority, string> = {
      High: 'bg-red-100 text-red-800',
      Medium: 'bg-orange-100 text-orange-800',
      Low: 'bg-gray-100 text-gray-800',
    };
    return colors[priority] || 'bg-gray-100 text-gray-800';
  };

  const filteredApplications = useMemo(() => {
    if (!searchTerm.trim()) return applications;
    
    const term = searchTerm.toLowerCase();
    return applications.filter(app => 
      app.companyName.toLowerCase().includes(term) ||
      app.position.toLowerCase().includes(term) ||
      app.location?.toLowerCase().includes(term)
    );
  }, [applications, searchTerm]);

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
          <Button 
            variant={viewMode === 'list' ? 'default' : 'outline'} 
            size="sm" 
            onClick={() => setViewMode('list')}
            aria-pressed={viewMode === 'list'}
            aria-label="Switch to list view"
          >
            <Rows className="mr-2 h-4 w-4" /> List
          </Button>
          <Button 
            variant={viewMode === 'kanban' ? 'default' : 'outline'} 
            size="sm" 
            onClick={() => setViewMode('kanban')}
            aria-pressed={viewMode === 'kanban'}
            aria-label="Switch to kanban view"
          >
            <Columns className="mr-2 h-4 w-4" /> Kanban
          </Button>
          <Dialog open={isAddOpen} onOpenChange={setIsAddOpen}>
            <DialogTrigger asChild>
              <Button aria-label="Add new job application">
                <Plus className="mr-2 h-4 w-4" />
                Add Application
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Add Application</DialogTitle>
              </DialogHeader>
              <div className="space-y-3">
                <Input 
                  placeholder="Company Name *" 
                  value={newCompany} 
                  onChange={(e) => setNewCompany(e.target.value)}
                  aria-label="Company name"
                  required
                />
                <Input 
                  placeholder="Position *" 
                  value={newPosition} 
                  onChange={(e) => setNewPosition(e.target.value)}
                  aria-label="Job position"
                  required
                />
                <Input 
                  placeholder="Location" 
                  value={newLocation} 
                  onChange={(e) => setNewLocation(e.target.value)}
                  aria-label="Job location"
                />
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
                <Button onClick={handleAddApplication} disabled={isLoading}>
                  {isLoading ? 'Saving...' : 'Save'}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      <div className="flex items-center space-x-2">
        <div className="relative flex-1">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-neutral-500" />
          <Input
            placeholder="Search by company or position..."
            className="pl-8"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        <Button variant="outline">Filter</Button>
      </div>

      {viewMode === 'list' ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {filteredApplications.map((app) => (
            <Link key={app.id} href={`/applications/${app.id}`}>
              <Card className="hover:shadow-md transition-shadow cursor-pointer">
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div className="space-y-1">
                      <CardTitle className="text-lg">{app.companyName}</CardTitle>
                      <p className="text-sm font-medium text-neutral-700">
                        {app.position}
                      </p>
                    </div>
                    <Badge className={getPriorityColor(app.priority)} variant="secondary">
                      {app.priority}
                    </Badge>
                  </div>
                </CardHeader>
                <CardContent className="space-y-2">
                  <div className="flex items-center text-sm text-neutral-500">
                    <MapPin className="mr-1 h-3 w-3" />
                    {app.location}
                  </div>
                  <div className="flex items-center text-sm text-neutral-500">
                    <Calendar className="mr-1 h-3 w-3" />
                    Applied: {formatDate(app.applicationDate)}
                  </div>
                  <div className="mt-4">
                    <Badge className={getStatusColor(app.status)} variant="secondary">
                      {app.status}
                    </Badge>
                  </div>
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>
      ) : (
        <div className="grid gap-4 md:grid-cols-4">
          {(['Applied','Interview','Offer','Rejected'] as const).map((col) => (
            <Card key={col}>
              <CardHeader>
                <CardTitle className="text-sm">{col}</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                {filteredApplications.filter((a) => a.status === col).map((app) => (
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
