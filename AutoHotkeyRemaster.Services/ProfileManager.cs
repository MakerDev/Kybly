using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace AutoHotkeyRemaster.Services
{
    public class ProfileManager
    {
        public const int MAX_PROFILE_NUM = 10;

        public int ProfileCount
        {
            get
            {
                return Profiles.Count;
            }
            private set { }
        }

        public List<HotkeyProfile> Profiles { get; private set; } = new List<HotkeyProfile>();

        public ProfileManager()
        {
            LoadAllProfiles();
        }

        public HotkeyProfile FindProfileOrDefault(int profileNum)
        {
            if (profileNum < 0)
                return Profiles[0];

            foreach (var profile in Profiles)
            {
                if (profile.ProfileNum == profileNum)
                    return profile;
            }

            //TODO : 디폴트 프로필 커스터마이징 가능하도록 하기
            return Profiles[0];
        }

        public HotkeyProfile CreateNewProfile(string profileName = null)
        {
            HotkeyProfile profile =
                HotkeyProfile.CreateNewProfile(ProfileCount+1, profileName);

            Profiles.Add(profile);

            return profile;
        }

        public HotkeyProfile DeleteProfile(int profileNum)
        {
            int profileIdx = profileNum - 1;

            HotkeyProfile deletedProfile = Profiles[profileIdx];

            foreach (var profile in Profiles)
            {
                profile.Delete($"profile{profile.ProfileNum}");
            }

            Profiles.RemoveAt(profileIdx);

            for (int i = profileIdx; i < ProfileCount; i++)
            {
                Profiles[i].ProfileNum -= 1;
            }

            SaveAllProfiles();

            return deletedProfile;
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
            for (int i = 0; i < MAX_PROFILE_NUM; i++)
            {
                HotkeyProfile profile = HotkeyProfile.LoadFromFile(i + 1);

                if (profile == null)
                    break;

                Profiles.Add(profile);
            }
        }
    }
}
