using System;
using System.Threading.Tasks;
using SharpEvents;

namespace SharpSettingsStore
{
    public interface ITypedSettingsService<T>
    {
        Task<T> GetSettings();

        Task SaveSettings(Action<T> settings);
        
        IAsyncEventAggregatorConsumer<T> SettingsUpdated { get; }
    }
}