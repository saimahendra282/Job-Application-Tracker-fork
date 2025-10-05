# .NET Web API Development Rules

## Technology Stack

-   **Framework**: .NET 9 Web API
-   **Language**: C# 12
-   **ORM**: Entity Framework Core
-   **Database**: SQL Server
-   **Authentication**: ASP.NET Core Identity with JWT
-   **Mapping**: AutoMapper
-   **Real-time**: SignalR
-   **Validation**: FluentValidation
-   **Documentation**: Swagger/OpenAPI

## Code Style & Standards

### C# Coding Standards

-   Follow Microsoft C# coding conventions
-   Use PascalCase for public members, classes, and methods
-   Use camelCase for private fields and local variables
-   Use meaningful and descriptive names
-   Prefer explicit types over `var` when type is not obvious
-   Use `readonly` for fields that are only set in constructors
-   Use `const` for compile-time constants

### Architecture Patterns

-   Follow Clean Architecture principles
-   Implement Repository pattern for data access
-   Use Dependency Injection throughout the application
-   Implement CQRS pattern for complex operations
-   Use MediatR for command/query handling
-   Follow SOLID principles

### Project Structure

```
LiterateWinnerApi/
├── Controllers/          # API controllers
├── Services/            # Business logic services
├── DTO/                 # Data Transfer Objects
├── Persistence/         # Data access layer
├── Infrastructure/      # Cross-cutting concerns
├── Extensions/          # Extension methods
├── Mappings/           # AutoMapper profiles
├── Enum/               # Enumerations
├── Utils/              # Utility classes
└── SignalR/            # Real-time communication
```

### Controller Guidelines

-   Keep controllers thin - delegate to services
-   Use proper HTTP status codes
-   Implement proper error handling
-   Use async/await for all I/O operations
-   Validate input using Data Annotations or FluentValidation
-   Return appropriate response types
-   Use proper routing attributes

### Service Layer

-   Implement business logic in services
-   Use interfaces for all services
-   Register services in DI container
-   Handle exceptions appropriately
-   Use proper logging
-   Implement caching where appropriate

### Data Access

-   Use Entity Framework Core for data access
-   Implement proper DbContext management
-   Use async methods for database operations
-   Implement proper error handling
-   Use transactions for complex operations
-   Optimize queries to avoid N+1 problems

### DTOs and Models

-   Use DTOs for API communication
-   Separate input and output DTOs
-   Use AutoMapper for object mapping
-   Implement proper validation
-   Use record types for immutable DTOs
-   Follow naming conventions

### Authentication & Authorization

-   Use ASP.NET Core Identity
-   Implement JWT token authentication
-   Use role-based authorization
-   Implement proper password policies
-   Use secure token storage
-   Implement refresh token mechanism

### Error Handling

-   Use global exception handling middleware
-   Implement proper error responses
-   Log errors appropriately
-   Use Problem Details for error responses
-   Handle validation errors gracefully
-   Implement proper error codes

### Validation

-   Use Data Annotations for simple validation
-   Use FluentValidation for complex validation
-   Validate input at multiple levels
-   Return meaningful validation messages
-   Implement custom validation attributes

### Logging

-   Use structured logging with Serilog
-   Log important business events
-   Include correlation IDs
-   Log performance metrics
-   Implement proper log levels
-   Use log aggregation

### Performance

-   Use async/await for I/O operations
-   Implement proper caching strategies
-   Optimize database queries
-   Use connection pooling
-   Implement response compression
-   Monitor performance metrics

### Security

-   Implement proper input validation
-   Use HTTPS in production
-   Implement CORS properly
-   Use secure headers
-   Implement rate limiting
-   Follow OWASP guidelines

### Testing

-   Write unit tests for business logic
-   Write integration tests for API endpoints
-   Use test databases for testing
-   Mock external dependencies
-   Maintain test coverage above 80%
-   Use proper test data

### Configuration

-   Use appsettings.json for configuration
-   Use environment-specific settings
-   Use Options pattern for configuration
-   Validate configuration on startup
-   Use secure configuration storage
-   Document configuration options

### Database

-   Use Entity Framework migrations
-   Implement proper database seeding
-   Use connection strings from configuration
-   Implement database health checks
-   Use proper indexing strategies
-   Monitor database performance

### Real-time Communication

-   Use SignalR for real-time features
-   Implement proper connection management
-   Handle connection errors gracefully
-   Use appropriate hub methods
-   Implement proper authorization
-   Monitor connection metrics

### API Documentation

-   Use Swagger/OpenAPI for documentation
-   Document all endpoints
-   Include examples in documentation
-   Use proper XML comments
-   Generate client SDKs
-   Maintain up-to-date documentation

### Deployment

-   Use Docker for containerization
-   Implement proper health checks
-   Use environment-specific configurations
-   Implement proper logging
-   Monitor application metrics
-   Use CI/CD pipelines

### Development Workflow

-   Use proper Git workflow
-   Write meaningful commit messages
-   Use feature branches
-   Implement code reviews
-   Use static analysis tools
-   Follow coding standards
