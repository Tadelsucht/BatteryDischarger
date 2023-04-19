using Hardware.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BatteryDischarger.BatteryRelated
{
    public class BatteryWaster
    {
        private const int DefaultNumberOfWasteThreads = 8;
        private const ulong TargetProcessorTimeInPercent = 50;
        private static readonly IHardwareInfo hardwareInfo = new HardwareInfo();
        public static bool HasBeenStopped { get; protected set; } = false;

        public void Stop()
        { HasBeenStopped = true; }

        public void Reset()
        { HasBeenStopped = false; }

        public List<string> AbortThreadNameList = new List<string>();

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