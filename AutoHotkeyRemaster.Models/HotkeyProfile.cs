using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutoHotkeyRemaster.Models
{
    public class HotkeyProfile
    {
        public const int MAX_HOTKEY = 60;

        private string _profileName = null;
        public string ProfileName
        {
            get
            {
                return _profileName ?? $"Profile{ProfileNum}";

            }
            set { _profileName = value; }
        }

        public List<Hotkey> Hotkeys { get; set; } = new List<Hotkey>();

        [JsonIgnore]
        public int ProfileNum { get; set; }
        [JsonIgnore]
        public int HotkeyCount
        {
            get { return Hotkeys.Count; }
            private set { }
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
            if (HotkeyCount >= MAX_HOTKEY)
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

        /// <summary>
        /// Add hotkey if it was not present in this profile. If it had an existing one,
        /// replace it with the new hotkey
        /// </summary>
        /// <param name="hotkey"></param>
        /// <returns> -1 : No more hotkey is available <para />
        /// 0 : Existing hotkey is editted <para />
        /// 1 : New hotkey is added <para />
        /// </returns>
        public int AddOrEditHotkeyIfExisting(Hotkey hotkey)
        {
            Hotkey hotkeyBefore = FindHotkeyOrNull(hotkey);

            if (hotkeyBefore != null)
            {
                hotkeyBefore.Action = hotkey.Action;
                hotkeyBefore.EndingAction = hotkey.EndingAction;

                return 0;
            }

            if (HotkeyCount >= MAX_HOTKEY)
            {
                return -1;
            }

            Hotkeys.Add(hotkey);

            return 1;
        }

        public bool DeleteHotkeyIfExisting(Hotkey hotkey)
        {
            Hotkey hotkeyBefore = FindHotkeyOrNull(hotkey);

            if (hotkeyBefore == null)
            {
                return false;
            }

            Hotkeys.Remove(hotkeyBefore);

            return true;
        }

        public bool HasHotkey(int trigger)
        {
            foreach (var hotkey in Hotkeys)
            {
                if (hotkey.Trigger.Key == trigger)
                    return true;
            }

            return false;
        }

        public bool HasHotkey(Hotkey hotkeyToCheck)
        {
            foreach (var hotkey in Hotkeys)
            {
                if (hotkeyToCheck.Trigger == hotkey.Trigger)
                    return true;
            }

            return false;
        }

        public Hotkey FindHotkeyOrNull(Hotkey hotkeyToCheck)
        {
            foreach (var hotkey in Hotkeys)
            {
                if (hotkeyToCheck.Trigger == hotkey.Trigger)
                    return hotkey;
            }

            return null;
        }
    }
}
