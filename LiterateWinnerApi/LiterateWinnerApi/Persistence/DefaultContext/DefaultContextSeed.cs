using JobApplicationTrackerApi.Enum;
using JobApplicationTrackerApi.Persistence.DefaultContext.Entity;

namespace JobApplicationTrackerApi.Persistence.DefaultContext;

/// <summary>
/// Seeds the database with initial data for testing
/// </summary>
public static class DefaultContextSeed
{
    /// <summary>
    /// Seeds the database with sample data
    /// </summary>
    /// <param name="context">The database context</param>
    public static async Task SeedAsync(
        DefaultContext context,
        ILogger<DefaultContext> logger
    )
    {
        try
        {
            logger.LogInformation("Seeding default context database...");

            // Check if data already exists
            if (context.Applications.Any())
            {
                return;
            }

            // Create sample applications
            var applications = new List<Application>
            {
                new()
                {
                    Id = 1,
                    UserId = "1",
                    CompanyName = "Microsoft",
                    Position = "Senior Software Engineer",
                    Location = "Seattle, WA",
                    JobUrl = "https://careers.microsoft.com/job/12345",
                    ApplicationStatus = ApplicationStatus.Interview,
                    Priority = ApplicationPriority.High,
                    SalaryMin = 120000,
                    SalaryMax = 150000,
                    ApplicationDate = DateTime.UtcNow.AddDays(-10),
                    ResponseDate = DateTime.UtcNow.AddDays(-5),
                    JobDescription = "We are looking for a Senior Software Engineer to join our team...",
                    Requirements = "5+ years of experience with C# and .NET, Azure experience preferred",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    Id = 2,
                    UserId = "1",
                    CompanyName = "Google",
                    Position = "Software Engineer III",
                    Location = "Mountain View, CA",
                    JobUrl = "https://careers.google.com/job/67890",
                    ApplicationStatus = ApplicationStatus.Applied,
                    Priority = ApplicationPriority.Medium,
                    SalaryMin = 130000,
                    SalaryMax = 160000,
                    ApplicationDate = DateTime.UtcNow.AddDays(-7),
                    JobDescription = "Join Google as a Software Engineer III...",
                    Requirements = "3+ years of experience with Python, Go, or Java",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-7)
                },
                new()
                {
                    Id = 3,
                    UserId = "1",
                    CompanyName = "Amazon",
                    Position = "SDE II",
                    Location = "Seattle, WA",
                    JobUrl = "https://amazon.jobs/en/jobs/11111",
                    ApplicationStatus = ApplicationStatus.Offer,
                    Priority = ApplicationPriority.High,
                    Salary = 140000,
                    ApplicationDate = DateTime.UtcNow.AddDays(-20),
                    ResponseDate = DateTime.UtcNow.AddDays(-15),
                    OfferDate = DateTime.UtcNow.AddDays(-3),
                    JobDescription = "Amazon is looking for a Software Development Engineer II...",
                    Requirements = "4+ years of experience with Java or C++",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-20)
                },
                new()
                {
                    Id = 4,
                    UserId = "2",
                    CompanyName = "Netflix",
                    Position = "Senior Full Stack Engineer",
                    Location = "Los Gatos, CA",
                    JobUrl = "https://jobs.netflix.com/jobs/22222",
                    ApplicationStatus = ApplicationStatus.Rejected,
                    Priority = ApplicationPriority.Medium,
                    SalaryMin = 150000,
                    SalaryMax = 180000,
                    ApplicationDate = DateTime.UtcNow.AddDays(-15),
                    ResponseDate = DateTime.UtcNow.AddDays(-5),
                    JobDescription = "Netflix is seeking a Senior Full Stack Engineer...",
                    Requirements = "6+ years of experience with React, Node.js, and AWS",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-15)
                }
            };

            context.Applications.AddRange(applications);

            // Create sample interviews
            var interviews = new List<Interview>
            {
                new()
                {
                    Id = 1,
                    ApplicationId = 1,
                    InterviewDate = DateTime.UtcNow.AddDays(3),
                    InterviewType = InterviewType.Video,
                    Duration = 60,
                    InterviewerName = "Sarah Johnson",
                    InterviewerPosition = "Hiring Manager",
                    Location = "Virtual",
                    MeetingLink = "https://teams.microsoft.com/l/meetup-join/12345",
                    Notes = "Technical interview focusing on C# and Azure",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Id = 2,
                    ApplicationId = 1,
                    InterviewDate = DateTime.UtcNow.AddDays(7),
                    InterviewType = InterviewType.Onsite,
                    Duration = 120,
                    InterviewerName = "Mike Chen",
                    InterviewerPosition = "Senior Engineer",
                    Location = "Microsoft Campus, Building 92",
                    Notes = "Onsite technical interview with coding challenges",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Id = 3,
                    ApplicationId = 3,
                    InterviewDate = DateTime.UtcNow.AddDays(-10),
                    InterviewType = InterviewType.Phone,
                    Duration = 45,
                    InterviewerName = "Alex Rodriguez",
                    InterviewerPosition = "Recruiter",
                    Location = "Phone",
                    Notes = "Initial phone screening - went well",
                    Outcome = "Passed",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-15)
                }
            };

            context.Interviews.AddRange(interviews);

            // Create sample notes
            var notes = new List<Note>
            {
                new()
                {
                    Id = 1,
                    ApplicationId = 1,
                    Title = "Company Research",
                    Content =
                        "Microsoft is a leading technology company with strong focus on cloud computing and AI. Their Azure platform is growing rapidly.",
                    NoteType = NoteType.Research,
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    Id = 2,
                    ApplicationId = 1,
                    Title = "Interview Preparation",
                    Content =
                        "Review C# advanced topics, Azure services, and system design patterns. Practice coding problems on LeetCode.",
                    NoteType = NoteType.Interview,
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-8)
                },
                new()
                {
                    Id = 3,
                    ApplicationId = 3,
                    Title = "Offer Details",
                    Content =
                        "Received offer: $140k base salary, $20k signing bonus, RSUs worth $50k over 4 years. Benefits include health, dental, vision, 401k matching.",
                    NoteType = NoteType.Offer,
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-3)
                }
            };

            context.Notes.AddRange(notes);

            // Create sample contacts
            var contacts = new List<Contact>
            {
                new()
                {
                    Id = 1,
                    ApplicationId = 1,
                    Name = "Sarah Johnson",
                    Position = "Hiring Manager",
                    Email = "sarah.johnson@microsoft.com",
                    LinkedIn = "https://linkedin.com/in/sarahjohnson",
                    IsPrimaryContact = true,
                    Notes = "Very responsive and helpful during the interview process",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    Id = 2,
                    ApplicationId = 1,
                    Name = "Mike Chen",
                    Position = "Senior Software Engineer",
                    Email = "mike.chen@microsoft.com",
                    LinkedIn = "https://linkedin.com/in/mikechen",
                    IsPrimaryContact = false,
                    Notes = "Will be conducting the technical interview",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-8)
                },
                new()
                {
                    Id = 3,
                    ApplicationId = 3,
                    Name = "Alex Rodriguez",
                    Position = "Technical Recruiter",
                    Email = "alex.rodriguez@amazon.com",
                    Phone = "+1-555-0123",
                    LinkedIn = "https://linkedin.com/in/alexrodriguez",
                    IsPrimaryContact = true,
                    Notes = "Great recruiter, very professional and kept me updated throughout the process",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-20)
                }
            };

            context.Contacts.AddRange(contacts);

            // Create sample documents
            var documents = new List<Document>
            {
                new()
                {
                    Id = 1,
                    ApplicationId = 1,
                    DocumentType = DocumentType.Resume,
                    FileName = "John_Doe_Resume_2024.pdf",
                    FileUrl = "/documents/resumes/John_Doe_Resume_2024.pdf",
                    FileSize = 245760,
                    MimeType = "application/pdf",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    Id = 2,
                    ApplicationId = 1,
                    DocumentType = DocumentType.CoverLetter,
                    FileName = "Microsoft_Cover_Letter.pdf",
                    FileUrl = "/documents/cover-letters/Microsoft_Cover_Letter.pdf",
                    FileSize = 128000,
                    MimeType = "application/pdf",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    Id = 3,
                    ApplicationId = 3,
                    DocumentType = DocumentType.OfferLetter,
                    FileName = "Amazon_Offer_Letter.pdf",
                    FileUrl = "/documents/offers/Amazon_Offer_Letter.pdf",
                    FileSize = 512000,
                    MimeType = "application/pdf",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-3)
                }
            };

            context.Documents.AddRange(documents);

            // Create sample status history
            var statusHistory = new List<StatusHistory>
            {
                new()
                {
                    Id = 1,
                    ApplicationId = 1,
                    OldStatus = null,
                    NewStatus = ApplicationStatus.Applied,
                    ChangedAt = DateTime.UtcNow.AddDays(-10),
                    Notes = "Initial application submitted",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    Id = 2,
                    ApplicationId = 1,
                    OldStatus = ApplicationStatus.Applied,
                    NewStatus = ApplicationStatus.Interview,
                    ChangedAt = DateTime.UtcNow.AddDays(-5),
                    Notes = "Received interview invitation",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Id = 3,
                    ApplicationId = 3,
                    OldStatus = null,
                    NewStatus = ApplicationStatus.Applied,
                    ChangedAt = DateTime.UtcNow.AddDays(-20),
                    Notes = "Initial application submitted",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-20)
                },
                new()
                {
                    Id = 4,
                    ApplicationId = 3,
                    OldStatus = ApplicationStatus.Applied,
                    NewStatus = ApplicationStatus.Interview,
                    ChangedAt = DateTime.UtcNow.AddDays(-15),
                    Notes = "Phone screening scheduled",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-15)
                },
                new()
                {
                    Id = 5,
                    ApplicationId = 3,
                    OldStatus = ApplicationStatus.Interview,
                    NewStatus = ApplicationStatus.Offer,
                    ChangedAt = DateTime.UtcNow.AddDays(-3),
                    Notes = "Received job offer",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-3)
                },
                new()
                {
                    Id = 6,
                    ApplicationId = 4,
                    OldStatus = null,
                    NewStatus = ApplicationStatus.Applied,
                    ChangedAt = DateTime.UtcNow.AddDays(-15),
                    Notes = "Initial application submitted",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-15)
                },
                new()
                {
                    Id = 7,
                    ApplicationId = 4,
                    OldStatus = ApplicationStatus.Applied,
                    NewStatus = ApplicationStatus.Rejected,
                    ChangedAt = DateTime.UtcNow.AddDays(-5),
                    Notes = "Application rejected after initial review",
                    Status = CommonStatus.Active,
                    CreatedBy = "System",
                    CreatedUtc = DateTime.UtcNow.AddDays(-5)
                }
            };

            context.StatusHistory.AddRange(statusHistory);

            await context.SaveChangesAsync();

            logger.LogInformation("Seeding completed.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while seeding the database.");
            throw;
        }
    }
}