using System;
using System.Collections.Generic;

namespace BatteryDischarger.PlatformSpecificActions
{
    // Fallback adapter for unknown operating systems that tries each known platform implementation.
    public class PlatformSpecificActionsAllCombined : APlatformSpecificActions
    {
        // Keep adapters together so the fallback can attempt each platform-specific mechanism independently.
        private readonly List<APlatformSpecificActions> Actions = new List<APlatformSpecificActions>();

        // Registers the supported platform adapters in their existing attempt order.
        public PlatformSpecificActionsAllCombined()
        {
            Actions.Add(new PlatformSpecificActionsWindows());
            Actions.Add(new PlatformSpecificActionsLinux());
            Actions.Add(new PlatformSpecificActionsOSX());
        }

        // Exposes every enum value because this fallback cannot determine which host commands are available.
        public override IEnumerable<EndActionEnum> GetSupportedEndActions()
        {
            foreach (EndActionEnum entry in Enum.GetValues(typeof(EndActionEnum))) yield return entry;
        }

        // Releases sleep-prevention state on every adapter and isolates failures between platforms.
        public override void TryDisablePreventSleep()
        {
            foreach (var action in Actions) try { action.TryDisablePreventSleep(); } catch { }
        }

        // Requests sleep prevention from every adapter; a returned call does not confirm OS acceptance.
        public override void TryEnablePreventSleep()
        {
            foreach (var action in Actions) try { action.TryEnablePreventSleep(); } catch { }
        }

        // Attempts each adapter's hibernation mechanism without a completion result.
        public override void TryHibernate()
        {
            foreach (var action in Actions) try { action.TryHibernate(); } catch { }
        }

        // Attempts each adapter's shutdown mechanism without a completion result.
        public override void TryShutdown()
        {
            foreach (var action in Actions) try { action.TryShutdown(); } catch { }
        }

        // Attempts each adapter's sleep mechanism without a completion result.
        public override void TrySleep()
        {
            foreach (var action in Actions) try { action.TrySleep(); } catch { }
        }
    }
}
