namespace libdebug
{
	public class ProcessList
	{
		public Process[] processes;

		public ProcessList(int number, string[] names, int[] pids)
		{
			processes = new Process[number];
			for (int i = 0; i < number; i++)
			{
				processes[i] = new Process(names[i], pids[i]);
			}
		}

		public Process FindProcess(string name, bool contains = false)
		{
			Process[] array = processes;
			foreach (Process process in array)
			{
				if (contains)
				{
					if (process.name.Contains(name))
					{
						return process;
					}
				}
				else if (process.name == name)
				{
					return process;
				}
			}
			return null;
		}
	}
}
