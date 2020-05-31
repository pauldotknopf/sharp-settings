using System;
using System.Threading.Tasks;
using SharpEvents;

namespace SharpSettingsStore.Impl
{
    public class TypedSettingsService<T> : ITypedSettingsService<T>
    {
        private readonly ISettingsService _settingsService;
        private readonly AsyncEventAggregator<T> _settingsUpdated;

        public TypedSettingsService(ISettingsService settingsService, SharpEventDispatcherAsyncDel eventDispatcher)
        {
            _settingsService = settingsService;
            _settingsUpdated = new AsyncEventAggregator<T>(eventDispatcher);
        }
        
        public Task<T> GetSettings()
        {
            return _settingsService.GetSetting<T>();
        }

        public async Task SaveSettings(Action<T> settings)
        {
            var instance = default(T);
            await _settingsService.SaveSetting<T>(x =>
            {
                settings(x);
                instance = x;
            });
            await _settingsUpdated.Publish(this, instance);
        }

        public IAsyncEventAggregatorConsumer<T> SettingsUpdated => _settingsUpdated;
    }
}