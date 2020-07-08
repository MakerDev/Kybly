using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;
using System.Windows.Xps;
using System.Runtime.Serialization.Json;

namespace AutoHotkeyRemaster.Models
{
    public class HotkeyProfile
    {
        public const int MAX_HOTKEY = 60;

        public string ProfileName { get; set; } = null;
        public List<Hotkey> Hotkeys { get; private set; } = new List<Hotkey>();


        [JsonIgnore]
        public int ProfileNum { get; set; }
        [JsonIgnore]
        public int HotkeyCount
        {
            get { return Hotkeys.Count; }
            private set { }
        }

        private HotkeyProfile() { }

        public static HotkeyProfile CreateNewProfile(int profileNum, string profileName = null)
        {
            HotkeyProfile profile = new HotkeyProfile();
            profile.ProfileNum = profileNum;
            profile.ProfileName = profileName;

            return profile;
        }

        /// <summary>
        /// Add new hotkey to profile
        /// </summary>
        /// <param name="hotkey"></param>
        /// <returns>
        ///-1 : profile already full <para />
        /// 0 : Hotkey already exists. <para />
        /// otherwise : return the number of hotkeys in the profile
        ///</returns>
        public int AddHotkey(Hotkey hotkey)
        {
            if (HotkeyCount > MAX_HOTKEY)
            {
                return -1;
            }
            else if (HasHotkey(hotkey))
            {
                return 0;
            }

            Hotkeys.Add(hotkey);

            return HotkeyCount;
        }

        public bool HasHotkey(Hotkey hotkeyToCheck)
        {
            foreach (var hotkey in Hotkeys)
            {
                if (hotkeyToCheck == hotkey)
                    return true;
            }

            return false;
        }

        public void Save(string filename)
        {
            string path = Environment.CurrentDirectory + "/SaveFiles/" + filename + ".json";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            //TODO : delete legacy code
            //using (FileStream filestream = File.OpenWrite(path))
            //{
            //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Hotkey>));
            //    serializer.WriteObject(filestream, Hotkeys);
            //}

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(path, jsonString);
        }

        public void Delete(string filename)
        {
            string path = Environment.CurrentDirectory + "/SaveFiles/" + filename + ".json";

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static HotkeyProfile LoadFromFile(int profileNum)
        {
            string filename = $"profile{profileNum}";
            string path = Environment.CurrentDirectory + "/SaveFiles/" + filename + ".json";

            if (!File.Exists(path))
            {
                return null;
            }

            //TODO : delete legacy code 
            //using (FileStream filestream = File.OpenRead(path))
            //{
            //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Hotkey>));
            //    profile.Hotkeys = serializer.ReadObject(filestream) as List<Hotkey>;
            //}

            string jsonString = File.ReadAllText(path);
            HotkeyProfile profile = JsonSerializer.Deserialize<HotkeyProfile>(jsonString);
            profile.ProfileNum = profileNum;

            return profile;
        }
    }
}
