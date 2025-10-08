using FluentAssertions;
using JobApplicationTrackerApi.Services.CacheService;
using LiterateWinnerApi.Services.CacheService;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace JobApplicationTrackerApi.Tests.UnitTests.Services;

/// <summary>
/// Unit tests for CacheService to verify distributed and local caching behavior.
/// Tests cover get/set operations, cache expiration, pattern-based removal, and distributed locks.
/// </summary>
public class CacheServiceTests
{
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<CacheService>> _loggerMock;
    private readonly CacheService _cacheService;

    public CacheServiceTests()
    {
        _distributedCacheMock = new Mock<IDistributedCache>();
        _memoryCacheMock = new Mock<IMemoryCache>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<CacheService>>();

        // Setup service provider to return null for Redis (simplifies testing)
        _serviceProviderMock.Setup(x => x.GetService(typeof(IConnectionMultiplexer)))
            .Returns((IConnectionMultiplexer?)null);

        _cacheService = new CacheService(
            _distributedCacheMock.Object,
            _memoryCacheMock.Object,
            _serviceProviderMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAsync_WhenDataExistsInMemoryCache_ShouldReturnValueWithoutCallingDistributedCache()
    {
        // Arrange - Setup memory cache to return data
        var key = "memory-test-key";
        var expectedValue = new TestData { Id = 2, Name = "Memory Test" };

        object? cacheEntry = expectedValue;
        _memoryCacheMock
            .Setup(x => x.TryGetValue(key, out cacheEntry))
            .Returns(true);

        // Act
        var result = await _cacheService.GetAsync<TestData>(key, useLocalCache: true);

        // Assert - Should return from memory cache and not call distributed cache
        result.Should().NotBeNull();
        result!.Id.Should().Be(expectedValue.Id);
        _distributedCacheMock.Verify(
            x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAsync_WhenDataDoesNotExist_ShouldReturnDefault()
    {
        // Arrange - Setup cache to return null
        var key = "non-existent-key";

        _distributedCacheMock
            .Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        object? cacheEntry = null;
        _memoryCacheMock
            .Setup(x => x.TryGetValue(key, out cacheEntry))
            .Returns(false);

        // Act
        var result = await _cacheService.GetAsync<TestData>(key);

        // Assert - Should return default (null)
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldStoreInBothDistributedAndMemoryCache()
    {
        // Arrange
        var key = "set-test-key";
        var value = new TestData { Id = 3, Name = "Set Test" };

        // Act
        await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(5));

        // Assert - Should call both distributed and memory cache set operations
        _distributedCacheMock.Verify(
            x => x.SetAsync(
                key,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _memoryCacheMock.Verify(
            x => x.CreateEntry(key),
            Times.Once
        );
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveFromBothCaches()
    {
        // Arrange
        var key = "remove-test-key";

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert - Should call remove on both caches
        _distributedCacheMock.Verify(
            x => x.RemoveAsync(key, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _memoryCacheMock.Verify(
            x => x.Remove(key),
            Times.Once
        );
    }

    [Fact]
    public async Task GetOrSetAsync_WhenDataDoesNotExist_ShouldCallFactoryAndCache()
    {
        // Arrange
        var key = "factory-test-key";
        var expectedValue = new TestData { Id = 4, Name = "Factory Test" };
        var factoryCalled = false;

        _distributedCacheMock
            .Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        object? cacheEntry = null;
        _memoryCacheMock
            .Setup(x => x.TryGetValue(key, out cacheEntry))
            .Returns(false);

        // Act
        var result = await _cacheService.GetOrSetAsync(
            key,
            async () =>
            {
                factoryCalled = true;
                await Task.Delay(10); // Simulate async work
                return expectedValue;
            }
        );

        // Assert - Factory should be called and value should be cached
        factoryCalled.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Id.Should().Be(expectedValue.Id);

        _distributedCacheMock.Verify(
            x => x.SetAsync(
                key,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetOrSetAsync_WhenDataExists_ShouldNotCallFactory()
    {
        // Arrange
        var key = "existing-data-key";
        var existingValue = new TestData { Id = 5, Name = "Existing" };
        var factoryCalled = false;

        object? cacheEntry = existingValue;
        _memoryCacheMock
            .Setup(x => x.TryGetValue(key, out cacheEntry))
            .Returns(true);

        // Act
        var result = await _cacheService.GetOrSetAsync(
            key,
            async () =>
            {
                factoryCalled = true;
                await Task.Delay(10);
                return new TestData { Id = 999, Name = "Should Not Be Used" };
            }
        );

        // Assert - Factory should not be called, existing value returned
        factoryCalled.Should().BeFalse();
        result.Should().NotBeNull();
        result!.Id.Should().Be(existingValue.Id);
        result.Name.Should().Be(existingValue.Name);
    }

    [Fact]
    public async Task RemoveByPatternAsync_WhenRedisNotConfigured_ShouldLogAndReturn()
    {
        // Arrange
        var pattern = "test:*";

        // Act
        await _cacheService.RemoveByPatternAsync(pattern);

        // Assert - Should log information and not throw
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Redis not configured")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task SetAsync_WithException_ShouldLogError()
    {
        // Arrange
        var key = "error-key";
        var value = new TestData { Id = 6, Name = "Error Test" };

        _distributedCacheMock
            .Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new Exception("Cache error"));

        // Act
        await _cacheService.SetAsync(key, value);

        // Assert - Should log error and not throw
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error setting cache value")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task AcquireLockAsync_ShouldReturnDisposableLock()
    {
        // Arrange
        var key = "lock-key";

        // Act
        var lockHandle = await _cacheService.AcquireLockAsync(key, TimeSpan.FromSeconds(30));

        // Assert - Should return a lock handle
        lockHandle.Should().NotBeNull();

        // Cleanup
        if (lockHandle != null)
        {
            await lockHandle.DisposeAsync();
        }
    }

    [Fact]
    public async Task GetAsync_WithUseLocalCacheFalse_ShouldSkipMemoryCache()
    {
        // Arrange
        var key = "no-local-cache-key";
        var value = new TestData { Id = 7, Name = "No Local Cache" };
        var json = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(json);

        _distributedCacheMock
            .Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        // Act
        var result = await _cacheService.GetAsync<TestData>(key, useLocalCache: false);

        // Assert - Should not check memory cache
        result.Should().NotBeNull();
        _memoryCacheMock.Verify(
            x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny),
            Times.Never
        );
    }

    /// <summary>
    /// Test data class for cache testing
    /// </summary>
    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
