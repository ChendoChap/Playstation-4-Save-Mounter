namespace libdebug
{
	public struct envxmm
	{
		public ushort en_cw;

		public ushort en_sw;

		public byte en_tw;

		public byte en_zero;

		public ushort en_opcode;

		public ulong en_rip;

		public ulong en_rdp;

		public uint en_mxcsr;

		public uint en_mxcsr_mask;
	}
}
