# Job Application Tracker API - Configuration Guide

## Overview

This guide explains how to configure the Job Application Tracker API for different environments.

## Quick Start

1. Copy `appsettings.sample.json` to `appsettings.json`
2. Update the configuration values according to your environment
3. Run the application

## Configuration Sections

### 1. Database Configuration

#### Connection Strings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER_NAME\\SQLEXPRESS;Initial Catalog=JobApplicationTracker;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False",
    "IdentityConnection": "Data Source=YOUR_SERVER_NAME\\SQLEXPRESS;Initial Catalog=JobApplicationTrackerIdentity;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
  }
}
```

**Required Changes:**

- Replace `YOUR_SERVER_NAME` with your SQL Server instance name
- Replace `YOUR_USERNAME` with your SQL Server username
- Replace `YOUR_PASSWORD` with your SQL Server password
- Update database names if needed

### 2. JWT Authentication

```json
{
  "Jwt": {
    "Issuer": "JobApplicationTrackerIssuer",
    "Audience": "JobApplicationTrackerManager", 
    "SecurityKey": "YOUR_JWT_SECURITY_KEY_HERE_AT_LEAST_64_CHARACTERS_LONG_FOR_SECURITY_PURPOSES",
    "AccessTokenLifetime": "0.01:00:00.0",
    "RefreshTokenLifetime": "3.00:00:00.0"
  }
}
```

**Required Changes:**

- Generate a secure JWT security key (at least 64 characters)
- Update Issuer and Audience if needed
- Adjust token lifetimes as required

**Generate JWT Security Key:**

```csharp
// Use this code to generate a secure key
var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
Console.WriteLine(key);
```

### 3. Email Configuration

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Job Application Tracker",
    "EnableSsl": true
  }
}
```

**For Gmail:**

1. Enable 2-Factor Authentication
2. Generate an App Password
3. Use the App Password in `SmtpPassword`

**For Other Providers:**

- Update `SmtpServer` and `SmtpPort` accordingly
- Adjust `EnableSsl` based on provider requirements

### 4. File Storage

```json
{
  "FileStorage": {
    "StorageType": "Local",
    "LocalPath": "wwwroot/uploads",
    "MaxFileSize": 10485760,
    "AllowedExtensions": [".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png"]
  }
}
```

**Options:**

- `StorageType`: "Local" or "AzureBlob" (future implementation)
- `LocalPath`: Directory for file uploads
- `MaxFileSize`: Maximum file size in bytes (10MB default)
- `AllowedExtensions`: File types allowed for upload

### 5. CORS Configuration

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:3001",
      "https://your-frontend-domain.com"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS"],
    "AllowedHeaders": ["Content-Type", "Authorization", "X-Requested-With"]
  }
}
```

**Required Changes:**

- Add your frontend domain(s) to `AllowedOrigins`
- Update methods and headers as needed

### 6. Logging Configuration

The application uses Serilog for structured logging with multiple sinks:

- **Console**: For development debugging
- **Seq**: For log aggregation (optional)
- **MSSqlServer**: For database logging
- **File**: For file-based logging

**Seq Setup (Optional):**

1. Install Seq: `docker run -d --name seq -p 5341:80 datalust/seq:latest`
2. Access Seq UI at `http://localhost:5341`

### 7. Rate Limiting

```json
{
  "RateLimiting": {
    "EnableRateLimiting": true,
    "RequestsPerMinute": 100,
    "BurstLimit": 20
  }
}
```

**Configuration:**

- `EnableRateLimiting`: Enable/disable rate limiting
- `RequestsPerMinute`: Maximum requests per minute per IP
- `BurstLimit`: Maximum burst requests allowed

### 8. Caching

```json
{
  "Cache": {
    "DefaultExpirationMinutes": 30,
    "StatisticsCacheMinutes": 5,
    "EnableRedis": false,
    "RedisConnectionString": "localhost:6379"
  }
}
```

**Options:**

- `DefaultExpirationMinutes`: Default cache expiration
- `StatisticsCacheMinutes`: Statistics cache duration
- `EnableRedis`: Use Redis for distributed caching
- `RedisConnectionString`: Redis connection string

### 9. Background Services

```json
{
  "BackgroundServices": {
    "EmailReminderService": {
      "Enabled": true,
      "CheckIntervalMinutes": 60,
      "ReminderHoursBeforeInterview": 24
    }
  }
}
```

**Configuration:**

- `Enabled`: Enable/disable email reminders
- `CheckIntervalMinutes`: How often to check for reminders
- `ReminderHoursBeforeInterview`: Hours before interview to send reminder

## Environment-Specific Configuration

### Development

- Use `appsettings.Development.json` for development-specific settings
- Enable detailed logging
- Use local database
- Disable rate limiting

### Production

- Use `appsettings.Production.json` for production settings
- Use production database
- Enable rate limiting
- Use secure JWT keys
- Configure proper CORS origins

## Security Considerations

1. **Never commit `appsettings.json` to version control**
2. **Use User Secrets for development:**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   dotnet user-secrets set "Jwt:SecurityKey" "your-jwt-key"
   ```

3. **Use Azure Key Vault or similar for production secrets**

4. **Rotate JWT keys regularly**

5. **Use strong passwords for database connections**

## Database Setup

1. **Create Databases:**
   ```sql
   CREATE DATABASE JobApplicationTracker;
   CREATE DATABASE JobApplicationTrackerIdentity;
   ```

2. **Run Migrations:**
   ```bash
   dotnet ef database update --context DefaultContext
   dotnet ef database update --context IdentityContext
   ```

3. **Seed Data (Optional):**
   ```bash
   # The application will automatically seed data on first run
   ```

## Troubleshooting

### Common Issues

1. **Database Connection Failed:**
    - Check connection string
    - Verify SQL Server is running
    - Check firewall settings

2. **JWT Authentication Failed:**
    - Verify JWT configuration
    - Check security key length
    - Ensure proper token format

3. **Email Not Sending:**
    - Check SMTP configuration
    - Verify email credentials
    - Check firewall for port 587

4. **CORS Errors:**
    - Update CORS configuration
    - Check frontend URL
    - Verify allowed methods

### Logs Location

- **Console**: Application output
- **File**: `Logs/log-{date}.txt`
- **Database**: `Logs` table
- **Seq**: `http://localhost:5341` (if enabled)

## Support

For additional help:

1. Check the application logs
2. Review the API documentation at `/api/home/documentation`
3. Check the health endpoint at `/health`
4. Review the database status at `/api/home/database-status`
