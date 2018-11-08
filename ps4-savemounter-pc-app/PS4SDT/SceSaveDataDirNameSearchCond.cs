using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	public struct SceSaveDataDirNameSearchCond
	{
		[FieldOffset(0)]
		public int userId;

		[FieldOffset(8)]
		public ulong titleId;

		[FieldOffset(16)]
		public ulong dirName;

		[FieldOffset(24)]
		public uint key;

		[FieldOffset(28)]
		public uint order;
	}
}
