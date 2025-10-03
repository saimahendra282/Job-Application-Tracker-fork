# Job Application Tracker - Feature Specification & Architecture

## 📋 Table of Contents
1. [Core Features](#core-features)
2. [Database Design](#database-design)
3. [Server-Side Functionalities](#server-side-functionalities)
4. [Client-Side Functionalities](#client-side-functionalities)
5. [Application Flow & Processes](#application-flow--processes)
6. [Team Task Division](#team-task-division)

---

## 🎯 Core Features

### **Phase 1: MVP Features (Must Have - Complete in 3-4 days)**

#### 1. **Application Management**
- Add new job application with basic details
- View all applications in list/grid view
- Edit application details
- Delete applications
- Filter applications by status
- Sort applications (by date, company, status)
- Search applications by company name or position

#### 2. **Status Tracking**
- Four main statuses: Applied, Interview, Offer, Rejected
- Visual status indicators (color-coded badges)
- Quick status update (drag-and-drop or dropdown)
- Status change history/timeline
- Set application priority (High, Medium, Low)

#### 3. **Interview Management**
- Schedule interviews linked to applications
- Multiple interviews per application
- Interview types (Phone, Video, On-site, Technical)
- Store interviewer details
- Add interview notes
- Meeting links for virtual interviews

#### 4. **Calendar View**
- Monthly calendar showing all scheduled interviews
- Color-coded by application status
- Click on calendar event to view details
- Day/Week/Month view options
- Today's interviews highlighted

#### 5. **Company Research Notes**
- Add multiple notes per application
- Note categories (Research, Interview Prep, Follow-up)
- Rich text formatting
- Timestamp for each note
- Edit and delete notes

#### 6. **Analytics Dashboard**
- Total applications count
- Applications by status (pie chart)
- Application timeline (line chart showing applications over time)
- Response rate calculation (interviews/total applications)
- Average time between application and interview
- Success rate (offers/total applications)

#### 7. **Email Reminders**
- Automatic email reminder 24 hours before interview
- Manual reminder setting for follow-ups
- Weekly summary of upcoming interviews

---

### **Phase 2: Nice-to-Have Features (If time permits)**

#### 8. **Document Management**
- Upload resume versions used for each application
- Store cover letters
- Attach offer letters
- Portfolio links

#### 9. **Contact Management**
- Store recruiter/hiring manager details
- LinkedIn profiles
- Email addresses
- Notes about contacts

#### 10. **Application Templates**
- Save application details as templates
- Quick apply using templates
- Common company presets

#### 11. **Export & Reports**
- Export applications to CSV/Excel
- Generate PDF reports
- Print-friendly views

#### 12. **Advanced Features**
- Browser extension to add jobs from LinkedIn/Indeed
- Job posting URL scraper (extract company, position automatically)
- Salary tracking and comparison
- Location-based filtering

---

## 🗄️ Database Design

### **Database Tables & Relationships**

#### **1. Applications Table (Main Entity)**
**Purpose:** Store all job application records

**Fields:**
- `Id` - Primary Key (Auto-increment)
- `CompanyName` - String (Required)
- `Position` - String (Required)
- `Location` - String (City, State/Country)
- `JobUrl` - String (Link to job posting)
- `Status` - Enum/String (Applied, Interview, Offer, Rejected)
- `Priority` - Enum/String (High, Medium, Low)
- `Salary` - Decimal (Expected/Offered salary)
- `SalaryMin` - Decimal (Salary range minimum)
- `SalaryMax` - Decimal (Salary range maximum)
- `ApplicationDate` - DateTime (When you applied)
- `ResponseDate` - DateTime (When company responded)
- `OfferDate` - DateTime (When offer received)
- `JobDescription` - Text (Copy of job description)
- `Requirements` - Text
- `CreatedAt` - DateTime
- `UpdatedAt` - DateTime
- `UserId` - Foreign Key (for multi-user support)

**Indexes:**
- Index on `Status` for filtering
- Index on `CompanyName` for searching
- Index on `ApplicationDate` for sorting

---

#### **2. Interviews Table**
**Purpose:** Track all interview schedules

**Fields:**
- `Id` - Primary Key
- `ApplicationId` - Foreign Key → Applications
- `InterviewDate` - DateTime (Required)
- `InterviewType` - Enum (Phone, Video, Onsite, Technical, HR, Final)
- `Duration` - Integer (Minutes)
- `InterviewerName` - String
- `InterviewerPosition` - String
- `Location` - String (Office address or "Virtual")
- `MeetingLink` - String (Zoom/Teams/Meet link)
- `Notes` - Text (Interview notes)
- `Outcome` - String (Passed, Failed, Waiting)
- `ReminderSent` - Boolean (Has email reminder been sent?)
- `CreatedAt` - DateTime

**Relationships:**
- One Application → Many Interviews (One-to-Many)

**Indexes:**
- Index on `InterviewDate` for calendar queries
- Index on `ApplicationId` for fetching interviews per application

---

#### **3. Notes Table**
**Purpose:** Store research and preparation notes

**Fields:**
- `Id` - Primary Key
- `ApplicationId` - Foreign Key → Applications
- `Title` - String
- `Content` - Text (Rich text/Markdown)
- `NoteType` - Enum (Research, Interview, Follow-up, General, Offer)
- `CreatedAt` - DateTime
- `UpdatedAt` - DateTime

**Relationships:**
- One Application → Many Notes (One-to-Many)

---

#### **4. Contacts Table**
**Purpose:** Store recruiter and hiring manager information

**Fields:**
- `Id` - Primary Key
- `ApplicationId` - Foreign Key → Applications
- `Name` - String (Required)
- `Position` - String (Job title)
- `Email` - String
- `Phone` - String
- `LinkedIn` - String (URL)
- `Notes` - Text
- `IsPrimaryContact` - Boolean
- `CreatedAt` - DateTime

**Relationships:**
- One Application → Many Contacts (One-to-Many)

---

#### **5. Documents Table**
**Purpose:** Store uploaded documents

**Fields:**
- `Id` - Primary Key
- `ApplicationId` - Foreign Key → Applications
- `DocumentType` - Enum (Resume, CoverLetter, Portfolio, OfferLetter)
- `FileName` - String
- `FileUrl` - String (Cloud storage URL or local path)
- `FileSize` - Integer (Bytes)
- `UploadedAt` - DateTime

**Relationships:**
- One Application → Many Documents (One-to-Many)

---

#### **6. StatusHistory Table (Optional but useful)**
**Purpose:** Track status changes over time

**Fields:**
- `Id` - Primary Key
- `ApplicationId` - Foreign Key → Applications
- `OldStatus` - String
- `NewStatus` - String
- `ChangedAt` - DateTime
- `Notes` - Text (Reason for change)

**Relationships:**
- One Application → Many StatusHistory records

---

#### **7. Users Table (For future multi-user support)**
**Purpose:** User authentication and profiles

**Fields:**
- `Id` - Primary Key
- `Email` - String (Unique)
- `PasswordHash` - String
- `FirstName` - String
- `LastName` - String
- `CreatedAt` - DateTime

**Relationships:**
- One User → Many Applications (One-to-Many)

---

### **Database Relationships Diagram**

```
Users (1) ──────→ (Many) Applications
                        │
                        ├──→ (Many) Interviews
                        ├──→ (Many) Notes
                        ├──→ (Many) Contacts
                        ├──→ (Many) Documents
                        └──→ (Many) StatusHistory
```

---

## 🖥️ Server-Side Functionalities

### **Backend API Responsibilities**

#### **1. Application Management API**

**Endpoints:**
- `GET /api/applications` - Get all applications (with filtering, sorting, pagination)
- `GET /api/applications/{id}` - Get single application with all related data
- `POST /api/applications` - Create new application
- `PUT /api/applications/{id}` - Update application
- `PATCH /api/applications/{id}/status` - Update only status
- `DELETE /api/applications/{id}` - Delete application (cascade delete related data)

**Business Logic:**
- Validate required fields (CompanyName, Position)
- Set default status to "Applied" if not provided
- Auto-set ApplicationDate to current date if not provided
- Validate status transitions (can't go from Rejected back to Applied)
- Calculate and update ResponseDate when status changes from Applied
- Ensure salary ranges are valid (min <= max)

**Data Processing:**
- Support filtering by: status, priority, date range, company name
- Support sorting by: date (newest/oldest), company name, status, priority
- Implement pagination (page size: 10-20 items)
- Include related data counts (number of interviews, notes, contacts)

---

#### **2. Interview Management API**

**Endpoints:**
- `GET /api/interviews` - Get all interviews
- `GET /api/interviews/calendar?start={date}&end={date}` - Get interviews for calendar view
- `GET /api/applications/{appId}/interviews` - Get interviews for specific application
- `POST /api/applications/{appId}/interviews` - Create new interview
- `PUT /api/interviews/{id}` - Update interview
- `DELETE /api/interviews/{id}` - Delete interview

**Business Logic:**
- Validate InterviewDate is in the future (for new interviews)
- Prevent duplicate interviews at the same time
- Auto-change application status to "Interview" when first interview is scheduled
- Calculate interview duration if not provided (default 60 minutes)
- Validate meeting links (proper URL format)

**Calendar Functionality:**
- Return interviews within date range for calendar rendering
- Group interviews by date
- Include application details in calendar events
- Support timezone conversion if needed

---

#### **3. Notes Management API**

**Endpoints:**
- `GET /api/applications/{appId}/notes` - Get all notes for application
- `POST /api/applications/{appId}/notes` - Create note
- `PUT /api/notes/{id}` - Update note
- `DELETE /api/notes/{id}` - Delete note

**Business Logic:**
- Auto-timestamp on creation and updates
- Support Markdown formatting
- Validate note types
- Search within notes content

---

#### **4. Statistics & Analytics API**

**Endpoints:**
- `GET /api/statistics/overview` - Get summary statistics
- `GET /api/statistics/by-status` - Count of applications per status
- `GET /api/statistics/timeline?months={n}` - Applications over time
- `GET /api/statistics/response-rate` - Calculate response metrics
- `GET /api/statistics/salary-insights` - Salary statistics

**Calculated Metrics:**
- **Total Applications:** Count of all applications
- **Active Applications:** Count where status != Rejected
- **Response Rate:** (Interviews + Offers) / Total Applications × 100%
- **Interview Rate:** Applications with status = Interview / Total
- **Offer Rate:** Offers / Total Applications × 100%
- **Average Response Time:** Average days between ApplicationDate and ResponseDate
- **Average Salary Offered:** Mean of all offered salaries
- **Success Rate:** Offers / (Offers + Rejected) × 100%

**Timeline Data:**
- Group applications by month
- Count applications per status per month
- Calculate trend (increasing/decreasing)

---

#### **5. Email Reminder System (Background Service)**

**Responsibilities:**
- Run as a background worker/cron job
- Check for upcoming interviews every hour
- Send email reminders 24 hours before interview
- Mark reminder as sent to prevent duplicates
- Send weekly summary emails (optional)

**Email Content:**
- Interview date and time
- Company name and position
- Interview type and location/link
- Interviewer details
- Link to view full details in app

**Email Templates:**
- Interview reminder template
- Weekly summary template
- Follow-up reminder template

---

#### **6. Contacts Management API**

**Endpoints:**
- `GET /api/applications/{appId}/contacts` - Get contacts
- `POST /api/applications/{appId}/contacts` - Add contact
- `PUT /api/contacts/{id}` - Update contact
- `DELETE /api/contacts/{id}` - Delete contact

**Business Logic:**
- Validate email format
- Validate LinkedIn URL format
- Mark one contact as primary

---

#### **7. File Upload API (Optional)**

**Endpoints:**
- `POST /api/applications/{appId}/documents` - Upload document
- `GET /api/documents/{id}` - Download document
- `DELETE /api/documents/{id}` - Delete document

**Business Logic:**
- File validation (type, size limits)
- Store files in cloud storage (Azure Blob, AWS S3) or local folder
- Generate secure download URLs
- Virus scanning (if critical)

---

### **Backend Technical Requirements**

**Data Validation:**
- Use Data Annotations or FluentValidation
- Validate all inputs before database operations
- Return proper error messages with HTTP status codes

**Error Handling:**
- Global exception handling middleware
- Log all errors
- Return user-friendly error messages
- Never expose stack traces in production

**Security:**
- Input sanitization to prevent SQL injection
- CORS configuration for React frontend
- Rate limiting on API endpoints
- (Future) JWT authentication

**Performance:**
- Use async/await for all database operations
- Implement caching for statistics queries
- Database indexing on frequently queried fields
- Lazy loading for related entities

**Logging:**
- Log all API requests
- Log all errors with stack traces
- Log background job execution

---

## 💻 Client-Side Functionalities

### **Frontend Responsibilities**

#### **1. Dashboard/Home Page**

**Display:**
- Summary cards: Total Applications, Interview Rate, Offer Rate, Active Applications
- Quick stats with icons and colors
- Recent applications (last 5)
- Upcoming interviews (next 7 days)
- Status distribution pie chart
- Application timeline chart (last 6 months)

**Interactions:**
- Click on stat cards to filter applications
- Click on recent applications to view details
- Click on upcoming interviews to view calendar

**Data Flow:**
- Fetch statistics from `/api/statistics/overview`
- Fetch recent applications from `/api/applications?limit=5&sort=date`
- Fetch upcoming interviews from `/api/interviews/calendar`

---

#### **2. Applications List Page**

**Display:**
- Grid or list view of all applications
- Application cards showing: Company logo, Position, Location, Status badge, Application date, Priority indicator
- Filter sidebar: By status, By priority, By date range
- Sort options: Newest, Oldest, Company A-Z, Status
- Search bar: Search by company or position name

**Interactions:**
- Click application card → Navigate to detail page
- Quick status change dropdown on each card
- Delete button with confirmation modal
- "Add New Application" button → Open form modal
- Drag-and-drop to change status (Kanban board view)

**Data Flow:**
- Fetch applications on page load and filter change
- Real-time updates when status changes
- Optimistic UI updates for better UX

**View Modes:**
- **List View:** Detailed rows with all info
- **Grid View:** Cards with summary info
- **Kanban View:** Columns by status with drag-and-drop

---

#### **3. Application Detail Page**

**Display:**
- Full application details
- Edit button for each section
- Status change dropdown
- Priority selector
- Tabbed interface:
  - **Overview Tab:** All application details
  - **Interviews Tab:** List of all interviews with details
  - **Notes Tab:** All research and notes
  - **Contacts Tab:** Recruiter/hiring manager info
  - **Documents Tab:** Uploaded files

**Interactions:**
- Edit any field inline or via modal
- Add new interview → Open interview form
- Add new note → Open note editor
- Delete application with confirmation
- Mark as favorite/important
- Share application details (export to PDF)

**Data Flow:**
- Fetch single application with all related data
- Lazy load tabs (fetch data when tab is clicked)
- Update data optimistically

---

#### **4. Interview Calendar Page**

**Display:**
- Full calendar view (FullCalendar library)
- Color-coded events by application status
- Month/Week/Day view toggle
- Interview details on event click
- Today button to jump to current date
- Mini calendar for quick date navigation

**Interactions:**
- Click event → View interview details popup
- Click date → Add new interview for that date
- Drag event to reschedule (optional)
- Filter by application or interview type
- Export calendar to iCal/Google Calendar

**Data Flow:**
- Fetch interviews for current month on load
- Fetch more data when navigating months
- Real-time updates when interviews are added/modified

---

#### **5. Add/Edit Application Form**

**Form Fields:**
- Company Name * (Required, autocomplete from previous entries)
- Position * (Required)
- Location
- Job URL
- Status (Dropdown: Applied, Interview, Offer, Rejected)
- Priority (Dropdown: High, Medium, Low)
- Salary Range (Min - Max)
- Application Date (Date picker, default: today)
- Job Description (Textarea with formatting)
- Requirements (Textarea)

**Interactions:**
- Real-time validation
- Save button → API call to create/update
- Cancel button → Close modal without saving
- Autosave draft (localStorage) for long forms

**Data Flow:**
- POST to `/api/applications` for new
- PUT to `/api/applications/{id}` for updates
- Show success/error toast notifications

---

#### **6. Add/Edit Interview Form**

**Form Fields:**
- Interview Date & Time * (Required, date-time picker)
- Interview Type * (Dropdown: Phone, Video, Onsite, Technical)
- Duration (Number input, minutes)
- Interviewer Name
- Interviewer Position
- Location (if onsite)
- Meeting Link (if virtual)
- Notes (Textarea)

**Interactions:**
- Validate date is in future
- Validate meeting link format
- Save → Create interview
- Email reminder checkbox

**Data Flow:**
- POST to `/api/applications/{appId}/interviews`
- Update application status if first interview

---

#### **7. Notes Section**

**Display:**
- List of all notes sorted by date
- Note type badges
- Expand/collapse note content
- Edit/Delete buttons per note

**Interactions:**
- Add note button → Open rich text editor
- Edit note → Open editor with existing content
- Delete note → Confirmation modal
- Filter notes by type

**Editor Features:**
- Rich text formatting (bold, italic, lists)
- Markdown support
- Auto-save drafts
- Character count

---

#### **8. Analytics Dashboard Page**

**Charts & Visualizations:**

**A. Status Distribution Pie Chart**
- Shows count per status with percentages
- Color-coded slices
- Interactive (click to filter)

**B. Application Timeline Chart**
- Line/Bar chart showing applications over time
- X-axis: Months, Y-axis: Count
- Multiple lines for different statuses
- Date range selector

**C. Response Rate Metrics**
- Gauge chart or progress bars
- Interview rate: % of applications that got interviews
- Offer rate: % of applications that got offers
- Rejection rate: % rejected

**D. Average Time Metrics**
- Time from application to response
- Time from interview to offer
- Display as cards with icons

**E. Salary Insights**
- Average salary offered
- Salary range distribution
- Bar chart of salary by company/position

**F. Top Companies Table**
- Companies with most applications
- Companies with best response rates

**Interactions:**
- Filter by date range
- Export charts as images
- Print dashboard

**Data Flow:**
- Fetch all statistics on page load
- Use React Query for caching
- Refresh data periodically

---

#### **9. Search & Filter Functionality**

**Global Search:**
- Search across all applications
- Search by company name, position, location
- Real-time search results
- Keyboard shortcuts (Ctrl+K)

**Advanced Filters:**
- Status (multi-select)
- Priority (multi-select)
- Date range (from - to)
- Salary range
- Has interviews (yes/no)
- Location
- Save filter presets

**Filter UI:**
- Sidebar with all filter options
- Applied filters chips (removable)
- Clear all filters button
- Filter count badge

---

#### **10. Notifications & Reminders (Client-side)**

**Notification Types:**
- Interview reminders (from backend)
- Success/error toasts for actions
- Browser notifications (with permission)
- Upcoming interview badges

**Implementation:**
- Toast notifications library (react-hot-toast)
- Browser Notification API
- Visual badges on calendar dates
- Bell icon with notification count

---

### **Client-Side Technical Requirements**

**State Management:**
- React Context for global state (user, theme)
- React Query for server state (applications, interviews)
- Local state for UI (modals, forms)
- LocalStorage for user preferences

**Form Handling:**
- React Hook Form for form state
- Yup/Zod for validation schemas
- Error messages under each field
- Disable submit during API call

**Data Fetching:**
- Axios for HTTP requests
- React Query for caching and synchronization
- Loading states with skeletons
- Error boundaries for error handling

**Routing:**
- React Router for navigation
- Protected routes (for future auth)
- Dynamic routes (/applications/:id)
- 404 page for invalid routes

**UI/UX:**
- Responsive design (mobile, tablet, desktop)
- Dark mode support (optional)
- Loading skeletons instead of spinners
- Smooth transitions and animations
- Accessibility (ARIA labels, keyboard navigation)

**Performance:**
- Lazy load pages and components
- Infinite scroll or pagination for long lists
- Debounce search inputs
- Memoize expensive calculations
- Image lazy loading

---

## 🔄 Application Flow & Processes

### **Process 1: Adding a New Job Application**

**User Flow:**
1. User clicks "Add New Application" button
2. Form modal opens
3. User fills in company name, position, and other details
4. User selects status (default: Applied)
5. User clicks "Save"
6. Form validates inputs
7. If valid → API call to POST /api/applications
8. Backend creates new application record
9. Backend returns created application with ID
10. Frontend closes modal and shows success toast
11. New application appears in list
12. Dashboard statistics update

**Data Flow:**
```
User Input → Form Validation → API Call → Database Insert 
→ Return Response → Update UI → Show Notification
```

---

### **Process 2: Scheduling an Interview**

**User Flow:**
1. User opens application detail page
2. User clicks "Schedule Interview" button
3. Interview form opens
4. User selects date/time, type, and adds details
5. User clicks "Save Interview"
6. Frontend validates interview date is in future
7. API call to POST /api/applications/{appId}/interviews
8. Backend creates interview record
9. Backend updates application status to "Interview" (if first interview)
10. Backend schedules email reminder for 24 hours before
11. Frontend updates application detail page
12. Interview appears on calendar
13. Success notification shown

**Background Process:**
- Background service runs every hour
- Checks for interviews in next 24 hours where ReminderSent = false
- Sends email reminder to user
- Marks ReminderSent = true

---

### **Process 3: Tracking Status Changes**

**User Flow:**
1. User views application list
2. User clicks status dropdown on an application card
3. User selects new status (e.g., "Offer")
4. Confirmation modal appears (optional)
5. API call to PATCH /api/applications/{id}/status
6. Backend validates status transition
7. Backend updates status and UpdatedAt timestamp
8. Backend creates StatusHistory record
9. Backend calculates new statistics
10. Frontend updates card status badge
11. Dashboard statistics refresh
12. Success toast notification

**Status Transition Rules:**
- Applied → Interview ✅
- Applied → Rejected ✅
- Interview → Offer ✅
- Interview → Rejected ✅
- Offer → Accepted ✅ (future feature)
- Rejected → * ❌ (Cannot go back from rejected)

---

### **Process 4: Viewing Analytics**

**User Flow:**
1. User clicks "Analytics" in navigation
2. Frontend fetches all statistics data
3. Multiple API calls in parallel:
   - GET /api/statistics/overview
   - GET /api/statistics/by-status
   - GET /api/statistics/timeline?months=6
   - GET /api/statistics/response-rate
4. Backend calculates all metrics from database
5. Backend returns JSON responses
6. Frontend processes data for charts
7. Charts render with animations
8. User can interact with charts (hover, click)
9. User can change date range → Refetch data

**Statistics Calculations:**
```
Response Rate = (Interview Count + Offer Count) / Total Applications × 100%
Interview Rate = Interview Count / Total Applications × 100%
Offer Rate = Offer Count / Total Applications × 100%
Avg Response Time = AVG(ResponseDate - ApplicationDate) in days
Success Rate = Offer Count / (Offer Count + Rejected Count) × 100%
```

---

### **Process 5: Email Reminder System**

**Background Service Flow:**
1. Service runs every 1 hour (configurable)
2. Query database for interviews where:
   - InterviewDate.Date == Tomorrow
   - ReminderSent == false
3. For each interview found:
   - Fetch related application details
   - Generate email content from template
   - Send email via SMTP or email service (SendGrid, etc.)
   - Update ReminderSent = true
4. Log success/failure
5. Wait for next scheduled run

**Email Template Data:**
- Company Name
- Position
- Interview Date & Time
- Interview Type
- Interviewer Name
- Meeting Link (if virtual)
- Location (if onsite)
- Link to view full details

---

### **Process 6: Search & Filter Applications**

**User Flow:**
1. User types in search box or selects filter
2. Input is debounced (wait 300ms after typing stops)
3. API call with query parameters:
   - GET /api/applications?search={query}&status={status}&priority={priority}&dateFrom={date}&dateTo={date}
4. Backend queries database with WHERE clauses
5. Backend returns filtered results
6. Frontend updates list with new data
7. "No results" message if empty
8. Clear filters button appears

**Backend Query Logic:**
```sql
SELECT * FROM Applications
WHERE 
  (CompanyName LIKE '%search%' OR Position LIKE '%search%')
  AND Status IN (selected statuses)
  AND Priority IN (selected priorities)
  AND ApplicationDate BETWEEN dateFrom AND dateTo
ORDER BY ApplicationDate DESC
```

---

### **Process 7: Deleting an Application**

**User Flow:**
1. User clicks delete button on application
2. Confirmation modal appears: "Are you sure? This will delete all interviews, notes, and contacts."
3. User confirms deletion
4. API call to DELETE /api/applications/{id}
5. Backend starts transaction
6. Backend deletes all related records (cascade):
   - Interviews
   - Notes
   - Contacts
   - Documents
   - StatusHistory
7. Backend deletes application record
8. Backend commits transaction
9. Frontend removes application from list
10. Dashboard statistics update
11. Success toast: "Application deleted"

**Database Cascade Delete:**
- Ensure foreign keys have ON DELETE CASCADE
- Or manually delete related records in service layer

---

## 👥 Team Task Division

### **Developer 1: Frontend Developer**

**Responsibilities:**
- Set up React project with TypeScript
- Create routing structure with React Router
- Build all UI components and pages:
  - Dashboard/Home page
  - Applications list page
  - Application detail page
  - Forms (Add/Edit application, Add/Edit interview)
  - Notes section
  - Contacts section
- Implement global search and filters
- Create reusable components (buttons, cards, modals, badges)
- Style with Tailwind CSS
- State management with React Context/Query
- Form handling with React Hook Form
- Toast notifications
- Responsive design for mobile

**Deliverables:**
- Complete frontend application
- All pages and components functional
- Connected to backend API
- Deployed (Vercel/Netlify)

---

### **Developer 2: Backend API Developer**

**Responsibilities:**
- Set up ASP.NET Core 9 Web API project
- Design and create database schema
- Write Entity Framework Core models and DbContext
- Implement all API endpoints:
  - Applications CRUD
  - Interviews CRUD
  - Notes CRUD
  - Contacts CRUD
  - Statistics endpoints
- Business logic in service layer
- Data validation and error handling
- Global exception handling middleware
- CORS configuration
- API documentation with Swagger
- Database migrations

**Deliverables:**
- Complete REST API
- All endpoints tested and working
- Database created and seeded
- API documentation
- Deployed (Azure/AWS)

---

### **Developer 3: Calendar Integration & Email Service**

**Responsibilities:**
- Implement FullCalendar in React frontend
- Calendar page with Month/Week/Day views
- Fetch and display interviews on calendar
- Color-coding by status
- Event click to view details
- Add interview from calendar date
- Backend: Email reminder background service
- Configure SMTP or email service (SendGrid)
- Create email templates
- Implement reminder scheduling logic
- Test email delivery
- (Optional) Export calendar to iCal format

**Deliverables:**
- Fully functional calendar view
- Background service for email reminders
- Email templates created
- Email sending tested
- Integration with backend API

---

### **Developer 4: Analytics Dashboard & DevOps**

**Responsibilities:**
- Analytics page in frontend
- Integrate Chart.js or Recharts
- Create all charts and visualizations:
  - Status distribution pie chart
  - Application timeline chart
  - Response rate gauges
  - Salary insights charts
- Statistics API endpoints implementation
- Calculate all metrics correctly
- Docker setup:
  - Dockerfile for backend
  - Dockerfile for frontend
  - docker-compose.yml for full stack
- CI/CD pipeline setup (GitHub Actions)
- Environment configuration
- Database setup script
- README documentation

**Deliverables:**
- Complete analytics dashboard
- Statistics API working
- Docker containers for easy deployment
- CI/CD pipeline
- Comprehensive README with setup instructions

---

### **Timeline (4 Days)**

**Day 1: Setup & Foundation**
- All: Project setup, Git repository
- Dev 1: React project + routing + basic layout
- Dev 2: API project + database design + models
- Dev 3: Calendar library setup + email service config
- Dev 4: Docker setup + analytics library setup

**Day 2: Core Features**
- Dev 1: Applications list + forms
- Dev 2: Applications API + Interviews API
- Dev 3: Calendar view + interview display
- Dev 4: Statistics API + basic charts

**Day 3: Integration & Advanced Features**
- Dev 1: Application detail page + notes + contacts
- Dev 2: Notes API + Contacts API + validation
- Dev 3: Email templates + reminder service
- Dev 4: Complete analytics dashboard

**Day 4: Testing & Deployment**
- All: Integration testing
- Dev 1: UI polish + responsive design
- Dev 2: API testing + bug fixes
- Dev 3: Email testing + calendar polish
- Dev 4: Deployment + documentation + demo

---

## 📝 Additional Notes

### **Data Validation Rules**

**Applications:**
- CompanyName: Required, max 200 chars
- Position: Required, max 200 chars
- Status: Must be one of the 4 valid statuses
- Salary: If provided, must be positive number
- ApplicationDate: Cannot be in future

**Interviews:**
- InterviewDate: Required, must be valid datetime
- InterviewDate: Should be in future (for new interviews)
- Duration: If provided, must be 15-480 minutes
- MeetingLink: Must be valid URL format

**Notes:**
- Content: Required
- Title: Max 200 chars

### **Error Handling**

**Backend:**
- 400 Bad Request: Validation errors
- 404 Not Found: Resource doesn't exist
- 500 Internal Server Error: Unexpected errors
- Return consistent error format: `{ "error": "message", "details": [] }`

**Frontend:**
- Show error toasts for API failures
- Form validation errors under each field
- Error boundaries for component crashes
- Retry mechanism for failed requests

### **Performance Considerations**

**Backend:**
- Paginate application lists (20 per page)
- Index frequently queried fields (Status, ApplicationDate, CompanyName)
- Use async/await for all database operations
- Cache statistics for 5 minutes (use IMemoryCache)
- Lazy load related entities (interviews, notes) only when needed
- Implement database connection pooling
- Use SELECT only required columns, not SELECT *
- Batch operations where possible

**Frontend:**
- Lazy load pages with React.lazy()
- Debounce search input (300ms delay)
- Use React Query for automatic caching
- Infinite scroll for long lists instead of loading all at once
- Optimize images (compress, use appropriate formats)
- Code splitting by route
- Memoize expensive calculations with useMemo
- Virtualize long lists with react-window if needed

---

### **Security Considerations**

**Backend:**
- Input validation on all endpoints
- SQL injection prevention (use parameterized queries with EF Core)
- CORS: Only allow frontend origin
- Rate limiting: Max 100 requests per minute per IP
- XSS prevention: Sanitize user inputs
- CSRF protection (future with authentication)
- Secure file uploads: Validate file types and sizes
- Environment variables for sensitive data (connection strings, SMTP credentials)
- HTTPS only in production

**Frontend:**
- Sanitize user inputs before rendering
- Don't store sensitive data in localStorage
- Validate data on client-side before sending
- Use HTTPS for all API calls
- Content Security Policy headers

---

### **Testing Strategy**

**Backend Testing:**
- Unit tests for service layer logic
- Integration tests for API endpoints
- Test all CRUD operations
- Test validation rules
- Test business logic (status transitions, calculations)
- Test email sending (mock SMTP)
- Use xUnit or NUnit framework
- Achieve at least 70% code coverage

**Frontend Testing:**
- Unit tests for utility functions
- Component tests with React Testing Library
- Integration tests for critical user flows
- Test form validations
- Test API integration (mock API calls)
- Use Jest as test runner
- Snapshot tests for UI components

**Manual Testing:**
- Test all user flows end-to-end
- Test on different browsers (Chrome, Firefox, Safari)
- Test responsive design on mobile devices
- Test error scenarios (network failures, invalid inputs)
- Performance testing (page load times)

---

## 🚀 Deployment Guide

### **Backend Deployment Options**

**Option 1: Azure App Service**
- Create Azure SQL Database
- Create App Service (Windows/Linux)
- Configure connection strings in App Settings
- Deploy via Visual Studio or GitHub Actions
- Enable Application Insights for monitoring

**Option 2: AWS (EC2 + RDS)**
- Set up RDS SQL Server instance
- Deploy API to EC2 or Elastic Beanstalk
- Configure security groups
- Use AWS Systems Manager for secrets

**Option 3: Docker Container**
- Build Docker image
- Push to Docker Hub or Azure Container Registry
- Deploy to any container host (Azure Container Apps, AWS ECS, DigitalOcean)

### **Frontend Deployment Options**

**Option 1: Vercel (Recommended)**
- Connect GitHub repository
- Auto-deploy on push to main branch
- Set environment variables (API URL)
- Custom domain support

**Option 2: Netlify**
- Similar to Vercel
- Drag-and-drop deployment
- Serverless functions support

**Option 3: Static Hosting**
- Build production bundle: `npm run build`
- Upload to Azure Static Web Apps, AWS S3 + CloudFront, or GitHub Pages

### **Database Deployment**

**Production Setup:**
- Create SQL Server database (Azure SQL, AWS RDS, or hosted SQL Server)
- Run migrations to create schema
- Seed initial data if needed
- Set up automated backups
- Configure connection string in backend

**Connection String Format:**
```
Server=tcp:yourserver.database.windows.net,1433;
Initial Catalog=JobTrackerDb;
Persist Security Info=False;
User ID=yourusername;
Password=yourpassword;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

### **Email Service Setup**

**Option 1: SendGrid**
- Sign up for free account (100 emails/day)
- Get API key
- Configure in appsettings.json
- Use SendGrid SDK for .NET

**Option 2: SMTP (Gmail, Outlook)**
- Enable SMTP in email provider
- Get SMTP credentials
- Configure in appsettings.json
- Use MailKit library

**Option 3: AWS SES**
- Set up SES in AWS
- Verify domain or email
- Get SMTP credentials
- Lower cost for high volume

---

## 📦 Tech Stack Summary

### **Frontend**
- **Framework:** React 18+ with TypeScript
- **Routing:** React Router v6
- **State Management:** 
  - React Context (global state)
  - React Query / TanStack Query (server state)
- **Forms:** React Hook Form + Yup/Zod validation
- **HTTP Client:** Axios
- **Styling:** Tailwind CSS
- **Charts:** Recharts or Chart.js
- **Calendar:** FullCalendar
- **Icons:** Lucide React or Heroicons
- **Notifications:** React Hot Toast
- **Date Handling:** date-fns or Day.js
- **Build Tool:** Vite or Create React App

### **Backend**
- **Framework:** ASP.NET Core 9 Web API
- **ORM:** Entity Framework Core 9
- **Database:** SQL Server (Azure SQL, SQL Server Express, or LocalDB for dev)
- **Authentication:** (Future) JWT with ASP.NET Core Identity
- **Email:** MailKit or SendGrid SDK
- **Background Jobs:** IHostedService or Hangfire
- **Validation:** Data Annotations or FluentValidation
- **API Docs:** Swagger/OpenAPI (Swashbuckle)
- **Logging:** Serilog or built-in ILogger
- **Testing:** xUnit + Moq

### **DevOps & Tools**
- **Version Control:** Git + GitHub
- **Containerization:** Docker + Docker Compose
- **CI/CD:** GitHub Actions
- **API Testing:** Postman or Thunder Client
- **Database Tools:** Azure Data Studio, SQL Server Management Studio
- **Code Editor:** VS Code, Visual Studio 2022

---

## 📖 API Endpoints Reference

### **Applications**

```
GET    /api/applications
       Query params: ?status={status}&priority={priority}&search={query}&page={num}&pageSize={num}&sortBy={field}&sortOrder={asc|desc}
       Response: Array of ApplicationDto with pagination metadata

GET    /api/applications/{id}
       Response: Single ApplicationDto with all related data (interviews, notes, contacts)

POST   /api/applications
       Body: CreateApplicationDto
       Response: Created ApplicationDto (201 Created)

PUT    /api/applications/{id}
       Body: UpdateApplicationDto
       Response: 204 No Content or 404 Not Found

PATCH  /api/applications/{id}/status
       Body: { "status": "Interview" }
       Response: 204 No Content

DELETE /api/applications/{id}
       Response: 204 No Content (cascade delete related data)
```

### **Interviews**

```
GET    /api/interviews/calendar
       Query params: ?start={date}&end={date}
       Response: Array of InterviewDto for calendar view

GET    /api/applications/{appId}/interviews
       Response: Array of InterviewDto for specific application

POST   /api/applications/{appId}/interviews
       Body: CreateInterviewDto
       Response: Created InterviewDto (201 Created)

PUT    /api/interviews/{id}
       Body: UpdateInterviewDto
       Response: 204 No Content

DELETE /api/interviews/{id}
       Response: 204 No Content
```

### **Notes**

```
GET    /api/applications/{appId}/notes
       Response: Array of NoteDto

POST   /api/applications/{appId}/notes
       Body: CreateNoteDto
       Response: Created NoteDto (201 Created)

PUT    /api/notes/{id}
       Body: UpdateNoteDto
       Response: 204 No Content

DELETE /api/notes/{id}
       Response: 204 No Content
```

### **Contacts**

```
GET    /api/applications/{appId}/contacts
       Response: Array of ContactDto

POST   /api/applications/{appId}/contacts
       Body: CreateContactDto
       Response: Created ContactDto (201 Created)

PUT    /api/contacts/{id}
       Body: UpdateContactDto
       Response: 204 No Content

DELETE /api/contacts/{id}
       Response: 204 No Content
```

### **Statistics**

```
GET    /api/statistics/overview
       Response: {
         totalApplications: number,
         activeApplications: number,
         interviewCount: number,
         offerCount: number,
         rejectedCount: number,
         responseRate: number,
         averageResponseTime: number
       }

GET    /api/statistics/by-status
       Response: {
         "Applied": 10,
         "Interview": 5,
         "Offer": 2,
         "Rejected": 3
       }

GET    /api/statistics/timeline
       Query params: ?months={number}
       Response: Array of { month: string, counts: { Applied: num, Interview: num, ... } }

GET    /api/statistics/response-rate
       Response: {
         interviewRate: number,
         offerRate: number,
         rejectionRate: number
       }
```

---

## 🎨 UI/UX Design Guidelines

### **Color Scheme**

**Status Colors:**
- Applied: Blue (#3B82F6)
- Interview: Yellow/Orange (#F59E0B)
- Offer: Green (#10B981)
- Rejected: Red (#EF4444)

**Priority Colors:**
- High: Red (#DC2626)
- Medium: Orange (#F97316)
- Low: Gray (#6B7280)

**UI Colors:**
- Primary: Blue (#2563EB)
- Secondary: Gray (#64748B)
- Success: Green (#059669)
- Warning: Amber (#D97706)
- Error: Red (#DC2626)
- Background: White/Gray-50
- Text: Gray-900/Gray-700

### **Typography**

- **Headings:** Inter, SF Pro Display, or system-ui
- **Body:** Inter, Roboto, or system-ui
- **Monospace:** JetBrains Mono, Fira Code (for code/data)

**Font Sizes:**
- h1: 2.5rem (40px)
- h2: 2rem (32px)
- h3: 1.5rem (24px)
- h4: 1.25rem (20px)
- body: 1rem (16px)
- small: 0.875rem (14px)

### **Component Patterns**

**Application Card:**
- Company logo/icon
- Company name (bold, large)
- Position title
- Location with icon
- Status badge (colored)
- Priority indicator (small dot)
- Application date (relative: "2 days ago")
- Quick actions: View, Edit, Delete

**Status Badge:**
- Rounded pill shape
- Bold text
- Colored background (semi-transparent)
- Small icon before text

**Calendar Event:**
- Time displayed
- Company name + Position (truncated if long)
- Color stripe matching application status
- Hover: Show full details tooltip

**Statistics Card:**
- Large number at top
- Label below number
- Icon on right side
- Subtle background color
- Trend indicator (↑↓) if applicable

### **Responsive Breakpoints**

- Mobile: < 640px (1 column)
- Tablet: 640px - 1024px (2 columns)
- Desktop: > 1024px (3-4 columns)

### **Loading States**

- Use skeleton loaders instead of spinners
- Skeleton should match actual content layout
- Show loading state for API calls > 300ms
- Optimistic updates where appropriate

---

## 🐛 Common Issues & Solutions

### **Backend Issues**

**Issue:** Database connection fails
**Solution:** Check connection string, ensure SQL Server is running, verify firewall rules

**Issue:** CORS errors in browser
**Solution:** Add frontend URL to CORS policy in Program.cs

**Issue:** Emails not sending
**Solution:** Check SMTP credentials, enable "Less secure apps" for Gmail, verify SendGrid API key

**Issue:** Background service not running
**Solution:** Ensure service is registered in Program.cs with AddHostedService

### **Frontend Issues**

**Issue:** API calls failing with 404
**Solution:** Verify API base URL is correct, check if backend is running

**Issue:** Calendar not displaying events
**Solution:** Check date format sent to API, verify interview data structure matches FullCalendar format

**Issue:** Form validation not working
**Solution:** Ensure Yup/Zod schema is properly defined, check React Hook Form configuration

**Issue:** Charts not rendering
**Solution:** Verify data format matches chart requirements, check for null/undefined values

---

## ✅ Pre-Launch Checklist

### **Backend**
- [ ] All API endpoints tested and working
- [ ] Database schema created with proper indexes
- [ ] Validation on all inputs
- [ ] Error handling implemented
- [ ] CORS configured correctly
- [ ] Environment variables set
- [ ] Email service configured and tested
- [ ] Background service running
- [ ] API documentation complete (Swagger)
- [ ] Unit tests written and passing

### **Frontend**
- [ ] All pages responsive on mobile/tablet/desktop
- [ ] Forms validate correctly
- [ ] Loading states shown during API calls
- [ ] Error messages display for failures
- [ ] Search and filters working
- [ ] Calendar displaying correctly
- [ ] Charts rendering with correct data
- [ ] Navigation working on all routes
- [ ] 404 page for invalid routes
- [ ] Accessibility: keyboard navigation, ARIA labels

### **Integration**
- [ ] Frontend connects to backend API
- [ ] Data flows correctly between frontend and backend
- [ ] Real-time updates work
- [ ] File uploads working (if implemented)
- [ ] Email reminders sending correctly

### **DevOps**
- [ ] Docker Compose file working
- [ ] Database migrations running successfully
- [ ] Environment variables documented
- [ ] README with setup instructions
- [ ] CI/CD pipeline configured
- [ ] Production deployment tested

### **Documentation**
- [ ] README.md complete with:
  - [ ] Project description
  - [ ] Features list
  - [ ] Tech stack
  - [ ] Setup instructions (local development)
  - [ ] API documentation link
  - [ ] Screenshots/demo
  - [ ] Contributing guidelines
  - [ ] License
- [ ] Code comments for complex logic
- [ ] API endpoint documentation
- [ ] Database schema diagram

---

## 🎯 Success Metrics

**For Hacktoberfest:**
- Project repository public on GitHub
- At least 5 good-first-issue tags created
- hacktoberfest and hacktoberfest-2025 topics added
- Clear CONTRIBUTING.md file
- Code of conduct added
- MIT or Apache 2.0 license

**For Project Quality:**
- All MVP features implemented and working
- No critical bugs
- Responsive on all devices
- API response time < 500ms
- Page load time < 3 seconds
- Passes basic accessibility checks
- At least 60% code coverage in tests

---

## 📚 Learning Resources

**ASP.NET Core:**
- Official Docs: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- Background Services: https://docs.microsoft.com/aspnet/core/fundamentals/host/hosted-services

**React:**
- Official Docs: https://react.dev
- React Router: https://reactrouter.com
- React Query: https://tanstack.com/query/latest
- React Hook Form: https://react-hook-form.com

**Other:**
- FullCalendar: https://fullcalendar.io/docs/react
- Tailwind CSS: https://tailwindcss.com/docs
- Chart.js: https://www.chartjs.org/docs
- SendGrid .NET: https://github.com/sendgrid/sendgrid-csharp

---

## 🎉 Bonus Features (If Time Allows)

1. **Browser Extension:** Quick-add jobs from LinkedIn/Indeed
2. **Export to PDF:** Generate resume-ready application history
3. **AI Integration:** Auto-fill job details from URL using AI
4. **Mobile App:** React Native version
5. **Notifications:** Browser push notifications for reminders
6. **Collaboration:** Share applications with mentors/friends
7. **Templates:** Save application templates for quick entry
8. **Interview Prep:** Store common interview questions and answers
9. **Salary Negotiation Tool:** Suggest salary ranges based on market data
10. **Job Board Integration:** Import jobs directly from APIs

---

## 💡 Tips for Success

1. **Start Simple:** Build MVP first, add features later
2. **Communicate Daily:** Daily standup meetings (even 15 mins)
3. **Use Git Properly:** Feature branches, pull requests, code reviews
4. **Test Early:** Don't wait until the end to test
5. **Document as You Go:** Write README and comments while building
6. **Deploy Early:** Deploy Day 1, iterate frequently
7. **Ask for Help:** Don't struggle alone, help each other
8. **Stay Focused:** Avoid feature creep, stick to the plan
9. **Celebrate Wins:** Acknowledge progress, no matter how small
10. **Have Fun:** Enjoy the process and learn together!

---

## 📞 Support & Questions

For this project implementation, focus on:
1. Clear communication within the team
2. Well-defined tasks and responsibilities
3. Regular integration testing
4. Good documentation
5. Clean, maintainable code

**Good luck with your Hacktoberfest 2025 project! 🚀**