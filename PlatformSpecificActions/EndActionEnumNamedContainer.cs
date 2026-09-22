using BatteryDischarger.Miscellaneous;
using BatteryDischarger.PlatformSpecificActions;

namespace BatteryDischarger.DataSource
{
    public class EndActionEnumNamedContainer : NamedContainerBase<EndActionEnum>
    {
        public EndActionEnumNamedContainer(EndActionEnum e) : base(e)
        {
        }

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
