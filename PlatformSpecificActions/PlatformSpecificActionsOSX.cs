using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BatteryDischarger.PlatformSpecificActions
{
    // https://apple.stackexchange.com/questions/103571/using-the-terminal-command-to-shutdown-restart-and-sleep-my-mac
    public class PlatformSpecificActionsOSX : APlatformSpecificActions
    {
        private Process osxProcess = null;

        public override IEnumerable<EndActionEnum> GetSupportedEndActions()
        {
            return new List<EndActionEnum>() { EndActionEnum.Shutdown, EndActionEnum.Sleep };
        }

        public override void TryDisablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_darwin.py
            if (osxProcess is not null) osxProcess.Kill();
        }

        public override void TryEnablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_darwin.py
            try { osxProcess = Process.Start("caffeinate", new List<string>() { "-d", "-u", "-t 2592000" }); } catch { }
        }

        public override void TryHibernate()
        {
            throw new PlatformNotSupportedException();
        }

        public override void TryShutdown()
        {
            // not sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("osascript", "-e 'tell app \"System Events\" to shut down'");
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown", "-h now");

            // sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo osascript", " -e 'tell app \"System Events\" to shut down'");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "shutdown -h now");
        }

        public override void TrySleep()
        {
            // not sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("pmset", "sleepnow");
            PlatformSpecificActionsManager.TryCatchStartProcess("osascript", "-e 'tell application \"System Events\" to sleep");

            // sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "pmset sleepnow");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "osascript -e 'tell application \"System Events\" to sleep");
        }
    }
}