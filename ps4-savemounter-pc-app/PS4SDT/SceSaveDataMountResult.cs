using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	public struct SceSaveDataMountResult
	{
		[FieldOffset(0)]
		public SceSaveDataMountPoint mountPoint;

		[FieldOffset(32)]
		public ulong requiredBlocks;

		[FieldOffset(40)]
		public uint mountStatus;
	}
}
