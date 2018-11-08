using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Explicit, Size = 1328)]
	public struct SceSaveDataParam
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string title;

		[FieldOffset(128)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string subTitle;

		[FieldOffset(256)]
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string detail;

		[FieldOffset(1280)]
		public uint userParam;

		[FieldOffset(1288)]
		public long mtime;
	}
}
