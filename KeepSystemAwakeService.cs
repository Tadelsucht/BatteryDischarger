using BatteryDischarger.PlatformSpecificActions;

namespace BatteryDischarger
{
    public static class KeepSystemAwakeService
    {
        private static bool _keepSystemAwake;

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