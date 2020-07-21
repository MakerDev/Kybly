using AutoHotkeyRemaster.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public class UwpSavefileManager : IAsyncJsonFileManager
    {
        public StorageFolder _storageFolderPath { get; private set; }

        private UwpSavefileManager()
        {            

        }

        public static async Task<IAsyncJsonFileManager> CreateAsync()
        {
            var fileManager = new UwpSavefileManager();
            fileManager._storageFolderPath = await ApplicationData.Current.LocalFolder.CreateFolderAsync("savefiles");

            return fileManager;
        }

        public Task<bool> DeleteIfExistsAsync(string filename, bool appendExtenstion = true)
        {
            throw new NotImplementedException();
        }

        public Task<T> LoadAsync<T>(string filename, bool appendExtenstion = true) where T : class
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync<T>(T instance, string filename, bool appendExtenstion = true) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
