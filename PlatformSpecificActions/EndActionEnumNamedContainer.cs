using BatteryDischarger.Miscellaneous;
using BatteryDischarger.PlatformSpecificActions;

namespace BatteryDischarger.DataSource
{
    // Pairs a persisted end-action value with the localized text shown in the selection control.
    public class EndActionEnumNamedContainer : NamedContainerBase<EndActionEnum>
    {
        // Retains the enum value separately from its display name for configuration updates.
        public EndActionEnumNamedContainer(EndActionEnum e) : base(e)
        {
        }

        // Keeps legacy technical labels for power actions while presenting DoNothing as a plain localized choice.
        protected override string GetNameFromResources()
        {
            switch (EmbeddedEnum)
            {
                case EndActionEnum.Shutdown:
                    return BatteryDischarger.Properties.Resources.ShutDownDevice + " (Shutdown)";

                case EndActionEnum.Sleep:
                    return BatteryDischarger.Properties.Resources.SetDeviceToPowerSavingMode + " (Sleep)";

                case EndActionEnum.Hibernate:
                    return BatteryDischarger.Properties.Resources.HibernateDevice + " (Hibernate)";

                case EndActionEnum.DoNothing:
                    return BatteryDischarger.Properties.Resources.DoNothing;

                default:
                    return GetDefaultName();
            }
        }
    }
}
