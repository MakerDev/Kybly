using AutoHotkeyRemaster.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHotkeyRemaster.WPF.Helpers
{
    public class UwpSavefileManager : IJsonSavefileManager
    {
        public bool DeleteIfExists(string filename, bool appendExtenstion = true)
        {
            throw new NotImplementedException();
        }

        public T Load<T>(string filename, bool appendExtenstion = true) where T : class
        {
            throw new NotImplementedException();
        }

        public void Save<T>(T instance, string filename, bool appendExtenstion = true) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
