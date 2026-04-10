using Microsoft.Extensions.Caching.Memory;

namespace ClinicManagementSystem.Application.Common.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly HashSet<string> _keys = new();
        private readonly object _lock = new();

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            };

            lock (_lock)
            {
                _keys.Add(key);
            }

            _cache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            lock (_lock)
            {
                _keys.Remove(key);
            }

            _cache.Remove(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            lock (_lock)
            {
                var keysToRemove = _keys.Where(k => k.StartsWith(prefix)).ToList();
                foreach (var key in keysToRemove)
                {
                    _keys.Remove(key);
                    _cache.Remove(key);
                }
            }
        }
    }
}
