import { Card, CardContent } from '@/components/ui/card';
import { Calendar as CalendarIcon } from 'lucide-react';

export default function CalendarPage() {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight">Calendar</h2>
        <p className="text-neutral-500">
          View all your scheduled interviews
        </p>
      </div>

      <Card>
        <CardContent className="flex flex-col items-center justify-center py-16">
          <CalendarIcon className="h-16 w-16 text-neutral-400 mb-4" />
          <h3 className="text-lg font-semibold mb-2">Calendar View Coming Soon</h3>
          <p className="text-neutral-500 text-center max-w-md">
            The calendar view will display all your scheduled interviews with color-coding and
            interactive features. Integration with calendar libraries is in progress.
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
