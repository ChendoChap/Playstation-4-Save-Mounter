using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	public struct SceSaveDataMount2
	{
		[FieldOffset(0)]
		public int userId;

		[FieldOffset(8)]
		public ulong dirName;

		[FieldOffset(16)]
		public ulong blocks;

		[FieldOffset(24)]
		public uint mountMode;
	}
}
