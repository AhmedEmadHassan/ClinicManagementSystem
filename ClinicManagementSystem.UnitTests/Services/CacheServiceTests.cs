using ClinicManagementSystem.Application.Common.Cache;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ClinicManagementSystem.UnitTests.Services
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheService(_memoryCache);
        }

        [Fact]
        public void Set_WhenCalled_StoresValue()
        {
            _cacheService.Set("key1", "value1");
            var result = _cacheService.Get<string>("key1");
            result.Should().Be("value1");
        }

        [Fact]
        public void Get_WhenKeyDoesNotExist_ReturnsNull()
        {
            var result = _cacheService.Get<string>("nonexistent");
            result.Should().BeNull();
        }

        [Fact]
        public void Remove_WhenKeyExists_RemovesValue()
        {
            _cacheService.Set("key1", "value1");
            _cacheService.Remove("key1");
            var result = _cacheService.Get<string>("key1");
            result.Should().BeNull();
        }

        [Fact]
        public void RemoveByPrefix_WhenCalled_RemovesAllMatchingKeys()
        {
            _cacheService.Set("Doctor:GetAll:1:10", "value1");
            _cacheService.Set("Doctor:GetById:1", "value2");
            _cacheService.Set("Patient:GetAll:1:10", "value3");

            _cacheService.RemoveByPrefix("Doctor");

            _cacheService.Get<string>("Doctor:GetAll:1:10").Should().BeNull();
            _cacheService.Get<string>("Doctor:GetById:1").Should().BeNull();
            _cacheService.Get<string>("Patient:GetAll:1:10").Should().Be("value3");
        }

        [Fact]
        public void Set_WithCustomExpiry_StoresValue()
        {
            _cacheService.Set("key1", "value1", TimeSpan.FromSeconds(30));
            var result = _cacheService.Get<string>("key1");
            result.Should().Be("value1");
        }

        [Fact]
        public void RemoveByPrefix_WhenNoPrefixMatch_DoesNotRemoveOtherKeys()
        {
            _cacheService.Set("Patient:GetAll:1:10", "value1");
            _cacheService.RemoveByPrefix("Doctor");
            _cacheService.Get<string>("Patient:GetAll:1:10").Should().Be("value1");
        }
    }
}
