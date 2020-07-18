using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace AutoHotkeyRemaster.Models.Helpers
{
    public static class JsonFileManager
    {
        public static string SavefileFolder { get; private set; }             
            = Path.Combine(Environment.CurrentDirectory, "savefiles");

        static JsonFileManager()
        {
            if (!Directory.Exists(SavefileFolder))
                Directory.CreateDirectory(SavefileFolder);
        }

        public static bool DeleteIfExists(string filename, bool appendExtenstion = true)
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

        public static void Save(object instance, string filename, bool appendExtenstion = true)
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

        public static T Load<T>(string filename, bool appendExtenstion = true) where T : class
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
