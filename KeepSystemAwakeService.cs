using BatteryDischarger.PlatformSpecificActions;

namespace BatteryDischarger
{
    // Bridges the user's keep-awake preference to the active platform action provider.
    public static class KeepSystemAwakeService
    {
        // Tracks the requested setting; it does not confirm that the operating system accepted the request.
        private static bool _keepSystemAwake;

        // Forwards the request and stores it if the adapter call returns; the value does not confirm OS acceptance.
        public static bool KeepSystemAwake
        {
            get
            {
                return _keepSystemAwake;
            }
            set
            {
                if (value)
                {
                    PlatformSpecificActionsManager.PlatformSpecificEndActions.TryEnablePreventSleep();
                }
                else
                {
                    PlatformSpecificActionsManager.PlatformSpecificEndActions.TryDisablePreventSleep();
                }
                _keepSystemAwake = value;
            }
        }
    }
}
