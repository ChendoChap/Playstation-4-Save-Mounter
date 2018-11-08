using System.Runtime.InteropServices;

namespace libdebug
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ThreadInfo
	{
		public int pid;

		public int priority;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string name;
	}
}
