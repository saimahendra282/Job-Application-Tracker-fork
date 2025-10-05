# Job Application Tracker - Project Rules

## Project Overview

This is a full-stack job application tracking system with:

-   **Frontend**: Next.js 14+ with TypeScript, Tailwind CSS, and shadcn/ui
-   **Backend**: .NET 9 Web API with Entity Framework Core
-   **Database**: SQL Server with Identity for authentication

## Architecture Guidelines

### General Principles

-   Follow clean architecture patterns
-   Maintain separation of concerns between frontend and backend
-   Use consistent naming conventions across all components
-   Implement proper error handling and logging
-   Follow security best practices for authentication and authorization

### Code Organization

-   Keep related functionality grouped together
-   Use meaningful file and folder names
-   Maintain consistent import/using statement organization
-   Follow the established folder structure for each project

### Documentation

-   Update README files when adding new features
-   Document API endpoints and their usage
-   Include code comments for complex business logic
-   Maintain changelog for version tracking

### Testing

-   Write unit tests for business logic
-   Include integration tests for API endpoints
-   Test frontend components and user interactions
-   Maintain test coverage above 80%

### Security

-   Never commit sensitive information (API keys, passwords, etc.)
-   Use environment variables for configuration
-   Implement proper input validation
-   Follow OWASP security guidelines

### Performance

-   Optimize database queries
-   Implement proper caching strategies
-   Use lazy loading where appropriate
-   Monitor and optimize bundle sizes

### Git Workflow

-   Use feature branches for new development
-   Write descriptive commit messages
-   Create pull requests for code review
-   Keep main branch stable and deployable

## Technology Stack

-   **Frontend**: Next.js, TypeScript, Tailwind CSS, shadcn/ui, React Query
-   **Backend**: .NET 9, Entity Framework Core, AutoMapper, SignalR
-   **Database**: SQL Server
-   **Authentication**: ASP.NET Core Identity with JWT
-   **Containerization**: Docker and Docker Compose
-   **Version Control**: Git with GitHub

## Development Environment

-   Use the provided Docker Compose setup for local development
-   Follow the setup instructions in each project's README
-   Use the recommended IDE extensions and configurations
-   Maintain consistent code formatting across the project
