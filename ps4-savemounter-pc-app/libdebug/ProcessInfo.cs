using System.Runtime.InteropServices;

namespace libdebug
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ProcessInfo
	{
		public int pid;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
		public string name;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string path;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string titleid;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string contentid;
	}
}
