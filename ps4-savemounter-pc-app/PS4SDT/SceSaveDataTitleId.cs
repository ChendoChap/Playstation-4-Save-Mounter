using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	public struct SceSaveDataTitleId
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
		public string data;
	}
}
