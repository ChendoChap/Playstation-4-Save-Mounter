using System.Runtime.InteropServices;

namespace libdebug
{
	public struct acc
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public byte[] fp_bytes;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		private byte[] fp_pad;
	}
}
