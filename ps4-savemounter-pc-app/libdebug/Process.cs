namespace libdebug
{
	public class Process
	{
		public string name;

		public int pid;

		public Process(string name, int pid)
		{
			this.name = name;
			this.pid = pid;
		}

		public override string ToString()
		{
			return $"[{pid}] {name}";
		}
	}
}
