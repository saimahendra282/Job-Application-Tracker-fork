using System.Text;
using System.Text.Json;
using JobApplicationTrackerApi.Persistence.DefaultContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Controllers;

/// <summary>
/// Root endpoint providing API metadata, useful links, and an index of available endpoints.
/// Serves HTML for browsers and JSON for API clients via content negotiation.
/// </summary>
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController(
    IApiDescriptionGroupCollectionProvider apiExplorer,
    IWebHostEnvironment environment,
    IConfiguration configuration,
    LinkGenerator linkGenerator,
    DefaultContext context,
    ILogger<HomeController> logger
)
    : Controller
{
    private readonly IApiDescriptionGroupCollectionProvider _apiExplorer =
        apiExplorer ?? throw new ArgumentNullException(nameof(apiExplorer));

    private readonly IConfiguration _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    private readonly DefaultContext _context =
        context ?? throw new ArgumentNullException(nameof(context));

    private readonly IWebHostEnvironment _environment =
        environment ?? throw new ArgumentNullException(nameof(environment));

    private readonly LinkGenerator _linkGenerator =
        linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));

    private readonly ILogger<HomeController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Returns API home document. HTML for browsers, JSON for API clients.
    /// </summary>
    /// <returns>API home document.</returns>
    [HttpGet("/")]
    [Produces("text/html", "application/json")]
    public async Task<IActionResult> Index()
    {
        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        // Get database statistics
        var dbStats = await GetDatabaseStatistics();

        var meta = new
        {
            name = "Job Application Tracker API",
            version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0",
            environment = _environment.EnvironmentName,
            serverTimeUtc = DateTime.UtcNow,
            description = "Comprehensive RESTful API for tracking job applications, interviews, and career management.",
            framework = "ASP.NET Core 9.0",
            database = "SQL Server with Entity Framework Core 9.0",
            features = new[]
            {
                "Job Application Management",
                "Interview Scheduling & Tracking",
                "Notes & Research Management",
                "Contact Management",
                "Document Storage",
                "Status History Tracking",
                "Analytics & Reporting",
                "Email Reminders",
                "Calendar Integration"
            },
            databaseStatistics = dbStats,
            links = new[]
            {
                new { rel = "openapi", href = $"{baseUrl}/openapi/v1.json", description = "OpenAPI (JSON)" },
                new { rel = "swagger-ui", href = $"{baseUrl}/swagger", description = "Swagger UI" },
                new { rel = "health", href = $"{baseUrl}/health", description = "Liveness/Readiness" },
                new
                {
                    rel = "documentation", href = $"{baseUrl}/api/home/documentation", description = "API Documentation"
                }
            }
        };

        // Build endpoint index via ApiExplorer
        var endpoints = _apiExplorer.ApiDescriptionGroups.Items
            .SelectMany(g => g.Items)
            .Where(d => d.RelativePath is not null)
            .Select(d => new
            {
                controller = d.ActionDescriptor.RouteValues.TryGetValue("controller", out var c) ? c : null,
                method = d.HttpMethod,
                path = "/" + d.RelativePath!.TrimStart('/'),
                supportedResponseTypes = d.SupportedResponseTypes.Select(rt => rt.StatusCode).Distinct().OrderBy(x => x)
            })
            .OrderBy(x => x.controller)
            .ThenBy(x => x.path)
            .ThenBy(x => x.method)
            .ToList();

        var accepts = request.Headers["Accept"].ToString();
        var wantsHtml = string.IsNullOrWhiteSpace(accepts) ||
                        accepts.Contains("text/html", StringComparison.OrdinalIgnoreCase);

        if (wantsHtml)
        {
            var html = BuildHtmlHome(meta, endpoints);
            return Content(html, "text/html", Encoding.UTF8);
        }

        return Ok(new { meta, endpoints });
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("/health")]
    [Produces("application/json")]
    public async Task<IActionResult> Health()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var dbStats = await GetDatabaseStatistics();

            var health = new
            {
                status = canConnect ? "Healthy" : "Unhealthy",
                timestamp = DateTime.UtcNow,
                uptime = Environment.TickCount64,
                environment = _environment.EnvironmentName,
                version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "Unknown",
                machineName = Environment.MachineName,
                processId = Environment.ProcessId,
                workingSet = Environment.WorkingSet,
                processorCount = Environment.ProcessorCount,
                database = new
                {
                    connected = canConnect,
                    provider = _context.Database.ProviderName,
                    statistics = dbStats
                }
            };

            return canConnect ? Ok(health) : StatusCode(503, health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { status = "Unhealthy", error = ex.Message, timestamp = DateTime.UtcNow });
        }
    }

    /// <summary>
    /// Gets API documentation and usage examples
    /// </summary>
    /// <returns>API documentation</returns>
    [HttpGet("/api/home/documentation")]
    [Produces("application/json")]
    public IActionResult GetDocumentation()
    {
        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        var documentation = new
        {
            title = "Job Application Tracker API Documentation",
            version = "1.0.0",
            description = "A comprehensive API for managing job applications, interviews, and related activities",
            baseUrl = baseUrl,
            authentication = new
            {
                type = "JWT Bearer Token",
                description = "All endpoints require authentication except health checks and home page",
                header = "Authorization: Bearer {token}"
            },
            commonHeaders = new
            {
                contentType = "application/json",
                accept = "application/json"
            },
            responseFormats = new
            {
                success = new { statusCode = 200, description = "Request successful" },
                created = new { statusCode = 201, description = "Resource created successfully" },
                noContent = new { statusCode = 204, description = "Request successful, no content returned" },
                badRequest = new { statusCode = 400, description = "Invalid request data" },
                unauthorized = new { statusCode = 401, description = "Authentication required" },
                forbidden = new { statusCode = 403, description = "Access denied" },
                notFound = new { statusCode = 404, description = "Resource not found" },
                internalServerError = new { statusCode = 500, description = "Internal server error" }
            },
            exampleRequests = new
            {
                createApplication = new
                {
                    method = "POST",
                    url = "/api/applications",
                    body = new
                    {
                        companyName = "Microsoft",
                        position = "Senior Software Engineer",
                        location = "Seattle, WA",
                        jobUrl = "https://careers.microsoft.com/job/12345",
                        applicationStatus = 1,
                        priority = 3,
                        salaryMin = 120000,
                        salaryMax = 150000,
                        applicationDate = "2024-01-15T00:00:00Z",
                        jobDescription = "We are looking for a Senior Software Engineer...",
                        requirements = "5+ years of experience with C# and .NET"
                    }
                },
                createInterview = new
                {
                    method = "POST",
                    url = "/api/applications/1/interviews",
                    body = new
                    {
                        interviewDate = "2024-01-20T14:00:00Z",
                        interviewType = 2,
                        duration = 60,
                        interviewerName = "Sarah Johnson",
                        interviewerPosition = "Hiring Manager",
                        location = "Virtual",
                        meetingLink = "https://teams.microsoft.com/l/meetup-join/12345",
                        notes = "Technical interview focusing on C# and Azure"
                    }
                }
            },
            filteringAndPagination = new
            {
                applications = new
                {
                    queryParameters = new
                    {
                        status = "Filter by application status (1=Applied, 2=Interview, 3=Offer, 4=Rejected)",
                        priority = "Filter by priority (1=Low, 2=Medium, 3=High)",
                        search = "Search by company name or position",
                        page = "Page number (default: 1)",
                        pageSize = "Items per page (default: 20)",
                        sortBy = "Sort field (ApplicationDate, CompanyName, Status)",
                        sortOrder = "Sort order (asc, desc)"
                    },
                    example =
                        "/api/applications?status=1&priority=3&search=Microsoft&page=1&pageSize=10&sortBy=ApplicationDate&sortOrder=desc"
                }
            }
        };

        return Ok(documentation);
    }

    private async Task<object> GetDatabaseStatistics()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                return new { connected = false, message = "Database unavailable" };
            }

            return new
            {
                connected = true,
                provider = _context.Database.ProviderName,
                statistics = new
                {
                    applications = await _context.Applications.CountAsync(),
                    interviews = await _context.Interviews.CountAsync(),
                    notes = await _context.Notes.CountAsync(),
                    contacts = await _context.Contacts.CountAsync(),
                    documents = await _context.Documents.CountAsync(),
                    statusHistory = await _context.StatusHistory.CountAsync()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get database statistics");
            return new { connected = false, error = ex.Message };
        }
    }

    private static string BuildHtmlHome(object meta, IEnumerable<object> endpoints)
    {
        // Modern, responsive HTML landing page
        var sb = new StringBuilder();
        sb.Append(
            "<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>Job Application Tracker API</title><style>");
        sb.Append(
            "body{font-family:Inter,system-ui,Segoe UI,Roboto,Helvetica,Arial,sans-serif;margin:0;padding:24px;color:#0f172a;background:#fff;line-height:1.6}");
        sb.Append(
            "h1{font-size:32px;margin:0 0 8px;background:linear-gradient(135deg,#2563eb,#7c3aed);-webkit-background-clip:text;-webkit-text-fill-color:transparent;background-clip:text}");
        sb.Append(
            "h2{font-size:24px;margin:24px 0 16px;color:#1e293b}h3{font-size:18px;margin:16px 0 8px;color:#334155}");
        sb.Append(
            "p{margin:8px 0;color:#475569}.muted{color:#64748b}.card{border:1px solid #e2e8f0;border-radius:12px;padding:20px;margin:16px 0;background:#fafafa}");
        sb.Append(".grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(280px,1fr));gap:16px}");
        sb.Append(
            "code,kbd{background:#f1f5f9;padding:4px 8px;border-radius:6px;font-family:JetBrains Mono,Consolas,monospace}");
        sb.Append("a{color:#2563eb;text-decoration:none;font-weight:500}a:hover{text-decoration:underline}");
        sb.Append(
            ".endpoints{max-height:500px;overflow:auto;border:1px solid #e2e8f0;border-radius:12px;padding:16px;background:#fafafa}");
        sb.Append(
            "table{width:100%;border-collapse:collapse}th,td{padding:8px 12px;text-align:left;border-bottom:1px solid #e2e8f0}");
        sb.Append(
            "th{background:#f8fafc;font-weight:600;color:#374151}.status-badge{padding:4px 12px;border-radius:20px;font-size:12px;font-weight:600}");
        sb.Append(
            ".status-healthy{background:#dcfce7;color:#166534}.status-unhealthy{background:#fef2f2;color:#dc2626}");
        sb.Append(
            ".feature-list{display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr));gap:8px;margin:16px 0}");
        sb.Append(".feature-item{padding:8px 12px;background:#f1f5f9;border-radius:8px;font-size:14px}");
        sb.Append("</style></head><body>");

        // Header
        sb.Append("<h1>Job Application Tracker API</h1>");
        sb.Append(
            "<p class=\"muted\">Comprehensive RESTful API for tracking job applications, interviews, and career management. This landing page exposes key metadata and available endpoints.</p>");

        // Metadata card
        sb.Append("<div class=\"card\"><h2>API Metadata</h2><pre><code>");
        sb.Append(JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));
        sb.Append("</code></pre></div>");

        // Quick links grid
        sb.Append("<div class=\"grid\">");
        sb.Append("<div class=\"card\"><h3>📚 Documentation</h3><ul>");
        sb.Append("<li><a href=\"/swagger\" target=\"_blank\">Swagger UI</a></li>");
        sb.Append("<li><a href=\"/scalar\" target=\"_blank\">Scalar UI</a></li>");
        sb.Append("<li><a href=\"/openapi/v1.json\" target=\"_blank\">OpenAPI (JSON)</a></li>");
        sb.Append("<li><a href=\"/api/home/documentation\" target=\"_blank\">API Documentation</a></li>");
        sb.Append("</ul></div>");

        sb.Append(
            "<div class=\"card\"><h3>🔧 Usage</h3><p class=\"muted\">Authorize with <code>Bearer &lt;token&gt;</code> in the <code>Authorization</code> header.</p>");
        sb.Append("<pre><code>curl -H \"Authorization: Bearer &lt;token&gt;\" /api/applications</code></pre></div>");

        sb.Append("<div class=\"card\"><h3>🏥 Health</h3><p class=\"muted\">Check API and database status.</p>");
        sb.Append("<a href=\"/health\" target=\"_blank\">Health Check</a></div>");
        sb.Append("</div>");

        // Features
        sb.Append("<div class=\"card\"><h2>✨ Features</h2><div class=\"feature-list\">");
        var features = new[]
        {
            "Job Application Management", "Interview Scheduling", "Notes & Research", "Contact Management",
            "Document Storage", "Status Tracking", "Analytics & Reporting", "Email Reminders", "Calendar Integration"
        };
        foreach (var feature in features)
        {
            sb.Append($"<div class=\"feature-item\">{feature}</div>");
        }

        sb.Append("</div></div>");

        // Endpoints table
        sb.Append(
            "<div class=\"card\"><h2>🔗 Available Endpoints</h2><div class=\"endpoints\"><table><thead><tr><th>Method</th><th>Path</th><th>Controller</th><th>Status Codes</th></tr></thead><tbody>");
        foreach (var ep in endpoints)
        {
            var method = ep.GetType().GetProperty("method")?.GetValue(ep)?.ToString() ?? "";
            var path = ep.GetType().GetProperty("path")?.GetValue(ep)?.ToString() ?? "";
            var controller = ep.GetType().GetProperty("controller")?.GetValue(ep)?.ToString() ?? "";
            var statusCodes = ep.GetType().GetProperty("supportedResponseTypes")?.GetValue(ep)?.ToString() ?? "";

            var methodColor = method switch
            {
                "GET" => "#10b981",
                "POST" => "#3b82f6",
                "PUT" => "#f59e0b",
                "PATCH" => "#8b5cf6",
                "DELETE" => "#ef4444",
                _ => "#6b7280"
            };

            sb.Append($"<tr><td><code style=\"color:{methodColor};font-weight:600\">{method}</code></td>");
            sb.Append($"<td><code>{path}</code></td>");
            sb.Append($"<td class=\"muted\">{controller}</td>");
            sb.Append($"<td class=\"muted\">{statusCodes}</td></tr>");
        }

        sb.Append("</tbody></table></div></div>");

        // Footer
        sb.Append(
            $"<p class=\"muted\">&copy; {DateTime.UtcNow.Year} Job Application Tracker API. Built with ASP.NET Core 9.0</p>");
        sb.Append("</body></html>");
        return sb.ToString();
    }
}