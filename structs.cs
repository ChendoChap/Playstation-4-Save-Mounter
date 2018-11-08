using System.Runtime.InteropServices;

namespace PS4SDT
{
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct SceSaveDataMount2
    {
        [FieldOffset(0x0)] public int userId;
        [FieldOffset(0x8)] public ulong dirName;
        [FieldOffset(0x10)] public ulong blocks;
        [FieldOffset(0x18)] public uint mountMode;
    }
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct SceSaveDataMountResult
    {
        [FieldOffset(0x0)] public SceSaveDataMountPoint mountPoint;
        [FieldOffset(0x20)] public ulong requiredBlocks;
        [FieldOffset(0x28)] public uint mountStatus;

    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct SceSaveDataMountPoint
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string data;
    }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    public struct SceSaveDataTransferringMount
    {
        public int userId;
        public ulong titleId;
        public ulong dirName;
        public ulong fingerprint;
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct SceSaveDataTitleId
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string data;
    }
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct SceSaveDataDirName
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string data;
    }
    [StructLayout(LayoutKind.Sequential, Size = 80)]
    public struct SceSaveDataFingerprint
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
        public byte[] data;
    }
    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct SceSaveDataDirNameSearchCond
    {
        [FieldOffset(0x0)] public int userId;
        [FieldOffset(0x8)] public ulong titleId;
        [FieldOffset(0x10)] public ulong dirName;
        [FieldOffset(0x18)] public uint key;
        [FieldOffset(0x1C)] public uint order;
    }
    [StructLayout(LayoutKind.Explicit, Size = 56)]
    public struct SceSaveDataDirNameSearchResult
    {
        [FieldOffset(0x0)] public uint hitNum;
        [FieldOffset(0x8)] public ulong dirNames;
        [FieldOffset(0x10)] public uint dirNamesNum;
        [FieldOffset(0x14)] public uint setNum;
        [FieldOffset(0x18)] public ulong param;
        [FieldOffset(0x20)] public ulong infos;
    }
    [StructLayout(LayoutKind.Explicit, Size = 1328)]
    public struct SceSaveDataParam
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        [FieldOffset(0x0)] public string title;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        [FieldOffset(0x80)] public string subTitle;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        [FieldOffset(0x100)] public string detail;
        [FieldOffset(0x500)] public uint userParam;
        [FieldOffset(0x508)] public long mtime;
    }
    [StructLayout(LayoutKind.Sequential, Size = 48)]
    public struct SceSaveDataSearchInfo
    {
        public ulong blocks;
        public ulong freeBlocks;
    }
}
