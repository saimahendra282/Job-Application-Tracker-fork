using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using JobApplicationTrackerApi.Services.CacheService;
using LiterateWinnerApi.Services.CacheService;
using JobApplicationTrackerApi.Services.MonitoringService;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace JobApplicationTrackerApi.Tests.PerformanceTests;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class ServicePerformanceBenchmarks
{
    private ICacheService? _cacheService;
    private Mock<IMonitoringService>? _monitoringServiceMock;
    private Mock<IDistributedCache>? _distributedCacheMock;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();

        _cacheService = new LiterateWinnerApi.Services.CacheService.CacheService(
            serviceProvider.GetRequiredService<IDistributedCache>(),
            serviceProvider.GetRequiredService<IMemoryCache>(),
            serviceProvider,
            serviceProvider.GetRequiredService<ILogger<LiterateWinnerApi.Services.CacheService.CacheService>>()
        );
        _monitoringServiceMock = new Mock<IMonitoringService>();
        _distributedCacheMock = new Mock<IDistributedCache>();
    }

    [Benchmark]
    public async Task CacheSetOperation()
    {
        await _cacheService!.SetAsync("test_key", "test_value", TimeSpan.FromMinutes(5));
    }

    [Benchmark]
    public async Task CacheGetOperation()
    {
        await _cacheService!.GetAsync<string>("test_key");
    }

    [Benchmark]
    public void StringConcatenation()
    {
        var result = string.Empty;
        for (int i = 0; i < 1000; i++)
        {
            result += i.ToString();
        }
    }

    [Benchmark]
    public void StringBuilderConcatenation()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            sb.Append(i);
        }
        var result = sb.ToString();
    }

    [Benchmark]
    public void ConcurrentDictionaryOperations()
    {
        var dict = new ConcurrentDictionary<int, string>();
        Parallel.For(0, 1000, i =>
        {
            dict.TryAdd(i, $"value_{i}");
        });
    }

    [Benchmark]
    public void JsonSerialization()
    {
        var data = new { Id = 1, Name = "Test", Items = Enumerable.Range(1, 100).ToList() };
        var json = JsonSerializer.Serialize(data);
    }

    [Benchmark]
    public void JsonDeserialization()
    {
        var json = "{\"Id\":1,\"Name\":\"Test\",\"Items\":[1,2,3,4,5,6,7,8,9,10]}";
        var data = JsonSerializer.Deserialize<TestData>(json);
    }

    private class TestData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<int>? Items { get; set; }
    }
}