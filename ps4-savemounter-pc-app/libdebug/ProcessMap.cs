namespace libdebug
{
	public class ProcessMap
	{
		public int pid;

		public MemoryEntry[] entries;

		public ProcessMap(int pid, MemoryEntry[] entries)
		{
			this.pid = pid;
			this.entries = entries;
		}

		public MemoryEntry FindEntry(string name, bool contains = false)
		{
			MemoryEntry[] array = entries;
			foreach (MemoryEntry memoryEntry in array)
			{
				if (contains)
				{
					if (memoryEntry.name.Contains(name))
					{
						return memoryEntry;
					}
				}
				else if (memoryEntry.name == name)
				{
					return memoryEntry;
				}
			}
			return null;
		}

		public MemoryEntry FindEntry(ulong size)
		{
			MemoryEntry[] array = entries;
			foreach (MemoryEntry memoryEntry in array)
			{
				if (memoryEntry.start - memoryEntry.end == size)
				{
					return memoryEntry;
				}
			}
			return null;
		}
	}
}
