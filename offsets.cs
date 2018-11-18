using System;
using System.Collections.Generic;
using System.Text;

namespace PS4SDT
{
    class offsets
    {
        public const int sceUserServiceGetInitialUser = 0x33B0;

        public const int sceSaveDataMount2 = 0x24BE0;
        public const int sceSaveDataUmount = 0x250C0;
        public const int sceSaveDataDirNameSearch = 0x25CA0;
        public const int sceSaveDataTransferringMount = 0x24F70;
		
        public const int malloc = 0x23D90;
        public const int free = 0x23E20;
    }
}
