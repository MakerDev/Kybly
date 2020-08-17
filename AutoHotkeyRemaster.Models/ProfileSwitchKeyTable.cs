namespace AutoHotkeyRemaster.Models
{
    public class ProfileSwitchKeyTable
    {
        public int[][] SwitchKeyTable { get; set; } = new int[AppConstants.MAX_PROFILE][];

        public int this[int from, int to]
        {
            get
            {
                if (from >= AppConstants.MAX_PROFILE || from < 0 || to >= AppConstants.MAX_PROFILE || to < 0)
                {
                    return -1;
                }

                return SwitchKeyTable[from][to];
            }

            set
            {
                SwitchKeyTable[from][to] = value;
            }
        }

        public int[] this[int profile]
        {
            get
            {
                return SwitchKeyTable[profile];
            }

            private set { }
        }

        public ProfileSwitchKeyTable()
        {
            for (int i = 0; i < AppConstants.MAX_PROFILE; i++)
            {
                SwitchKeyTable[i] = new int[AppConstants.MAX_PROFILE];
            }
        }
    }
}
