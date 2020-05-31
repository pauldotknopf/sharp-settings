using System;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using SharpDataAccess.Data;

namespace SharpSettingsStore.Impl
{
    public class SettingsService : ISettingsService
    {
        private readonly IDataService _dataService;
        private readonly ISettingsSerializer _settingsSerializer;

        public SettingsService(IDataService dataService, ISettingsSerializer settingsSerializer)
        {
            _dataService = dataService;
            _settingsSerializer = settingsSerializer;
        }
        
        public async Task<T> GetSetting<T>()
        {
            var name = GetSettingNameAttribute(typeof(T));
            
            using (var connection = new ConScope(_dataService))
            {
                var setting = await connection.Connection.SingleAsync(connection.Connection.From<SettingV1>().Where(x => x.Name == name));
                if (setting == null || string.IsNullOrEmpty(setting.Settings))
                {
                    return (T)Activator.CreateInstance(typeof(T));
                }
                return _settingsSerializer.Deserialize<T>(setting.Settings);
            }
        }

        public async Task SaveSetting<T>(T value)
        {
            var name = GetSettingNameAttribute(typeof(T));

            var settingValue = _settingsSerializer.Serialize(value);
            
            using (var connection = new ConScope(_dataService))
            using (var transaction = await connection.BeginTransaction())
            {
                var setting = await connection.Connection.SingleAsync(connection.Connection.From<SettingV1>().Where(x => x.Name == name));
                if (setting == null)
                {
                    setting = new SettingV1 {Name = name, Settings = settingValue};
                    await connection.Connection.SaveAsync(setting);
                }
                else
                {
                    setting.Settings = settingValue;
                    await connection.Connection.UpdateAsync(setting);
                }
                
                transaction.Commit();
            }
        }

        public async Task SaveSetting<T>(Action<T> updateAction)
        {
            using (var connection = new ConScope(_dataService))
            using (var transaction = await connection.BeginTransaction())
            {
                var name = GetSettingNameAttribute(typeof(T));
           
                var existing = await GetSetting<T>();
                updateAction(existing);

                var settingValue = _settingsSerializer.Serialize(existing);

                var setting = await connection.Connection.SingleAsync(connection.Connection.From<SettingV1>().Where(x => x.Name == name));
                if (setting == null)
                {
                    setting = new SettingV1 {Name = name, Settings = settingValue};
                    await connection.Connection.SaveAsync(setting);
                }
                else
                {
                    setting.Settings = settingValue;
                    await connection.Connection.UpdateAsync(setting);
                }
                
                transaction.Commit();
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static string GetSettingNameAttribute(Type settingType)
        {
            var nameAttribute = settingType.GetCustomAttributes(typeof(SettingNameAttribute), true).FirstOrDefault();
            if (nameAttribute == null)
            {
                throw new Exception("[SettingName] attribute is required.");
            }

            var name = ((SettingNameAttribute) nameAttribute).Name;
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("The Name property is required on the [SettingName] property.");
            }

            return name;
        }
    }
}