using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services;
using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace AutoHotkeyRemaster.WPF.Models
{
    //profile name 정보는 표시할 것. SwitchKeyCellModel은 리스트에 바인딩할 것
    public class ProfileSwitchKeyTableRowModel : PropertyChangedBase
    {
        private readonly ProfileSwitchKeyTableManager _profileSwitchKeyTable;
        private readonly HotkeyProfile _profile;

        public string Name
        {
            get
            {
                return _profile.ProfileName;
            }
            private set { }
        }
        public ObservableCollection<SwitchKeyCellModel> SwitchKeyCells { get; set; } = new ObservableCollection<SwitchKeyCellModel>();

        public ProfileSwitchKeyTableRowModel(ProfileSwitchKeyTableManager profileSwitchKeyTable, HotkeyProfile profile, int profileCount)
        {
            _profileSwitchKeyTable = profileSwitchKeyTable;
            _profile = profile;

            for (int i = 1; i <= profileCount; i++)
            {
                SwitchKeyCells.Add(new SwitchKeyCellModel(profileSwitchKeyTable, profile.ProfileNum, i));
            }
        }
    }
}
