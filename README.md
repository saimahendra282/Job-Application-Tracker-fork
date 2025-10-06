# 🚀 Job Application Tracker - Hacktoberfest 2025

[![Hacktoberfest](https://img.shields.io/badge/Hacktoberfest-2025-orange?style=for-the-badge&logo=github)](https://hacktoberfest.com/)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15.0-black?style=for-the-badge&logo=next.js)](https://nextjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue?style=for-the-badge&logo=typescript)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

A comprehensive job application tracking system built for **Hacktoberfest 2025**! This project helps job seekers organize, track, and manage their job applications efficiently with a modern .NET 9.0 Web API backend and a Next.js 15 frontend.

## 🎯 Project Overview

The Job Application Tracker is designed to streamline the job search process by providing:

- **📊 Dashboard**: Visual overview of application statuses and progress
- **📝 Application Management**: Add, edit, and track job applications
- **📅 Interview Scheduling**: Manage interview dates and follow-ups
- **📈 Analytics**: Track success rates and application trends
- **🔐 Secure Authentication**: JWT-based authentication system
- **📱 Responsive Design**: Works seamlessly on desktop and mobile

## 🏗️ Architecture

### Backend (.NET 9.0 Web API)
- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Caching**: Redis for performance optimization
- **Logging**: Serilog with multiple sinks
- **Documentation**: Swagger/OpenAPI with Scalar UI
- **Background Jobs**: Hangfire for scheduled tasks
- **Message Queue**: MassTransit with RabbitMQ

### Frontend (Next.js 15)
- **Framework**: Next.js 15 with App Router
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **State Management**: React Context + useReducer
- **HTTP Client**: Axios
- **UI Components**: Custom components with Radix UI
- **Authentication**: JWT token management

## 🚀 Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Redis](https://redis.io/download) (optional, for caching)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (optional, for message queuing)

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/ravindu0823/Job-Application-Tracker.git
   cd Job-Application-Tracker
   ```

2. **Navigate to the API project**
   ```bash
   cd LiterateWinnerApi/LiterateWinnerApi
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Update connection strings**
   - Update `appsettings.json` and `appsettings.Development.json` with your database connection string
   - Configure Redis connection (optional)
   - Set up RabbitMQ connection (optional)

5. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

6. **Start the API**
   ```bash
   dotnet run
   ```

   The API will be available at:
   - **API**: `https://localhost:7001` or `http://localhost:5001`
   - **Swagger UI**: `https://localhost:7001/swagger`
   - **Scalar API Docs**: `https://localhost:7001/scalar/v1`

### Frontend Setup

1. **Navigate to the frontend project**
   ```bash
   cd job-application-tracker-frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   # or
   yarn install
   # or
   pnpm install
   ```

3. **Configure environment variables**
   ```bash
   cp .env.example .env.local
   ```
   
   Update `.env.local` with your API endpoint:
   ```env
   NEXT_PUBLIC_API_URL=http://localhost:5001
   NEXT_PUBLIC_API_BASE_URL=https://localhost:7001
   ```

4. **Start the development server**
   ```bash
   npm run dev
   # or
   yarn dev
   # or
   pnpm dev
   ```

   The frontend will be available at `http://localhost:3000`

## 🧪 Testing

This project includes comprehensive testing to ensure code quality and reliability.

### Backend Testing (.NET)
[![Backend Coverage](https://img.shields.io/badge/Coverage-70%2B%25-brightgreen?style=for-the-badge)](https://github.com/priyankeshh/Job-Application-Tracker)

- **Unit Tests**: Service layer testing with xUnit and Moq
- **Integration Tests**: API endpoint testing with TestServer
- **Performance Tests**: Benchmarking with BenchmarkDotNet
- **Security Tests**: Authentication and token validation

**Run Backend Tests:**
```bash
cd LiterateWinnerApi
dotnet test --configuration Release
```

**Run Performance Benchmarks:**
```bash
cd LiterateWinnerApi
dotnet run --project JobApplicationTrackerApi.Tests --configuration Release
```

### Frontend Testing (Next.js)
[![Frontend Coverage](https://img.shields.io/badge/Coverage-71.8%25-brightgreen?style=for-the-badge)](https://github.com/priyankeshh/Job-Application-Tracker)

- **Unit Tests**: Component testing with Vitest and React Testing Library
- **Integration Tests**: User interaction testing
- **Coverage**: V8 coverage provider

**Run Frontend Tests:**
```bash
cd frontend
npm run test:run
```

**Run with Coverage:**
```bash
cd frontend
npm run test:coverage
```

### CI/CD
- **GitHub Actions**: Automated testing on every push and PR
- **Coverage Thresholds**: Minimum 70% coverage required
- **Multi-stage Pipeline**: Backend, frontend, and Docker build validation

## 📁 Project Structure

```
job-application-tracker/
├── 📁 LiterateWinnerApi/                    # .NET 9.0 Web API
│   ├── 📁 LiterateWinnerApi/
│   │   ├── 📁 Controllers/                  # API Controllers
│   │   ├── 📁 DTO/                         # Data Transfer Objects
│   │   │   └── 📁 Auth/                    # Authentication DTOs
│   │   ├── 📁 Enum/                        # Enumerations
│   │   ├── 📁 Extensions/                  # Extension methods
│   │   ├── 📁 Infrastructure/              # Infrastructure layer
│   │   │   ├── 📁 Middleware/              # Custom middleware
│   │   │   ├── 📁 Options/                 # Configuration options
│   │   │   └── 📁 Web/                     # Web-specific infrastructure
│   │   ├── 📁 Persistence/                 # Data access layer
│   │   │   ├── 📁 DefaultContext/          # Main database context
│   │   │   │   ├── 📁 Entity/              # Database entities
│   │   │   │   └── 📁 Migrations/          # EF Core migrations
│   │   │   └── 📁 IdentityContext/         # Identity context
│   │   ├── 📁 Services/                    # Business logic services
│   │   │   ├── 📁 CacheService/            # Caching service
│   │   │   ├── 📁 ConcurrencyService/      # Concurrency handling
│   │   │   ├── 📁 IdentityErrorHandlerService/
│   │   │   ├── 📁 IdentityService/         # Identity management
│   │   │   ├── 📁 MonitoringService/       # Application monitoring
│   │   │   └── 📁 TokenService/            # JWT token management
│   │   ├── 📁 SignalR/                     # Real-time communication
│   │   ├── 📁 Startup/                     # Application startup
│   │   ├── 📁 Utils/                       # Utility classes
│   │   ├── 📄 Program.cs                   # Application entry point
│   │   └── 📄 JobApplicationTrackerApi.csproj
│   └── 📄 JobApplicationTrackerApi.sln
├── 📁 job-application-tracker-frontend/     # Next.js 15 Frontend
│   ├── 📁 app/                             # Next.js App Router
│   ├── 📁 components/                      # React components
│   ├── 📁 lib/                             # Utility libraries
│   ├── 📁 hooks/                           # Custom React hooks
│   ├── 📁 context/                         # React context providers
│   ├── 📁 types/                           # TypeScript type definitions
│   ├── 📁 styles/                          # Global styles
│   ├── 📄 package.json
│   └── 📄 next.config.js
└── 📄 README.md
```

## 🛠️ Technology Stack

### Backend Technologies
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server** - Primary database
- **JWT Authentication** - Secure token-based authentication
- **AutoMapper** - Object-to-object mapping
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **Hangfire** - Background job processing
- **MassTransit** - Message queuing
- **Redis** - Caching and session storage
- **Polly** - Resilience and transient-fault-handling

### Frontend Technologies
- **Next.js 15** - React framework with App Router
- **TypeScript** - Type-safe JavaScript
- **Tailwind CSS** - Utility-first CSS framework
- **Axios** - HTTP client for API calls
- **React Hook Form** - Form handling
- **Zustand** - State management
- **Radix UI** - Accessible UI components
- **Lucide React** - Icon library
- **Framer Motion** - Animation library

## 🔧 API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh JWT token
- `POST /api/auth/logout` - User logout

### Job Applications
- `GET /api/job-applications` - Get all job applications
- `GET /api/job-applications/{id}` - Get specific job application
- `POST /api/job-applications` - Create new job application
- `PUT /api/job-applications/{id}` - Update job application
- `DELETE /api/job-applications/{id}` - Delete job application

### Analytics
- `GET /api/analytics/overview` - Get application statistics
- `GET /api/analytics/status-distribution` - Get status distribution
- `GET /api/analytics/monthly-trends` - Get monthly application trends

## 🎨 Features

### Core Features
- ✅ **User Authentication & Authorization**
- ✅ **Job Application CRUD Operations**
- ✅ **Application Status Tracking**
- ✅ **Interview Scheduling**
- ✅ **Company Management**
- ✅ **Notes & Comments**
- ✅ **File Attachments**
- ✅ **Search & Filtering**
- ✅ **Data Export (PDF/Excel)**
- ✅ **Real-time Notifications**

### Advanced Features
- 🔄 **Background Job Processing**
- 📊 **Analytics Dashboard**
- 🔔 **Email Notifications**
- 📱 **Mobile Responsive Design**
- 🌙 **Dark/Light Theme**
- 🔍 **Advanced Search**
- 📈 **Progress Tracking**
- 🏷️ **Tagging System**

## 🤝 Contributing to Hacktoberfest 2025

We welcome contributions for **Hacktoberfest 2025**! Here's how you can contribute:

### Getting Started
1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/amazing-feature`
3. **Make your changes**
4. **Commit your changes**: `git commit -m 'Add some amazing feature'`
5. **Push to the branch**: `git push origin feature/amazing-feature`
6. **Open a Pull Request**

### Contribution Guidelines
- Follow the existing code style and conventions
- Write meaningful commit messages
- Add tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

### Good First Issues
Look for issues labeled with:
- `good first issue` - Perfect for newcomers
- `hacktoberfest` - Hacktoberfest specific tasks
- `help wanted` - Community help needed
- `documentation` - Documentation improvements

### Areas for Contribution
- 🐛 **Bug Fixes**
- ✨ **New Features**
- 📚 **Documentation**
- 🧪 **Tests**
- 🎨 **UI/UX Improvements**
- ⚡ **Performance Optimizations**
- 🔒 **Security Enhancements**

## 🐛 Bug Reports & Feature Requests

Found a bug or have a feature request? Please open an issue with:
- Clear description of the problem/request
- Steps to reproduce (for bugs)
- Expected vs actual behavior
- Screenshots (if applicable)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **DigitalOcean** and **MLH** for organizing Hacktoberfest 2025
- **Microsoft** for .NET and ASP.NET Core
- **Vercel** for Next.js framework
- **All contributors** who help make this project better

## 📞 Support

- 📧 **Email**: guestpc87@gmail.com
- 🌐 **Website**: [ravinduperera.vercel.app](https://ravinduperera.vercel.app/)
- 📖 **Documentation**: [Wiki](https://github.com/ravindu0823/Job-Application-Tracker/wiki)
- 🐛 **Issues**: [GitHub Issues](https://github.com/ravindu0823/Job-Application-Tracker/issues)

---

<div align="center">

**⭐ Star this repository if you found it helpful!**

Made with ❤️ for **Hacktoberfest 2025**

[![DigitalOcean](https://img.shields.io/badge/Powered%20by-DigitalOcean-blue?style=for-the-badge)](https://www.digitalocean.com/)
[![MLH](https://img.shields.io/badge/Supported%20by-MLH-red?style=for-the-badge)](https://mlh.io/)

</div>
