using System.Collections.Generic;

namespace BatteryDischarger.PlatformSpecificActions
{
    public abstract class APlatformSpecificActions
    {
        public abstract IEnumerable<EndActionEnum> GetSupportedEndActions();

        public abstract void TryShutdown();

        public abstract void TrySleep();

        public abstract void TryHibernate();

        public abstract void TryEnablePreventSleep();

        public abstract void TryDisablePreventSleep();
    }
}