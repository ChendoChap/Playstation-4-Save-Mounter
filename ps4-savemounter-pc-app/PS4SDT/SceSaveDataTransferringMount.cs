using System.Runtime.InteropServices;

namespace PS4SDT
{
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct SceSaveDataTransferringMount
	{
		public int userId;

		public ulong titleId;

		public ulong dirName;

		public ulong fingerprint;
	}
}
