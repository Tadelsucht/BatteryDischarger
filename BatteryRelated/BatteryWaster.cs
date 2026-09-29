using Hardware.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryDischarger.BatteryRelated
{
    // Adds optional low-priority CPU work to increase discharge while the controlled-discharge monitor runs.
    public class BatteryWaster
    {
        // Used when the hardware provider cannot report a single CPU load sample.
        private const int DefaultNumberOfWasteThreads = 8;
        // Per-CPU sample threshold used to add a worker or remove one from the controller's tracking set.
        private const ulong TargetProcessorTimeInPercent = 50;
        // The provider is shared because CPU sampling and worker control are process-wide operations here.
        private static readonly IHardwareInfo hardwareInfo = new HardwareInfo();
        // Shared stop flag observed by both the controller and worker loops.
        public static bool HasBeenStopped { get; protected set; } = false;

        // Signals all active waste loops to exit at their next stop check.
        public void Stop()
        { HasBeenStopped = true; }

        // Allows a later acceleration run to use the shared worker-stop flag again.
        public void Reset()
        { HasBeenStopped = false; }

        // Names queued by the existing controller for worker-abort bookkeeping.
        // Records worker names removed from the controller set; worker threads do not consume this list here.
        public List<string> AbortThreadNameList = new List<string>();

        // Samples CPU load in the background and starts or retires low-priority workers toward the target.
        public void StartWasting()
        {
            if (!HasBeenStopped)
            {
                var task = Task.Run(() =>
                {
                    Dictionary<string, Thread> wasteThread = new Dictionary<string, Thread>();
                    while (HasBeenStopped == false)
                    {
                        hardwareInfo.RefreshCPUList(true);
                        if (hardwareInfo.CpuList.Count <= 0 || hardwareInfo.CpuList.Count > 1)
                        {
                            if (wasteThread.Count >= DefaultNumberOfWasteThreads) continue;
                            for (int i = 0; i < DefaultNumberOfWasteThreads; i++)
                            {
                                var name = Guid.NewGuid().ToString();
                                wasteThread.Add(name, StartWasteThread(name));
                            }
                            continue;
                        }
                        foreach (CPU cpu in hardwareInfo.CpuList)
                        {
                            if (cpu.PercentProcessorTime > 0) // If PercentProcessorTime could not be gathered
                            {
                                if (cpu.PercentProcessorTime < TargetProcessorTimeInPercent)
                                {
                                    var name = Guid.NewGuid().ToString();
                                    wasteThread.Add(name, StartWasteThread(name));
                                    AbortThreadNameList.Clear();
                                }
                                else
                                {
                                    if (wasteThread.Count > 0)
                                    {
                                        var name = wasteThread.First().Key;
                                        wasteThread.Remove(name);
                                        AbortThreadNameList.Add(name);
                                    }
                                }
                            }
                        }
                        Thread.Sleep(2000);
                    }
                });
            }
        }

        // Starts one low-priority worker tracked by the caller; the loop exits only when the shared stop flag is set.
        private Thread StartWasteThread(string name)
        {
            var thread = new Thread(() =>
             {
                 while (HasBeenStopped == false)
                 {
                     // Waste loop
                 }
             });
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
            return thread;
        }
    }
}
