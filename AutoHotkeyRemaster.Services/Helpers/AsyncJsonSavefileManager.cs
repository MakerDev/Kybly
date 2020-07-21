using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.Services.Helpers
{
    public class AsyncJsonSavefileManager : IAsyncJsonFileManager
    {
        public static string SavefileFolder { get; private set; }
            = Path.Combine(Environment.CurrentDirectory, "savefiles");

        public AsyncJsonSavefileManager()
        {
            if (!Directory.Exists(SavefileFolder))
                Directory.CreateDirectory(SavefileFolder);
        }

        public Task<bool> DeleteIfExistsAsync(string filename, bool appendExtenstion = true)
        {
            string path = Path.Combine(SavefileFolder, filename);

            if (appendExtenstion)
                path += ".json";

            if (File.Exists(path))
            {
                File.Delete(path);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<T> LoadAsync<T>(string filename, bool appendExtenstion = true) where T : class
        {
            string path = Path.Combine(SavefileFolder, filename);

            if (appendExtenstion)
                path += ".json";

            //TODO : Replace exception to reduce overhead. This is because we can't return Task.FromResult(null)
            if (!File.Exists(path))
                return Task.FromResult<T>(null);

            string jsonString = File.ReadAllText(path);
            T instance = JsonSerializer.Deserialize<T>(jsonString);

            return Task.FromResult(instance);
        }

        public Task SaveAsync<T>(T instance, string filename, bool appendExtenstion = true) where T : class
        {
            string path = Path.Combine(SavefileFolder, filename);

            if (appendExtenstion)
                path += ".json";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(instance, options);
            File.WriteAllText(path, jsonString);

            return Task.CompletedTask;
        }
    }
}
