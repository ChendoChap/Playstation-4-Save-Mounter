using System.Runtime.InteropServices;

namespace libdebug
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct dbregs
	{
		public ulong dr0;

		public ulong dr1;

		public ulong dr2;

		public ulong dr3;

		public ulong dr4;

		public ulong dr5;

		public ulong dr6;

		public ulong dr7;

		public ulong dr8;

		public ulong dr9;

		public ulong dr10;

		public ulong dr11;

		public ulong dr12;

		public ulong dr13;

		public ulong dr14;

		public ulong dr15;
	}
}
