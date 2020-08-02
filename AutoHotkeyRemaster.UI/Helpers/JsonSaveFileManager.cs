using AutoHotkeyRemaster.Services;
using System;
using System.IO;
using System.Text.Json;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public class JsonSaveFileManager : IJsonSavefileManager
    {
        public static string SavefileFolder { get; private set; }
            = Path.Combine(Environment.CurrentDirectory, "savefiles");

        public JsonSaveFileManager()
        {
            if (!Directory.Exists(SavefileFolder))
                Directory.CreateDirectory(SavefileFolder);
        }

        public bool DeleteIfExists(string filename, bool appendExtenstion = true)
        {
            string path = Path.Combine(SavefileFolder, filename);

            if (appendExtenstion)
                path += ".json";

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        public void Save<T>(T instance, string filename, bool appendExtenstion = true) where T : class
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
        }

        public T Load<T>(string filename, bool appendExtenstion = true) where T : class
        {
            string path = Path.Combine(SavefileFolder, filename);

            if (appendExtenstion)
                path += ".json";

            if (!File.Exists(path))
                return null;

            string jsonString = File.ReadAllText(path);
            T instance = JsonSerializer.Deserialize<T>(jsonString);

            return instance;
        }

    }
}
