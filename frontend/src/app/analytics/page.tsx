'use client';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { 
  BarChart3, 
  TrendingUp, 
  Target, 
  Award, 
  Download,
  Printer,
  Calendar,
  Building2,
  Clock,
  DollarSign
} from 'lucide-react';
// Using Recharts - Latest Stable Version (3.2.1+)
// Reference: https://github.com/recharts/recharts
// Documentation: http://recharts.org/
import {
  PieChart,
  Pie,
  Cell,
  BarChart,
  Bar,
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer
} from 'recharts';
import { useState, useRef } from 'react';
import { format, startOfMonth, endOfMonth } from 'date-fns';
import { useReactToPrint } from 'react-to-print';

export default function AnalyticsPage() {
  // Mock data for analytics
  const [dateRange, setDateRange] = useState({
    from: format(startOfMonth(new Date()), 'yyyy-MM-dd'),
    to: format(endOfMonth(new Date()), 'yyyy-MM-dd')
  });

  const [isPrintingReady, setIsPrintingReady] = useState(false);

  // Status distribution data
  const statusData = [
    { name: 'Applied', value: 12, color: '#3b82f6' },
    { name: 'Interview', value: 6, color: '#f59e0b' },
    { name: 'Offer', value: 2, color: '#10b981' },
    { name: 'Rejected', value: 4, color: '#ef4444' }
  ];

  // Application timeline data
  const timelineData = [
    { date: '2025-01-01', applications: 3, responses: 1 },
    { date: '2025-01-08', applications: 5, responses: 2 },
    { date: '2025-01-15', applications: 2, responses: 1 },
    { date: '2025-01-22', applications: 4, responses: 3 },
    { date: '2025-01-29', applications: 6, responses: 1 },
    { date: '2025-02-05', applications: 3, responses: 2 },
    { date: '2025-02-12', applications: 1, responses: 1 }
  ];

  // Salary insights data
  const salaryData = [
    { range: '50k-70k', count: 3 },
    { range: '70k-90k', count: 8 },
    { range: '90k-110k', count: 7 },
    { range: '110k-130k', count: 4 },
    { range: '130k+', count: 2 }
  ];

  // Top companies data
  const topCompanies = [
    { company: 'Tech Company Inc.', applications: 3, responses: 2, responseRate: 67 },
    { company: 'Startup XYZ', applications: 2, responses: 1, responseRate: 50 },
    { company: 'Enterprise Corp', applications: 4, responses: 1, responseRate: 25 },
    { company: 'Innovation Labs', applications: 2, responses: 0, responseRate: 0 },
    { company: 'Digital Solutions', applications: 3, responses: 2, responseRate: 67 }
  ];

  // Calculate metrics
  const totalApplications = statusData.reduce((sum, item) => sum + item.value, 0);
  const responseRate = Math.round(((statusData.find(s => s.name === 'Interview')?.value || 0) + 
                                 (statusData.find(s => s.name === 'Offer')?.value || 0)) / totalApplications * 100);
  const interviewRate = Math.round((statusData.find(s => s.name === 'Interview')?.value || 0) / totalApplications * 100);
  const offerRate = Math.round((statusData.find(s => s.name === 'Offer')?.value || 0) / totalApplications * 100);

  // CSV Export utility functions
  const convertToCSV = (data: Record<string, string | number>[], headers: string[]) => {
    const csvContent = [
      headers.join(','),
      ...data.map(row => 
        headers.map(header => {
          const value = row[header.toLowerCase().replace(' ', '')];
          // Escape commas and quotes in CSV
          return typeof value === 'string' && (value.includes(',') || value.includes('"')) 
            ? `"${value.replace(/"/g, '""')}"` 
            : value;
        }).join(',')
      )
    ].join('\n');
    return csvContent;
  };

  const downloadCSV = (csvContent: string, filename: string) => {
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    if (link.download !== undefined) {
      const url = URL.createObjectURL(blob);
      link.setAttribute('href', url);
      link.setAttribute('download', filename);
      link.style.visibility = 'hidden';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  };

  // Export functionality
  const handleExportChart = (chartName: string) => {
    const currentDate = format(new Date(), 'yyyy-MM-dd');
    
    switch (chartName) {
      case 'status-distribution':
        const statusCSV = convertToCSV(
          statusData.map(item => ({ status: item.name, count: item.value, percentage: ((item.value / totalApplications) * 100).toFixed(1) + '%' })),
          ['Status', 'Count', 'Percentage']
        );
        downloadCSV(statusCSV, `status-distribution-${currentDate}.csv`);
        break;
      
      case 'timeline':
        const timelineCSV = convertToCSV(
          timelineData.map(item => ({ 
            date: format(new Date(item.date), 'MMM dd, yyyy'), 
            applications: item.applications, 
            responses: item.responses 
          })),
          ['Date', 'Applications', 'Responses']
        );
        downloadCSV(timelineCSV, `application-timeline-${currentDate}.csv`);
        break;
      
      case 'salary':
        const salaryCSV = convertToCSV(
          salaryData.map(item => ({ range: item.range, count: item.count })),
          ['Salary Range', 'Count']
        );
        downloadCSV(salaryCSV, `salary-distribution-${currentDate}.csv`);
        break;
      
      case 'top-companies':
        const companiesCSV = convertToCSV(
          topCompanies.map(company => ({
            company: company.company,
            applications: company.applications,
            responses: company.responses,
            responserate: company.responseRate + '%'
          })),
          ['Company', 'Applications', 'Responses', 'Response Rate']
        );
        downloadCSV(companiesCSV, `top-companies-${currentDate}.csv`);
        break;
      
      case 'dashboard':
        // Export comprehensive dashboard data including all charts
        const comprehensiveData = [
          '# Analytics Dashboard Export',
          `# Generated on: ${format(new Date(), 'MMM dd, yyyy HH:mm')}`,
          '',
          '## Key Metrics',
          'Metric,Value',
          `Total Applications,${totalApplications}`,
          `Response Rate,${responseRate}%`,
          `Interview Rate,${interviewRate}%`,
          `Offer Rate,${offerRate}%`,
          `Average Response Time,5.2 days`,
          `Average Interview Wait,12.5 days`,
          `Average Expected Salary,$95500`,
          '',
          '## Status Distribution',
          'Status,Count,Percentage',
          ...statusData.map(item => `${item.name},${item.value},${((item.value / totalApplications) * 100).toFixed(1)}%`),
          '',
          '## Application Timeline',
          'Date,Applications,Responses',
          ...timelineData.map(item => `${format(new Date(item.date), 'MMM dd yyyy')},${item.applications},${item.responses}`),
          '',
          '## Top Companies',
          'Company,Applications,Responses,Response Rate',
          ...topCompanies.map(company => `${company.company},${company.applications},${company.responses},${company.responseRate}%`),
          '',
          '## Salary Distribution',
          'Salary Range,Count',
          ...salaryData.map(item => `${item.range},${item.count}`)
        ].join('\n');
        
        const blob = new Blob([comprehensiveData], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', `analytics-comprehensive-${currentDate}.csv`);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        break;
      
      default:
        console.log(`Exporting ${chartName} chart...`);
    }
  };

  // React-to-print functionality
  const printRef = useRef<HTMLDivElement>(null);
  const handlePrintDashboard = useReactToPrint({
    contentRef: printRef,
    documentTitle: `Analytics Dashboard - ${format(new Date(), 'MMM dd, yyyy')}`,
    onBeforePrint: async () => {
      setIsPrintingReady(true);
      console.log('Preparing to print analytics dashboard...');
      // Small delay to ensure state updates are reflected
      await new Promise(resolve => setTimeout(resolve, 500));
    },
    onAfterPrint: () => {
      setIsPrintingReady(false);
      console.log('Print dialog closed');
    },
    pageStyle: `
      @media print {
        * {
          -webkit-print-color-adjust: exact !important;
          color-adjust: exact !important;
        }
        
        body {
          font-family: system-ui, -apple-system, sans-serif;
          line-height: 1.4;
          color: #000 !important;
          background: #fff !important;
        }
        
        .print-container {
          padding: 0 !important;
          margin: 0 !important;
          width: 100% !important;
          max-width: none !important;
        }
        
        /* Hide interactive elements */
        button, .print\\:hidden {
          display: none !important;
        }
        
        /* Card styling for print */
        .card {
          border: 1px solid #ddd !important;
          box-shadow: none !important;
          margin-bottom: 16px !important;
          break-inside: avoid;
          page-break-inside: avoid;
        }
        
        /* Chart containers */
        .recharts-wrapper {
          width: 100% !important;
          height: 300px !important;
        }
        
        /* Grid layout for print */
        .grid {
          display: grid !important;
          gap: 16px !important;
          break-inside: avoid;
        }
        
        /* Typography */
        h1, h2, h3 {
          color: #000 !important;
          margin-bottom: 8px !important;
        }
        
        /* Tables */
        table {
          width: 100% !important;
          border-collapse: collapse !important;
          font-size: 12px !important;
        }
        
        th, td {
          border: 1px solid #ddd !important;
          padding: 8px !important;
          text-align: left !important;
        }
        
        /* Progress bars */
        .bg-blue-600, .bg-yellow-500, .bg-green-500 {
          background: #333 !important;
          print-color-adjust: exact !important;
        }
        
        @page {
          size: A4;
          margin: 15mm;
        }
      }
    `
  });

  return (
    <div ref={printRef} className="space-y-6 print:space-y-4 print-container">
      {/* Print-only header */}
      <div className="hidden print:block border-b pb-4 mb-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900">Job Application Analytics Dashboard</h1>
          <p className="text-sm text-gray-600 mt-2">
            Generated on {format(new Date(), 'MMMM dd, yyyy')} at {format(new Date(), 'HH:mm')}
          </p>
          <p className="text-sm text-gray-600">
            Period: {format(new Date(dateRange.from), 'MMM dd, yyyy')} - {format(new Date(dateRange.to), 'MMM dd, yyyy')}
          </p>
        </div>
      </div>

      {/* Header with filters and actions */}
      <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-4 print:hidden">
        <div>
          <h2 className="text-3xl font-bold tracking-tight">Analytics Dashboard</h2>
          <p className="text-neutral-500">
            Track your job search progress and insights
          </p>
        </div>
        
        <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-2">
          {/* Date Range Filter */}
          <div className="flex items-center gap-2">
            <Calendar className="h-4 w-4 text-neutral-500" />
            <Input
              type="date"
              value={dateRange.from}
              onChange={(e) => setDateRange(prev => ({ ...prev, from: e.target.value }))}
              className="w-auto"
            />
            <span className="text-neutral-500">to</span>
            <Input
              type="date"
              value={dateRange.to}
              onChange={(e) => setDateRange(prev => ({ ...prev, to: e.target.value }))}
              className="w-auto"
            />
          </div>
          
          {/* Export Actions */}
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => handleExportChart('dashboard')}
              className="gap-2 print:hidden"
              title="Export dashboard metrics to CSV"
            >
              <Download className="h-4 w-4" />
              Export CSV
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={handlePrintDashboard}
              className="gap-2 print:hidden"
              title="Print analytics dashboard using react-to-print"
              disabled={isPrintingReady}
            >
              <Printer className="h-4 w-4" />
              {isPrintingReady ? 'Preparing...' : 'Print'}
            </Button>
          </div>
        </div>
      </div>

      {/* Key Metrics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4 print:grid-cols-4 print:gap-2">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Applications</CardTitle>
            <BarChart3 className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{totalApplications}</div>
            <p className="text-xs text-neutral-500">
              This month
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Response Rate</CardTitle>
            <TrendingUp className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{responseRate}%</div>
            <p className="text-xs text-neutral-500">
              {(statusData.find(s => s.name === 'Interview')?.value || 0) + (statusData.find(s => s.name === 'Offer')?.value || 0)} responses from {totalApplications} applications
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Interview Rate</CardTitle>
            <Target className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{interviewRate}%</div>
            <p className="text-xs text-neutral-500">
              {statusData.find(s => s.name === 'Interview')?.value || 0} interviews from {totalApplications} applications
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Offer Rate</CardTitle>
            <Award className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{offerRate}%</div>
            <p className="text-xs text-neutral-500">
              {statusData.find(s => s.name === 'Offer')?.value || 0} offers from {totalApplications} applications
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Charts Grid */}
      <div className="grid gap-6 lg:grid-cols-2 print:grid-cols-1 print:gap-4">
        {/* Status Distribution Pie Chart */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Application Status Distribution</CardTitle>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleExportChart('status-distribution')}
              className="print:hidden"
              title="Export status distribution to CSV"
            >
              <Download className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            <div className="h-80 print:h-60">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={statusData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ name, percent }) => `${name} ${((percent as number) * 100).toFixed(0)}%`}
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="value"
                    animationBegin={0}
                    animationDuration={800}
                  >
                    {statusData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip 
                    contentStyle={{ 
                      backgroundColor: '#f8f9fa', 
                      border: '1px solid #dee2e6',
                      borderRadius: '6px'
                    }}
                    formatter={(value, name) => [`${value} applications`, name]}
                  />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        {/* Application Timeline Chart */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Application Timeline</CardTitle>
            <Button
              variant="ghost"
              size="sm"  
              onClick={() => handleExportChart('timeline')}
              className="print:hidden"
              title="Export timeline data to CSV"
            >
              <Download className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            <div className="h-80 print:h-60">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={timelineData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                  <XAxis 
                    dataKey="date" 
                    tickFormatter={(value) => format(new Date(value), 'MMM dd')}
                    stroke="#6b7280"
                  />
                  <YAxis stroke="#6b7280" />
                  <Tooltip 
                    labelFormatter={(value) => format(new Date(value), 'MMM dd, yyyy')}
                    contentStyle={{ 
                      backgroundColor: '#f8f9fa', 
                      border: '1px solid #dee2e6',
                      borderRadius: '6px'
                    }}
                  />
                  <Legend />
                  <Line 
                    type="monotone" 
                    dataKey="applications" 
                    stroke="#3b82f6" 
                    strokeWidth={3}
                    name="Applications"
                    dot={{ fill: '#3b82f6', strokeWidth: 2, r: 4 }}
                    activeDot={{ r: 6, stroke: '#3b82f6', strokeWidth: 2 }}
                    animationDuration={1000}
                  />
                  <Line 
                    type="monotone" 
                    dataKey="responses" 
                    stroke="#10b981" 
                    strokeWidth={3}
                    name="Responses"
                    dot={{ fill: '#10b981', strokeWidth: 2, r: 4 }}
                    activeDot={{ r: 6, stroke: '#10b981', strokeWidth: 2 }}
                    animationDuration={1000}
                    animationBegin={200}
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        {/* Salary Insights Chart */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Salary Range Distribution</CardTitle>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleExportChart('salary')}
              className="print:hidden"
              title="Export salary distribution to CSV"
            >
              <Download className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            <div className="h-80 print:h-60">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={salaryData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                  <XAxis dataKey="range" stroke="#6b7280" />
                  <YAxis stroke="#6b7280" />
                  <Tooltip 
                    contentStyle={{ 
                      backgroundColor: '#f8f9fa', 
                      border: '1px solid #dee2e6',
                      borderRadius: '6px'
                    }}
                    formatter={(value) => [`${value} applications`, 'Count']}
                  />
                  <Bar 
                    dataKey="count" 
                    fill="#f59e0b" 
                    radius={[4, 4, 0, 0]}
                    animationDuration={800}
                    animationBegin={0}
                  />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        {/* Response Rate Gauge (using Bar Chart) */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Response Rate Metrics</CardTitle>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleExportChart('response-rate')}
              className="print:hidden"
            >
              <Download className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Response Rate</span>
                <span className="text-sm text-neutral-500">{responseRate}%</span>
              </div>
              <div className="w-full bg-neutral-200 rounded-full h-2">
                <div 
                  className="bg-blue-600 h-2 rounded-full transition-all duration-300" 
                  style={{ width: `${responseRate}%` }}
                ></div>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Interview Rate</span>
                <span className="text-sm text-neutral-500">{interviewRate}%</span>
              </div>
              <div className="w-full bg-neutral-200 rounded-full h-2">
                <div 
                  className="bg-yellow-500 h-2 rounded-full transition-all duration-300" 
                  style={{ width: `${interviewRate}%` }}
                ></div>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Offer Rate</span>
                <span className="text-sm text-neutral-500">{offerRate}%</span>
              </div>
              <div className="w-full bg-neutral-200 rounded-full h-2">
                <div 
                  className="bg-green-500 h-2 rounded-full transition-all duration-300" 
                  style={{ width: `${offerRate}%` }}
                ></div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Top Companies Table */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <div>
            <CardTitle className="flex items-center gap-2">
              <Building2 className="h-5 w-5" />
              Top Companies
            </CardTitle>
            <p className="text-sm text-neutral-500">Companies you&apos;ve applied to most</p>
          </div>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => handleExportChart('top-companies')}
            className="print:hidden"
            title="Export companies data to CSV"
          >
            <Download className="h-4 w-4" />
          </Button>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b">
                  <th className="text-left py-2 font-medium">Company</th>
                  <th className="text-left py-2 font-medium">Applications</th>
                  <th className="text-left py-2 font-medium">Responses</th>
                  <th className="text-left py-2 font-medium">Response Rate</th>
                </tr>
              </thead>
              <tbody>
                {topCompanies.map((company, index) => (
                  <tr key={index} className="border-b">
                    <td className="py-3 font-medium">{company.company}</td>
                    <td className="py-3">{company.applications}</td>
                    <td className="py-3">{company.responses}</td>
                    <td className="py-3">
                      <div className="flex items-center gap-2">
                        <span className={`text-sm ${
                          company.responseRate >= 50 
                            ? 'text-green-600' 
                            : company.responseRate >= 25 
                            ? 'text-yellow-600' 
                            : 'text-red-600'
                        }`}>
                          {company.responseRate}%
                        </span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      {/* Average Time Metrics */}
      <div className="grid gap-4 md:grid-cols-3 print:grid-cols-3 print:gap-2">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Avg Response Time</CardTitle>
            <Clock className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">5.2 days</div>
            <p className="text-xs text-neutral-500">
              From application to first response
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Avg Interview Wait</CardTitle>
            <Calendar className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">12.5 days</div>
            <p className="text-xs text-neutral-500">
              From application to interview
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Avg Salary Expected</CardTitle>
            <DollarSign className="h-4 w-4 text-neutral-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">$95,500</div>
            <p className="text-xs text-neutral-500">
              Across all applications
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
