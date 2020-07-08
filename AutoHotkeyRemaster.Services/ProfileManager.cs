using AutoHotkeyRemaster.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace AutoHotkeyRemaster.Services
{
    public class ProfileManager
    {
        public const int MAX_PROFILE_NUM = 10;

        //Need to activate proper profile to window hook
        private readonly WindowsHookManager _windowsHookManager;

        public int ProfileCount
        {
            get
            {
                return Profiles.Count;
            }
            private set { }
        }

        public List<HotkeyProfile> Profiles { get; private set; } = new List<HotkeyProfile>();

        public ProfileManager(WindowsHookManager windowsHookManager)
        {
            _windowsHookManager = windowsHookManager;

            LoadAllProfiles();
        }

        public HotkeyProfile CreateNewProfile(string profileName = null)
        {
            HotkeyProfile profile = HotkeyProfile.CreateNewProfile(ProfileCount + 1, profileName);

            Profiles.Add(profile);

            return profile;
        }

        public void SaveAllProfiles()
        {
            foreach (var profile in Profiles)
            {
                profile.Save($"profile{profile.ProfileNum}");
            }
        }

        private void LoadAllProfiles()
        {
            for (int i = 1; i <= MAX_PROFILE_NUM; i++)
            {
                HotkeyProfile profile = HotkeyProfile.LoadFromFile(i);

                if (profile == null)
                    break;

                Profiles.Add(profile);
            }
        }
    }
}
