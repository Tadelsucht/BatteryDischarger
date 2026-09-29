using System.Collections.Generic;

namespace BatteryDischarger.PlatformSpecificActions
{
    // Defines the OS boundary for end actions and sleep-prevention requests; calls do not report completion status.
    public abstract class APlatformSpecificActions
    {
        // Lists choices the current adapter exposes to the user.
        public abstract IEnumerable<EndActionEnum> GetSupportedEndActions();

        // Requests an operating-system shutdown using the adapter's platform mechanism.
        public abstract void TryShutdown();

        // Requests system sleep; implementations may depend on installed platform commands or permissions.
        public abstract void TrySleep();

        // Requests hibernation where the platform supports it.
        public abstract void TryHibernate();

        // Requests that ordinary idle sleep be prevented for the active application run.
        public abstract void TryEnablePreventSleep();

        // Releases the adapter's sleep-prevention request when the run ends.
        public abstract void TryDisablePreventSleep();
    }
}
