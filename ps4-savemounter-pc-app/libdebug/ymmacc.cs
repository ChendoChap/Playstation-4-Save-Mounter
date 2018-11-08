using System.Runtime.InteropServices;

namespace libdebug
{
	public struct ymmacc
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] ymm_bytes;
	}
}
