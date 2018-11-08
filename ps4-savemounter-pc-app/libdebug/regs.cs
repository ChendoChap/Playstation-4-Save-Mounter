using System.Runtime.InteropServices;

namespace libdebug
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct regs
	{
		public ulong r_r15;

		public ulong r_r14;

		public ulong r_r13;

		public ulong r_r12;

		public ulong r_r11;

		public ulong r_r10;

		public ulong r_r9;

		public ulong r_r8;

		public ulong r_rdi;

		public ulong r_rsi;

		public ulong r_rbp;

		public ulong r_rbx;

		public ulong r_rdx;

		public ulong r_rcx;

		public ulong r_rax;

		public uint r_trapno;

		public ushort r_fs;

		public ushort r_gs;

		public uint r_err;

		public ushort r_es;

		public ushort r_ds;

		public ulong r_rip;

		public ulong r_cs;

		public ulong r_rflags;

		public ulong r_rsp;

		public ulong r_ss;
	}
}
