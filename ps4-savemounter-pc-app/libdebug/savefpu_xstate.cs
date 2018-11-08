using System.Runtime.InteropServices;

namespace libdebug
{
	public struct savefpu_xstate
	{
		public xstate_hdr sx_hd;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public ymmacc[] sx_ymm;
	}
}
