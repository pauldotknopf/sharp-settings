using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SharpSettingsStore.Impl
{
    public class SettingsSerializer : ISettingsSerializer
    {
        private readonly DefaultContractResolver _contractResolver = new DefaultContractResolver();
        
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ContractResolver = _contractResolver
            });
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
            {
                ContractResolver = _contractResolver
            });
        }
    }
}