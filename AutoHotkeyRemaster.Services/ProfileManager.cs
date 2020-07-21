using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Models.Helpers;
using AutoHotkeyRemaster.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoHotkeyRemaster.Services
{
    public class ProfileManager
    {
        public const int MAX_PROFILE_NUM = 10;
        private readonly IAsyncJsonFileManager _jsonSavefileManager;

        public int ProfileCount
        {
            get
            {
                return Profiles.Count;
            }
            private set { }
        }

        public List<HotkeyProfile> Profiles { get; private set; } = new List<HotkeyProfile>();


        /// <summary>
        /// Must explicitly call LoadAllProfileAsync to initialize
        /// </summary>
        /// <param name="jsonSavefileManager"></param>
        public ProfileManager(IAsyncJsonFileManager jsonSavefileManager)
        {
            _jsonSavefileManager = jsonSavefileManager;
        }

        public async Task LoadAllProfilesAsync()
        {
            for (int i = 0; i < MAX_PROFILE_NUM; i++)
            {
                HotkeyProfile profile = await LoadProfileFromFileAsync(i + 1);

                if (profile == null)
                    break;

                Profiles.Add(profile);
            }
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

        public async Task SaveProfileAsync(HotkeyProfile profile)
        {
            await _jsonSavefileManager.SaveAsync(profile, $"profile{profile.ProfileNum}");
        }

        public HotkeyProfile CreateNewProfile(string profileName = null)
        {
            HotkeyProfile profile = new HotkeyProfile
            {
                ProfileNum = ProfileCount + 1,
                ProfileName = profileName
            };

            Profiles.Add(profile);

            return profile;
        }

        public async Task<HotkeyProfile> DeleteProfileAsync(int profileNum)
        {
            int profileIdx = profileNum - 1;

            HotkeyProfile deletedProfile = Profiles[profileIdx];

            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (var profile in Profiles)
            {
                tasks.Add(_jsonSavefileManager.DeleteIfExistsAsync($"profile{profile.ProfileNum}"));
            }

            await Task.WhenAll(tasks);

            Profiles.RemoveAt(profileIdx);

            for (int i = profileIdx; i < ProfileCount; i++)
            {
                Profiles[i].ProfileNum -= 1;
            }

            await SaveAllProfileAsync();

            return deletedProfile;
        }

        public async Task SaveAllProfileAsync()
        {
            List<Task> tasks = new List<Task>();

            foreach (var profile in Profiles)
            {
                tasks.Add(_jsonSavefileManager.SaveAsync(profile, $"profile{profile.ProfileNum}"));
            }

            await Task.WhenAll(tasks);
        }


        private async Task<HotkeyProfile> LoadProfileFromFileAsync(int profileNum)
        {
            var profile = await _jsonSavefileManager.LoadAsync<HotkeyProfile>($"profile{profileNum}");

            if (profile != null) profile.ProfileNum = profileNum;

            return profile;
        }
    }
}
