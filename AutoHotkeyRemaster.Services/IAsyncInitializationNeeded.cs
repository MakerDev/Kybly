using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.Services
{
    interface IAsyncInitializationNeeded
    {
        Task InitializeAsync();
    }
}
