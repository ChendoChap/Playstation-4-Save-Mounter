using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Sequential, Size = 80)]
	public struct SceSaveDataFingerprint
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
		public byte[] data;
	}
}
