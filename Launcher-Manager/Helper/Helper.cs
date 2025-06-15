using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher_Manager.Helper
{
    public static class Helper
    {
        public static (double cpuTimeMs, double ramMb)? GetUsageFromPid(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                if (process.HasExited) return null;

                process.Refresh();
                return (
                    cpuTimeMs: process.TotalProcessorTime.TotalMilliseconds,
                    ramMb: process.WorkingSet64 / (1024.0 * 1024.0)
                );
            }
            catch
            {
                return null; // process not found
            }
        }

        public static async Task<(double cpuPercent, double ramMb)?> GetUsageFromPidAsync(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                if (process.HasExited) return null;

                var startCpuTime = process.TotalProcessorTime;
                var startTime = DateTime.UtcNow;

                await Task.Delay(500);

                process.Refresh();

                var endCpuTime = process.TotalProcessorTime;
                var endTime = DateTime.UtcNow;

                var cpuUsedMs = (endCpuTime - startCpuTime).TotalMilliseconds;
                var totalTimePassedMs = (endTime - startTime).TotalMilliseconds;

                int coreCount = Environment.ProcessorCount;
                double cpuPercent = (cpuUsedMs / (totalTimePassedMs * coreCount)) * 100.0;

                double ramMb = process.WorkingSet64 / (1024.0 * 1024.0);

                return (cpuPercent, ramMb);  // ควรจะไม่ error
            }
            catch
            {
                return null;
            }
        }


    }
}
