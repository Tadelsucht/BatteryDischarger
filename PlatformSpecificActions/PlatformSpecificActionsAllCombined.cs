using System;
using System.Collections.Generic;

namespace BatteryDischarger.PlatformSpecificActions
{
    public class PlatformSpecificActionsAllCombined : APlatformSpecificActions
    {
        private readonly List<APlatformSpecificActions> Actions = new List<APlatformSpecificActions>();

        public PlatformSpecificActionsAllCombined()
        {
            Actions.Add(new PlatformSpecificActionsWindows());
            Actions.Add(new PlatformSpecificActionsLinux());
            Actions.Add(new PlatformSpecificActionsOSX());
        }

        public override IEnumerable<EndActionEnum> GetSupportedEndActions()
        {
            foreach (EndActionEnum entry in Enum.GetValues(typeof(EndActionEnum))) yield return entry;
        }

        public override void TryDisablePreventSleep()
        {
            foreach (var action in Actions) try { action.TryDisablePreventSleep(); } catch { }
        }

        public override void TryEnablePreventSleep()
        {
            foreach (var action in Actions) try { action.TryEnablePreventSleep(); } catch { }
        }

        public override void TryHibernate()
        {
            foreach (var action in Actions) try { action.TryHibernate(); } catch { }
        }

        public override void TryShutdown()
        {
            foreach (var action in Actions) try { action.TryShutdown(); } catch { }
        }

        public override void TrySleep()
        {
            foreach (var action in Actions) try { action.TrySleep(); } catch { }
        }
    }
}