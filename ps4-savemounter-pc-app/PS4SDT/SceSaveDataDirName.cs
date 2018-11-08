using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Sequential, Size = 32)]
	public struct SceSaveDataDirName
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string data;
	}
}
