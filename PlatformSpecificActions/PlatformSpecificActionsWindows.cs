using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BatteryDischarger.PlatformSpecificActions
{
    // https://github.com/pedrolcl/screensaver-disabler/blob/master/ScreenSaverDisabler/Form1.cs
    // Implements Windows power requests using shell commands and the execution-state API.
    public class PlatformSpecificActionsWindows : APlatformSpecificActions
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        // Flags passed to SetThreadExecutionState; continuous requests remain until cleared for the thread.
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        // Clears the continuous away-mode request on the calling thread.
        public override void TryDisablePreventSleep()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        // Keeps the system awake without requiring the display to stay on through away mode.
        public override void TryEnablePreventSleep()
        {
            // https://stackoverflow.com/a/49045894/4172756
            // Prevent Idle-to-Sleep (monitor not affected) (see note above)
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        // Enables hibernation through legacy command forms, then requests forced hibernation.
        public override void TryHibernate()
        {
            PlatformSpecificActionsManager.TryCatchStartProcess("powercfg.exe", "-hibernate on");
            PlatformSpecificActionsManager.TryCatchStartProcess("powercfg.exe", "/hibernate on");
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown", "/h /f");
        }

        // Requests an immediate forced shutdown, which can close applications without saving their work.
        public override void TryShutdown()
        {
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown", "/s /t 0 /f");
        }

        // Disables hibernation before attempting the legacy suspend command, so this also changes a system setting.
        public override void TrySleep()
        {
            PlatformSpecificActionsManager.TryCatchStartProcess("powercfg.exe", "-hibernate off");
            PlatformSpecificActionsManager.TryCatchStartProcess("powercfg.exe", "/hibernate off");
            PlatformSpecificActionsManager.TryCatchStartProcess("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");
        }

        // Offers the actions implemented by this adapter, including the no-power-action choice.
        public override IEnumerable<EndActionEnum> GetSupportedEndActions()
        {
            return new List<EndActionEnum>() { EndActionEnum.Shutdown, EndActionEnum.Sleep, EndActionEnum.Hibernate, EndActionEnum.DoNothing };
        }
    }
}
