using System.Runtime.InteropServices;

namespace libdebug
{
	[StructLayout(LayoutKind.Sequential, Pack = 64)]
	public struct fpregs
	{
		public envxmm svn_env;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public acc[] sv_fp;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public xmmacc[] sv_xmm;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
		private byte[] sv_pad;

		public savefpu_xstate sv_xstate;
	}
}
