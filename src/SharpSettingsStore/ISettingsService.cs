using System;
using System.Threading.Tasks;

namespace SharpSettingsStore
{
    public interface ISettingsService
    {
        Task<T> GetSetting<T>();

        Task SaveSetting<T>(T value);
        
        Task SaveSetting<T>(Action<T> updateAction);
    }
}