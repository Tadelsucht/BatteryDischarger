using Hardware.Info;
using System;

namespace BatteryDischarger.BatteryRelated
{
    public class BatteryInfoManager
    {
        private readonly IHardwareInfo hardwareInfo;

        public BatteryInfoManager() : this(new HardwareInfo())
        {
        }

        public BatteryInfoManager(IHardwareInfo hardwareInfo)
        {
            this.hardwareInfo = hardwareInfo ?? throw new ArgumentNullException(nameof(hardwareInfo));
        }

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

        public int GetEstimatedChargeRemaining()
        {
            var battery = GetBatteryInfo();
            return battery.EstimatedChargeRemaining;
        }

        public int GetEstimatedTimeLeftUntilGivenPerctageHasBeenReached(int targetPercent)
        {
            var battery = GetBatteryInfo();
            return (int)(((float)(battery.EstimatedChargeRemaining - targetPercent) / (float)battery.EstimatedChargeRemaining) * battery.EstimatedRunTime);
        }

        public Battery GetBatteryInfo()
        {
            hardwareInfo.RefreshBatteryList();
            if (hardwareInfo.BatteryList.Count == 0) throw new NotSupportedException(Properties.Resources.NoBatteryWasDetected);
            if (hardwareInfo.BatteryList.Count > 1) throw new NotSupportedException(Properties.Resources.MoreThanOneBatteryWasDetectedWhichIsNotSupportedByTheProgram);
            return hardwareInfo.BatteryList[0];
        }

        // https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery
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
