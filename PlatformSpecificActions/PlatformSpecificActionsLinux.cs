using System.Collections.Generic;

namespace BatteryDischarger.PlatformSpecificActions
{
    // Implements Linux power requests through common systemd and legacy command-line tools.
    public class PlatformSpecificActionsLinux : APlatformSpecificActions
    {
        // Includes hibernation in the UI even though command availability and permissions vary by distribution.
        public override IEnumerable<EndActionEnum> GetSupportedEndActions()
        {
            return new List<EndActionEnum>() { EndActionEnum.Shutdown, EndActionEnum.Sleep, EndActionEnum.Hibernate, EndActionEnum.DoNothing };
        }

        // Unmasks sleep-related systemd targets; this does not track whether a mask predated the application request.
        public override void TryDisablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_linux.py
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", new List<string>() { "unmask", "sleep.target", "suspend.target", "hibernate.target", "hybrid-sleep.target" });
        }

        // Masks sleep-related systemd targets; process launch alone does not confirm that the policy changed.
        public override void TryEnablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_linux.py
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", new List<string>() { "mask", "sleep.target", "suspend.target", "hibernate.target", "hybrid-sleep.target" });
        }

        // Tries common hibernation commands without waiting for or verifying any command's result.
        public override void TryHibernate()
        {
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", "hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", "hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("pm-hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "pm-hibernate");
        }

        // Tries shutdown command variants with and without sudo; this adapter does not confirm which request succeeds.
        public override void TryShutdown()
        {
            // not sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown", "-h now");
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown", "-h 0");
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown", "now");
            PlatformSpecificActionsManager.TryCatchStartProcess("shutdown");

            // sudo
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "shutdown -h now");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "shutdown -h 0");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "shutdown now");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "shutdown");
        }

        // Tries common suspend commands without waiting for or verifying any command's result.
        public override void TrySleep()
        {
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", "suspend");
            PlatformSpecificActionsManager.TryCatchStartProcess("pmi", "action suspend");
            PlatformSpecificActionsManager.TryCatchStartProcess("pm-hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "pm-suspend");
        }
    }
}
