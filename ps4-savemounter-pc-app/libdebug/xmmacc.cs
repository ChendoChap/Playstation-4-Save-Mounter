using System.Runtime.InteropServices;

namespace libdebug
{
	public struct xmmacc
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] xmm_bytes;
	}
}
