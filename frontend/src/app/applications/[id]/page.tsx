import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { ArrowLeft, Calendar, MapPin, DollarSign, ExternalLink } from 'lucide-react';
import Link from 'next/link';

export default function ApplicationDetailPage({ params }: { params: { id: string } }) {
  // Mock data - will be replaced with real API call
  const application = {
    id: params.id,
    companyName: 'Tech Company Inc.',
    position: 'Software Engineer',
    location: 'San Francisco, CA',
    jobUrl: 'https://example.com/job',
    status: 'Interview',
    priority: 'High',
    salary: 120000,
    salaryMin: 110000,
    salaryMax: 130000,
    applicationDate: '2025-03-01',
    responseDate: '2025-03-05',
    jobDescription: 'We are looking for a talented Software Engineer to join our team...',
    requirements: '- 3+ years of experience\n- Proficient in React, Node.js\n- Strong problem-solving skills',
  };

  const interviews = [
    {
      id: 1,
      interviewDate: '2025-03-10T14:00:00',
      interviewType: 'Technical',
      interviewerName: 'John Doe',
      location: 'Virtual',
      meetingLink: 'https://zoom.us/j/123456789',
    },
  ];

  const notes = [
    {
      id: 1,
      title: 'Company Research',
      content: 'Tech Company Inc. is a fast-growing startup focused on AI solutions...',
      noteType: 'Research',
      createdAt: '2025-03-01',
    },
  ];

  const getStatusColor = (status: string) => {
    const colors = {
      Applied: 'bg-blue-100 text-blue-800',
      Interview: 'bg-yellow-100 text-yellow-800',
      Offer: 'bg-green-100 text-green-800',
      Rejected: 'bg-red-100 text-red-800',
    };
    return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800';
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-2">
          <Link href="/applications">
            <Button variant="ghost" size="icon">
              <ArrowLeft className="h-4 w-4" />
            </Button>
          </Link>
          <div>
            <h2 className="text-3xl font-bold tracking-tight">
              {application.companyName}
            </h2>
            <p className="text-neutral-500">{application.position}</p>
          </div>
        </div>
        <Badge className={getStatusColor(application.status)} variant="secondary">
          {application.status}
        </Badge>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Location</CardTitle>
            <MapPin className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-lg font-semibold">{application.location}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Applied On</CardTitle>
            <Calendar className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-lg font-semibold">
              {new Date(application.applicationDate).toLocaleDateString()}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Salary Range</CardTitle>
            <DollarSign className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-lg font-semibold">
              ${application.salaryMin?.toLocaleString()} - ${application.salaryMax?.toLocaleString()}
            </div>
          </CardContent>
        </Card>
      </div>

      <Tabs defaultValue="overview" className="space-y-4">
        <TabsList>
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="interviews">Interviews</TabsTrigger>
          <TabsTrigger value="notes">Notes</TabsTrigger>
        </TabsList>

        <TabsContent value="overview" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Job Description</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-neutral-700 whitespace-pre-wrap">
                {application.jobDescription}
              </p>
              {application.jobUrl && (
                <a
                  href={application.jobUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="mt-4 inline-flex items-center text-sm text-blue-600 hover:underline"
                >
                  View Job Posting
                  <ExternalLink className="ml-1 h-3 w-3" />
                </a>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Requirements</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-neutral-700 whitespace-pre-wrap">
                {application.requirements}
              </p>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="interviews" className="space-y-4">
          <div className="flex justify-end">
            <Button>Schedule Interview</Button>
          </div>
          {interviews.length > 0 ? (
            interviews.map((interview) => (
              <Card key={interview.id}>
                <CardHeader>
                  <CardTitle>{interview.interviewType} Interview</CardTitle>
                </CardHeader>
                <CardContent className="space-y-2">
                  <div className="flex items-center text-sm">
                    <Calendar className="mr-2 h-4 w-4 text-neutral-500" />
                    {new Date(interview.interviewDate).toLocaleString()}
                  </div>
                  <div className="flex items-center text-sm">
                    <MapPin className="mr-2 h-4 w-4 text-neutral-500" />
                    {interview.location}
                  </div>
                  {interview.meetingLink && (
                    <a
                      href={interview.meetingLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="inline-flex items-center text-sm text-blue-600 hover:underline"
                    >
                      Join Meeting
                      <ExternalLink className="ml-1 h-3 w-3" />
                    </a>
                  )}
                  <p className="text-sm text-neutral-700">
                    Interviewer: {interview.interviewerName}
                  </p>
                </CardContent>
              </Card>
            ))
          ) : (
            <Card>
              <CardContent className="py-8 text-center text-neutral-500">
                No interviews scheduled yet
              </CardContent>
            </Card>
          )}
        </TabsContent>

        <TabsContent value="notes" className="space-y-4">
          <div className="flex justify-end">
            <Button>Add Note</Button>
          </div>
          {notes.length > 0 ? (
            notes.map((note) => (
              <Card key={note.id}>
                <CardHeader>
                  <div className="flex items-center justify-between">
                    <CardTitle>{note.title}</CardTitle>
                    <Badge variant="outline">{note.noteType}</Badge>
                  </div>
                </CardHeader>
                <CardContent>
                  <p className="text-sm text-neutral-700 whitespace-pre-wrap">
                    {note.content}
                  </p>
                  <p className="mt-2 text-xs text-neutral-500">
                    {new Date(note.createdAt).toLocaleDateString()}
                  </p>
                </CardContent>
              </Card>
            ))
          ) : (
            <Card>
              <CardContent className="py-8 text-center text-neutral-500">
                No notes added yet
              </CardContent>
            </Card>
          )}
        </TabsContent>
      </Tabs>
    </div>
  );
}
