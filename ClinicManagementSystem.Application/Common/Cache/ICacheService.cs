namespace ClinicManagementSystem.Application.Common.Cache
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiry = null);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
    }
}
