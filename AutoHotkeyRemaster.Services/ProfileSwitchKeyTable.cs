﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoHotkeyRemaster.Models.Helpers;
using AutoHotkeyRemaster.Services.Helpers;

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
    public class ProfileSwitchKeyTable
    {
        private const int VK_F1 = 112;

        private const int MAX_PROFILE = ProfileManager.MAX_PROFILE_NUM;
        private readonly IAsyncJsonFileManager _jsonSavefileManager;

        public int[][] SwitchKeyTable { get; set; } = new int[MAX_PROFILE][];

        public int this[int from, int to]
        {
            get
            {
                if (from >= MAX_PROFILE || from < 0 || to >= MAX_PROFILE || to < 0)
                {
                    return -1;
                }

                return SwitchKeyTable[from][to];
            }

            private set { }
        }

        public int[] this[int profile]
        {
            get
            {
                return SwitchKeyTable[profile];
            }

            private set { }
        }

        /// <summary>
        /// must call InitializeTable to use
        /// </summary>
        /// <param name="jsonSavefileManager"></param>
        public ProfileSwitchKeyTable(IAsyncJsonFileManager jsonSavefileManager)
        {
            _jsonSavefileManager = jsonSavefileManager;            
        }

        public async Task InitializeTable()
        {
            for (int i = 0; i < MAX_PROFILE; i++)
            {
                SwitchKeyTable[i] = new int[MAX_PROFILE];
            }

            var table = await _jsonSavefileManager.LoadAsync<ProfileSwitchKeyTable>("profile_switch_key_table");

            if (table == null)
            {
                ResetToDefault();
            }
            else
            {
                SwitchKeyTable = table.SwitchKeyTable;
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
        public bool SetSwitchKeyByIndex(int from, int to, int newKey)
        {
            //TODO : 중복처리
            if (from == to || SwitchKeyTable[from].FirstOrDefault((key) => key == newKey) > 0)
                return false;

            SwitchKeyTable[from][to] = newKey;

            return true;
        }

        public async Task SaveTableAsync()
        {
            await _jsonSavefileManager.SaveAsync(this, "profile_switch_key_table");
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
