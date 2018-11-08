using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	public struct SceSaveDataMountPoint
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string data;
	}
}
