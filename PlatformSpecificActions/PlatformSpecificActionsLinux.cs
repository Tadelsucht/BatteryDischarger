using System.Collections.Generic;

namespace BatteryDischarger.PlatformSpecificActions
{
    public class PlatformSpecificActionsLinux : APlatformSpecificActions
    {
        public override IEnumerable<EndActionEnum> GetSupportedEndActions()
        {
            return new List<EndActionEnum>() { EndActionEnum.Shutdown, EndActionEnum.Sleep, EndActionEnum.Hibernate };
        }

        public override void TryDisablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_linux.py
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", new List<string>() { "unmask", "sleep.target", "suspend.target", "hibernate.target", "hybrid-sleep.target" });
        }

        public override void TryEnablePreventSleep()
        {
            // https://github.com/np-8/wakepy/blob/master/wakepy/_linux.py
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", new List<string>() { "mask", "sleep.target", "suspend.target", "hibernate.target", "hybrid-sleep.target" });
        }

        public override void TryHibernate()
        {
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", "hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", "hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("pm-hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "pm-hibernate");
        }

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

        public override void TrySleep()
        {
            PlatformSpecificActionsManager.TryCatchStartProcess("systemctl", "suspend");
            PlatformSpecificActionsManager.TryCatchStartProcess("pmi", "action suspend");
            PlatformSpecificActionsManager.TryCatchStartProcess("pm-hibernate");
            PlatformSpecificActionsManager.TryCatchStartProcess("sudo", "pm-suspend");
        }
    }
}