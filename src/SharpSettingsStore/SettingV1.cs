using ServiceStack.DataAnnotations;

namespace SharpSettingsStore
{
    [Alias("settings")]
    public class SettingV1
    {
        [Alias("id"), Required, PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Alias("name")]
        public string Name { get; set; }
        
        [Alias("settings")]
        public string Settings { get; set; }
    }
}