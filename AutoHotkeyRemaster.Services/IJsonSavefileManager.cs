using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.Services
{
    public interface IJsonSavefileManager
    {
        void Save<T>(T instance, string filename, bool appendExtension = true) where T : class;
        T Load<T>(string filename, bool appendExtenstion = true) where T : class;
        bool DeleteIfExists(string filename, bool appendExtenstion = true);
    }
}
