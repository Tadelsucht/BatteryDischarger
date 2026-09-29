using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BatteryDischarger.PlatformSpecificActions
{
    // Selects the current OS adapter and routes configured end actions to its best-effort implementation.
    public static class PlatformSpecificActionsManager
    {
        // Optional adapter override; while unset, the getter selects an adapter from the current host each time.
        private static APlatformSpecificActions _PlatformSpecificEndActions { get; set; }

        // Creates the adapter for the detected host; unknown hosts use the combined fallback adapter.
        public static APlatformSpecificActions PlatformSpecificEndActions
        {
            get
            {
                if (_PlatformSpecificEndActions is not null) return _PlatformSpecificEndActions;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new PlatformSpecificActionsWindows();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return new PlatformSpecificActionsLinux();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return new PlatformSpecificActionsOSX();
                }
                else
                {
                    return new PlatformSpecificActionsAllCombined();
                }
            }
        }

        // Maps the persisted action to one platform request; DoNothing deliberately makes no OS call.
        public static void TryExecuteEndAction(EndActionEnum endAction)
        {
            switch (endAction)
            {
                case EndActionEnum.Shutdown:
                    PlatformSpecificEndActions.TryShutdown();
                    break;

                case EndActionEnum.Sleep:
                    PlatformSpecificEndActions.TrySleep();
                    break;

                case EndActionEnum.Hibernate:
                    PlatformSpecificEndActions.TryHibernate();
                    break;

                case EndActionEnum.DoNothing:
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        // Starts a best-effort process without waiting for it or verifying its operating-system effect.
        public static void TryCatchStartProcess(string fileName)
        {
            try { Process.Start(fileName); } catch { }
        }

        // Starts a process with one argument string; startup errors are swallowed for legacy callers.
        public static void TryCatchStartProcess(string fileName, string arguments)
        {
            try { Process.Start(fileName, arguments); } catch { }
        }

        // Starts a process with separate arguments; success does not mean the requested action completed.
        public static void TryCatchStartProcess(string fileName, IEnumerable<string> arguments)
        {
            try { Process.Start(fileName, arguments); } catch { }
        }
    }
}
