using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BatteryDischarger.PlatformSpecificActions
{
    public static class PlatformSpecificActionsManager
    {
        private static APlatformSpecificActions _PlatformSpecificEndActions { get; set; }

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

                default:
                    throw new NotSupportedException();
            }
        }

        public static void TryCatchStartProcess(string fileName)
        {
            try { Process.Start(fileName); } catch { }
        }

        public static void TryCatchStartProcess(string fileName, string arguments)
        {
            try { Process.Start(fileName, arguments); } catch { }
        }

        public static void TryCatchStartProcess(string fileName, IEnumerable<string> arguments)
        {
            try { Process.Start(fileName, arguments); } catch { }
        }
    }
}