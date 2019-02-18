using System;
using System.Collections.Generic;
using System.Text;

namespace PS4Saves
{
    class offsets
    {
        public const int sceUserServiceGetInitialUser = 0x33B0;
        public const int sceUserServiceGetLoginUserIdList = 0x2B40;
        public const int sceUserServiceGetUserName = 0x3F20;

        public const int sceSaveDataMount = 0x248D0;
        public const int sceSaveDataUmount = 0x250C0;
        public const int sceSaveDataDirNameSearch = 0x25CA0;
        public const int sceSaveDataInitialize3 = 0x24740;
    }
}
