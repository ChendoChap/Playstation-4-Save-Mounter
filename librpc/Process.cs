/* golden */
/* 2/12/2018 */

namespace librpc
{
    public class Process
    {
        public string name;
        public int pid;

        /// <summary>
        /// Initializes Process class
        /// </summary>
        /// <param name="name">Process name</param>
        /// <param name="pid">Process ID</param>
        /// <returns></returns>
        public Process(string name, int pid)
        {
            this.name = name;
            this.pid = pid;
        }
        public override string ToString()
        {
            return $"[{pid}] - {name}";
        }
    }

    public class ProcessList
    {
        public Process[] processes;

        /// <summary>
        /// Initializes ProcessList class
        /// </summary>
        /// <param name="number">Number of processes</param>
        /// <param name="names">Process names</param>
        /// <param name="pids">Process IDs</param>
        /// <returns></returns>
        public ProcessList(int number, string[] names, int[] pids)
        {
            processes = new Process[number];
            for (int i = 0; i < number; i++)
            {
                processes[i] = new Process(names[i], pids[i]);
            }
        }

        /// <summary>
        /// Finds a process based off name
        /// </summary>
        /// <param name="name">Process name</param>
        /// <param name="contains">Condition to check if process name contains name</param>
        /// <returns></returns>
        public Process FindProcess(string name, bool contains = false)
        {
            foreach (Process p in processes)
            {
                if(contains)
                {
                    if (p.name.Contains(name))
                    {
                        return p;
                    }
                }
                else
                {
                    if (p.name == name)
                    {
                        return p;
                    }
                }
            }

            return null;
        }
    }

    public class MemoryEntry
    {
        public string name;
        public ulong start;
        public ulong end;
        public ulong offset;
        public uint prot;
    }

    public class ProcessInfo
    {
        public int pid;
        public MemoryEntry[] entries;

        /// <summary>
        /// Initializes ProcessInfo class with memory entries and process ID
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="entries">Process memory entries</param>
        /// <returns></returns>
        public ProcessInfo(int pid, MemoryEntry[] entries)
        {
            this.pid = pid;
            this.entries = entries;
        }

        /// <summary>
        /// Finds a virtual memory entry based off name
        /// </summary>
        /// <param name="name">Virtual memory entry name</param>
        /// <returns></returns>
        public MemoryEntry FindEntry(string name)
        {
            foreach (MemoryEntry entry in entries)
            {
                if (entry.name == name)
                {
                    return entry;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a virtual memory entry based off size
        /// </summary>
        /// <param name="size">Virtual memory entry size</param>
        /// <returns></returns>
        public MemoryEntry FindEntry(ulong size)
        {
            foreach (MemoryEntry entry in entries)
            {
                if ((entry.start - entry.end) == size)
                {
                    return entry;
                }
            }

            return null;
        }
    }
}
