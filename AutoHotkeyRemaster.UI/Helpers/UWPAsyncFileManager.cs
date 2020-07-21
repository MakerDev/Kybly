using AutoHotkeyRemaster.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public class UWPAsyncFileManager : IAsyncJsonFileManager
    {
        public StorageFolder _storageFolderPath { get; private set; } = ApplicationData.Current.LocalFolder;

        public async Task<bool> DeleteIfExistsAsync(string filename, bool appendExtenstion = true)
        {
            if (appendExtenstion)
                filename += ".json";

            var item = await _storageFolderPath.TryGetItemAsync(filename);

            if (item == null)
                return false;
         
            await item.DeleteAsync();

            return true;
        }

        public async Task<T> LoadAsync<T>(string filename, bool appendExtenstion = true) where T : class
        {
            if (appendExtenstion)
                filename += ".json";

            var item = await _storageFolderPath.TryGetItemAsync(filename);

            if (item == null)
                return null;

            string jsonString = await FileIO.ReadTextAsync((StorageFile)item);
            T instance = JsonSerializer.Deserialize<T>(jsonString);

            return instance;
        }

        public async Task SaveAsync<T>(T instance, string filename, bool appendExtenstion = true) where T : class
        {
            if (appendExtenstion)
                filename += ".json";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(instance, options);

            var file = await _storageFolderPath.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(file, jsonString);
        }
    }
}
