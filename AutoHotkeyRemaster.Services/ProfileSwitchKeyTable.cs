using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

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
        private const int VK_ESC = 27;

        private const int MAX_PROFILE = ProfileManager.MAX_PROFILE_NUM;

        private readonly int[][] _switchKeyTable = new int[MAX_PROFILE][];

        private int _escapeKey;

        public int this[int from, int to]
        {
            get
            {
                if (from >= MAX_PROFILE || from < 0 || to >= MAX_PROFILE || to < 0)
                {
                    return -1;
                }

                return _switchKeyTable[from][to];
            }

            private set { }
        }

        public int[] this[int profile]
        {
            get
            {
                return _switchKeyTable[profile];
            }

            private set { }
        }

        public ProfileSwitchKeyTable()
        {
            for (int i = 0; i < MAX_PROFILE; i++)
            {
                _switchKeyTable[i] = new int[MAX_PROFILE];
            }

            if (!LoadFromFile())
            {
                ResetToDefault();
            }
        }

        public int GetEscapeKey()
        {
            return _escapeKey;
        }

        public void SetEscapeKey(int key)
        {
            _escapeKey = key;
        }

        //키 중복등의 문제로 그 자리의 키를 기본전환키로 변경해야할 시
        public void SetToDefaultByIndex(int from, int to)
        {
            if (from == to)
            {
                _switchKeyTable[from][to] = -1;
            }
            else
            {
                _switchKeyTable[from][to] = VK_F1 + to;
            }
        }

        public bool SetSwitchKeyByIndex(int from, int to, int newKey)
        {
            //TODO : 중복처리
            if (from == to)
                return false;

            _switchKeyTable[from][to] = newKey;

            return true;
        }

        public void SaveKeys()
        {
            string path = Environment.CurrentDirectory + "/SaveFiles/" + "ProfileChangeKeys" + ".json";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            List<int> keyList = new List<int>();

            for (int i = 0; i < MAX_PROFILE; i++)
            {
                for (int j = 0; j < MAX_PROFILE; j++)
                {
                    keyList.Add(_switchKeyTable[i][j]);
                }
            }

            keyList.Add(_escapeKey);

            using (FileStream filestream = File.OpenWrite(path))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<int>));
                serializer.WriteObject(filestream, keyList);
            }
        }

        private bool LoadFromFile()
        {
            //파일 존재 여부 반드시 체크
            string path = Environment.CurrentDirectory + "/SaveFiles/" + "ProfileChangeKeys" + ".json";

            List<int> keyList;

            if (!File.Exists(path))
                return false;

            using (FileStream filestream = File.OpenRead(path))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<int>));
                keyList = serializer.ReadObject(filestream) as List<int>;
            }

            for (int i = 0; i < MAX_PROFILE; i++)
            {
                for (int j = 0; j < MAX_PROFILE; j++)
                {
                    _switchKeyTable[i][j] = keyList[10 * i + j];
                }
            }

            _escapeKey = keyList[MAX_PROFILE * MAX_PROFILE];

            return true;
        }

        private void ResetToDefault()
        {
            for (int i = 0; i < MAX_PROFILE; i++)
            {
                for (int j = 0; j < MAX_PROFILE; j++)
                {
                    if (i == j)
                    {
                        _switchKeyTable[i][i] = -1;  //자기 자신으로의 키는 -1로 설정해 둠.
                    }
                    else
                    {
                        _switchKeyTable[i][j] = VK_F1 + j;
                    }
                }
            }

            _escapeKey = VK_ESC;
        }

    }
}
