using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BatteryDischarger.PlatformSpecificActions
{
    // https://apple.stackexchange.com/questions/103571/using-the-terminal-command-to-shutdown-restart-and-sleep-my-mac
    // Implements macOS power requests and retains the caffeinate process used during a run.
    public class PlatformSpecificActionsOSX : APlatformSpecificActions
    {
        // Holds the process handle needed to release this adapter's sleep-prevention request.
        private Process osxProcess = null;

        // macOS exposes shutdown, sleep, and no-op choices here; hibernation is explicitly unsupported.
        public override IEnumerable<EndActionEnum> GetSupportedEndActions()
        {
            return new List<EndActionEnum>() { EndActionEnum.Shutdown, EndActionEnum.Sleep, EndActionEnum.DoNothing };
        }

        // Terminates the retained caffeinate process when sleep prevention is no longer requested.
        public override void TryDisablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_darwin.py
            if (osxProcess is not null) osxProcess.Kill();
        }

        // Starts caffeinate for a bounded 30-day interval; process startup is not verified beyond handle creation.
        public override void TryEnablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_darwin.py
            try { osxProcess = Process.Start("caffeinate", new List<string>() { "-d", "-u", "-t 2592000" }); } catch { }
        }

        // Fails explicitly because this adapter does not implement macOS hibernation.
        public override void TryHibernate()
        {
            throw new PlatformNotSupportedException();
        }

        // Tries AppleScript and shutdown command forms without confirming a completed shutdown.
        public override void TryShutdown()
        {
            // not sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("osascript", "-e 'tell app \"System Events\" to shut down'");
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown", "-h now");

            // sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo osascript", " -e 'tell app \"System Events\" to shut down'");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "shutdown -h now");
        }

        // Tries pmset and AppleScript sleep commands without confirming a completed sleep transition.
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
