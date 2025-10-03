"use client";
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Plus, Search, MapPin, Calendar, Columns, Rows } from 'lucide-react';
import Link from 'next/link';
import { useState } from 'react';

type ApplicationStatus = 'Applied' | 'Interview' | 'Offer' | 'Rejected';
type Priority = 'High' | 'Medium' | 'Low';

interface Application {
  id: number;
  companyName: string;
  position: string;
  location: string;
  status: ApplicationStatus;
  priority: Priority;
  applicationDate: string;
}

export default function ApplicationsPage() {
  // Mock data - will be replaced with real API calls
  const [viewMode, setViewMode] = useState<'list' | 'kanban'>('list');
  const [isAddOpen, setIsAddOpen] = useState(false);
  const [newCompany, setNewCompany] = useState('');
  const [newPosition, setNewPosition] = useState('');
  const [newLocation, setNewLocation] = useState('Remote');
  const [newStatus, setNewStatus] = useState<ApplicationStatus>('Applied');
  const [newPriority, setNewPriority] = useState<Priority>('Medium');
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
    },
    {
      id: 2,
      companyName: 'Startup XYZ',
      position: 'Frontend Developer',
      location: 'Remote',
      status: 'Interview',
      priority: 'Medium',
      applicationDate: '2025-02-28',
    },
    {
      id: 3,
      companyName: 'Enterprise Corp',
      position: 'Full Stack Engineer',
      location: 'New York, NY',
      status: 'Offer',
      priority: 'High',
      applicationDate: '2025-02-25',
    },
    {
      id: 4,
      companyName: 'Innovation Labs',
      position: 'Backend Developer',
      location: 'Austin, TX',
      status: 'Rejected',
      priority: 'Low',
      applicationDate: '2025-02-20',
    },
    ]
  );

  function handleAddApplication() {
    if (!newCompany.trim() || !newPosition.trim()) return;
    setApplications((prev) => [
      {
        id: (prev.at(-1)?.id || 0) + 1,
        companyName: newCompany.trim(),
        position: newPosition.trim(),
        location: newLocation.trim(),
        status: newStatus,
        priority: newPriority,
        applicationDate: new Date().toISOString(),
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

      <div className="flex items-center space-x-2">
        <div className="relative flex-1">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-neutral-500" />
          <Input
            placeholder="Search by company or position..."
            className="pl-8"
          />
        </div>
        <Button variant="outline">Filter</Button>
      </div>

      {viewMode === 'list' ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {applications.map((app) => (
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
                    Applied: {new Date(app.applicationDate).toLocaleDateString()}
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
                {applications.filter((a) => a.status === col).map((app) => (
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
