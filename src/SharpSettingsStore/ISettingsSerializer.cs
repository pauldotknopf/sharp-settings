namespace SharpSettingsStore
{
    public interface ISettingsSerializer
    {
        string Serialize<T>(T value);

        T Deserialize<T>(string value);
    }
}