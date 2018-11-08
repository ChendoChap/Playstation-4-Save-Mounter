using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Sequential, Size = 48)]
	public struct SceSaveDataSearchInfo
	{
		public ulong blocks;

		public ulong freeBlocks;
	}
}
