namespace libdebug
{
	public class MemoryEntry
	{
		public string name;

		public ulong start;

		public ulong end;

		public ulong offset;

		public uint prot;

		public override string ToString()
		{
			return $"{name} 0x{start:X}";
		}
	}
}
