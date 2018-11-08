using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Explicit, Size = 56)]
	public struct SceSaveDataDirNameSearchResult
	{
		[FieldOffset(0)]
		public uint hitNum;

		[FieldOffset(8)]
		public ulong dirNames;

		[FieldOffset(16)]
		public uint dirNamesNum;

		[FieldOffset(20)]
		public uint setNum;

		[FieldOffset(24)]
		public ulong param;

		[FieldOffset(32)]
		public ulong infos;
	}
}
