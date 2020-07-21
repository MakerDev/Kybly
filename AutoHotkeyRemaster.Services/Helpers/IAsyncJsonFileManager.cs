using System.Threading.Tasks;

namespace AutoHotkeyRemaster.Services.Helpers
{
    public interface IAsyncJsonFileManager
    {
        Task<bool> DeleteIfExistsAsync(string filename, bool appendExtenstion = true);
        Task<T> LoadAsync<T>(string filename, bool appendExtenstion = true) where T : class;
        Task SaveAsync<T>(T instance, string filename, bool appendExtenstion = true) where T : class;
    }
}