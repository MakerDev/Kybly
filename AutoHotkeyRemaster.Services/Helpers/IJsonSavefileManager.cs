namespace AutoHotkeyRemaster.Services.Helpers
{
    public interface IJsonSavefileManager
    {
        bool DeleteIfExists(string filename, bool appendExtenstion = true);
        T Load<T>(string filename, bool appendExtenstion = true) where T : class;
        void Save<T>(T instance, string filename, bool appendExtenstion = true) where T : class;
    }
}