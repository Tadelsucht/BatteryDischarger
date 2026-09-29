using Hardware.Info;
using System;

namespace BatteryDischarger.BatteryRelated
{
    // Reads the single battery supported by the current hardware provider and exposes app-level estimates.
    public class BatteryInfoManager
    {
        // The provider is injectable so battery-dependent behavior can be checked without physical hardware.
        private readonly IHardwareInfo hardwareInfo;

        // Uses the production Hardware.Info provider for normal application startup.
        public BatteryInfoManager() : this(new HardwareInfo())
        {
        }

        // Retains the supplied provider so each query can refresh and inspect its current battery snapshot.
        public BatteryInfoManager(IHardwareInfo hardwareInfo)
        {
            this.hardwareInfo = hardwareInfo ?? throw new ArgumentNullException(nameof(hardwareInfo));
        }

        // Treats the provider's non-charging status values as discharging; unknown and charging values stay false.
        public bool IsBatteryDischaring()
        {
            var battery = GetBatteryInfo();
            if (battery.BatteryStatus == (ushort)BatteryStatusEnum.Other
                || battery.BatteryStatus == (ushort)BatteryStatusEnum.Low
                || battery.BatteryStatus == (ushort)BatteryStatusEnum.Critical
                || battery.BatteryStatus == (ushort)BatteryStatusEnum.PartiallyCharged)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Returns the provider's estimated remaining charge in percent.
        public int GetEstimatedChargeRemaining()
        {
            var battery = GetBatteryInfo();
            return battery.EstimatedChargeRemaining;
        }

        // Returns minutes by scaling provider runtime against the percentage points remaining to targetPercent.
        public int GetEstimatedTimeLeftUntilGivenPerctageHasBeenReached(int targetPercent)
        {
            var battery = GetBatteryInfo();
            return (int)(((float)(battery.EstimatedChargeRemaining - targetPercent) / (float)battery.EstimatedChargeRemaining) * battery.EstimatedRunTime);
        }

        // Refreshes hardware data and rejects zero or multiple batteries because the UI supports exactly one.
        public Battery GetBatteryInfo()
        {
            hardwareInfo.RefreshBatteryList();
            if (hardwareInfo.BatteryList.Count == 0) throw new NotSupportedException(Properties.Resources.NoBatteryWasDetected);
            if (hardwareInfo.BatteryList.Count > 1) throw new NotSupportedException(Properties.Resources.MoreThanOneBatteryWasDetectedWhichIsNotSupportedByTheProgram);
            return hardwareInfo.BatteryList[0];
        }

        // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery
        // Mirrors the Windows battery status codes returned by Hardware.Info.
        protected enum BatteryStatusEnum
        {
            Other = 1,
            Unknown = 2,
            FullyCharged = 3,
            Low = 4,
            Critical = 5,
            Charging = 6,
            ChargingAndHigh = 7,
            ChargingAndLow = 8,
            ChargingAndCritical = 9,
            Undefined = 10,
            PartiallyCharged = 11
        }
    }
}
