using Scalar.AspNetCore;
using Serilog;

Console.WriteLine("Starting .....");


// Configure thread pool for high concurrency
ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
var newWorkerThreads = Environment.ProcessorCount * 32;
var newCompletionPortThreads = Environment.ProcessorCount * 32;
ThreadPool.SetMinThreads(
    Math.Max(workerThreads, newWorkerThreads),
    Math.Max(completionPortThreads, newCompletionPortThreads));

Console.WriteLine(
    $"Thread pool configured with min worker threads: {newWorkerThreads}, completion port threads: {newCompletionPortThreads}");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, services, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Register endpoints API explorer and HTTP context accessor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Using in-process OutputCache (configured in DI); no Redis output cache required when Redis is absent

var app = builder.Build();

// Enable middleware for concurrency and performance
app.UseResponseCompression();
app.UseResponseCaching();
// app.UseOutputCache();
app.UseRateLimiter();

app.UseOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
app.MapScalarApiReference();
app.UseSerilogRequestLogging();


app.UseStaticFiles();

// Correct order for CORS and SignalR: routing first, then CORS, then auth
app.UseRouting();

// CORS middleware - must be after UseRouting and before UseAuthorization
app.UseCors(x => x
    .SetIsOriginAllowed(_ => true) // For development - in production, specify your origins
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

// Authentication & endpoint mapping
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.Run();