using AutoHotkeyRemaster.Models;
using AutoHotkeyRemaster.Services.Helpers;
using AutoHotkeyRemaster.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHotkeyRemaster.Services
{
    //기본적으로 이동키는 그 자리의 f# 키를 쓴다.
    /*  1   2   3   4   5
     *  -1  a   b   c   d   
     *   a  -1  b   c   d 
     *   a   b  -1  c   d
     *   a   b  c  -1   d
     *   a   b   c  d  -1
     * 
     * 행 = 현재 프로필, 열 = 다음 프로필
     */
    public class ProfileSwitchKeyTableManager : IAsyncInitializationRequired
    {
        private const int VK_F1 = 112;

        private const int MAX_PROFILE = AppConstants.MAX_PROFILE;

        private readonly IAsyncJsonFileManager _jsonSavefileManager;
        private readonly ProfileManager _profileManager;

        public ProfileSwitchKeyTable SwitchKeyTable { get; private set; }

        public ProfileSwitchKeyTableManager(IAsyncJsonFileManager jsonSavefileManager, ProfileManager profileManager)
        {
            _jsonSavefileManager = jsonSavefileManager;
            _profileManager = profileManager;
        }

        public async Task InitializeAsync()
        {
            var table = await _jsonSavefileManager.LoadAsync<ProfileSwitchKeyTable>("profile_switch_key_table").ConfigureAwait(false);

            if (table == null)
            {
                SwitchKeyTable = new ProfileSwitchKeyTable();
                ResetToDefault();
            }
            else
            {
                SwitchKeyTable = table;
            }
        }

        public bool HasKey(int key)
        {
            for (int i = 0; i < MAX_PROFILE; i++)
            {
                for (int j = 0; j < MAX_PROFILE; j++)
                {
                    if (SwitchKeyTable[i][j] == key)
                        return true;
                }
            }

            return false;
        }

        //키 중복등의 문제로 그 자리의 키를 기본전환키로 변경해야할시
        public void SetToDefaultByIndex(int from, int to)
        {
            if (from == to)
            {
                SwitchKeyTable[from][to] = -1;
            }
            else
            {
                SwitchKeyTable[from][to] = VK_F1 + to;
            }
        }

        //As We don't check whether there exists duplicate switch key when adding a hotkey,
        //here we don't check whether this profile has the same key with this swtich key.
        //As swtich key has higher priority in hook manager, the duplicated hotkey will not be added in hook.
        /// <summary>
        /// This method doesn't check if the newKey is same with the activation key, as when activation key is pressed,
        /// the setting window will autmatically be closed
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="newKey"></param>
        /// <returns>
        /// -1 : Collistion with other switch key <para />
        /// 0 : Collision with profile hotkey
        /// </returns>
        public int SetSwitchKeyByIndex(int from, int to, int newKey)
        {
            if (SwitchKeyTable[from].FirstOrDefault((key) => key == newKey) > 0)
                return -1;

            if (_profileManager.Profiles[from].HasHotkey(newKey))
                return 0;

            SwitchKeyTable[from][to] = newKey;

            return 1;
        }

        public async Task SaveTableAsync()
        {
            await _jsonSavefileManager.SaveAsync(SwitchKeyTable, "profile_switch_key_table").ConfigureAwait(false);
        }

        private void ResetToDefault()
        {
            for (int i = 0; i < MAX_PROFILE; i++)
            {
                for (int j = 0; j < MAX_PROFILE; j++)
                {
                    if (i == j)
                    {
                        SwitchKeyTable[i][i] = -1;  //자기 자신으로의 키는 -1로 설정해 둠.
                    }
                    else
                    {
                        SwitchKeyTable[i][j] = VK_F1 + j;
                    }
                }
            }
        }
    }
}
