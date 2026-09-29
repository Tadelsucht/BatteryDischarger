namespace BatteryDischarger.PlatformSpecificActions
{
    // These names are persisted in Configuration.ini, so existing values must remain stable.
    public enum EndActionEnum
    {
        Shutdown,
        Sleep,
        Hibernate,
        // Completes controlled discharge without asking the operating system for a power action.
        DoNothing
    }
}
