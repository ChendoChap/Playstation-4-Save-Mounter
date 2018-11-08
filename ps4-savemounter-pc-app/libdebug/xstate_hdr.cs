using System.Runtime.InteropServices;

namespace libdebug
{
	public struct xstate_hdr
	{
		public ulong xstate_bv;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		private byte[] xstate_rsrv0;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
		private byte[] xstate_rsrv;
	}
}
